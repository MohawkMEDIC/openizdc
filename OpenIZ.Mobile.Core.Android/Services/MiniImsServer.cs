using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Services;
using System.Net;
using OpenIZ.Mobile.Core.Android.Threading;
using System.Threading;
using System.Net.Sockets;
using System.Globalization;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using OpenIZ.Mobile.Core.Android.Security;
using System.Security;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Reflection;
using OpenIZ.Mobile.Core.Android.Services.Attributes;
using OpenIZ.Core.Http;
using OpenIZ.Mobile.Core.Android.Resources;

namespace OpenIZ.Mobile.Core.Android.Services
{
    /// <summary>
    /// Represents a mini IMS server that the web-view can access
    /// </summary>
    [Service(Enabled = true)]
    public class MiniImsServer : IDaemonService
    {

        // Current context
        [ThreadStatic]
        public static HttpListenerContext CurrentContext;

        // Mini-listener
        private HttpListener m_listener;
        private Thread m_acceptThread;
        private Tracer m_tracer = Tracer.GetTracer(typeof(MiniImsServer));
        private List<AppletAsset> m_scanApplets = new List<AppletAsset>();
        private Dictionary<String, AppletAsset> m_cacheApplets = new Dictionary<string, AppletAsset>();
        private object m_lockObject = new object();
        private Dictionary<String, InvokationInformation> m_services = new Dictionary<string, InvokationInformation>();
        private IContentTypeMapper m_contentTypeHandler = new DefaultContentTypeMapper();
        private IThreadPoolService m_threadPool = null;

        /// <summary>
        /// Returns true if the service is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_listener?.IsListening == true;
            }
        }

        public event EventHandler Started;
        public event EventHandler Starting;
        public event EventHandler Stopped;
        public event EventHandler Stopping;

        public bool Start()
        {
            try
            {
                this.Starting?.Invoke(this, EventArgs.Empty);

                this.m_tracer.TraceInfo("Starting internal IMS services...");
                this.m_threadPool = ApplicationContext.Current.GetService<IThreadPoolService>();

                AndroidApplicationContext.Current.SetProgress("IMS Service Bus", 0);
                this.m_listener = new HttpListener();

                // Scan for services
                foreach (var t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.DefinedTypes).Where(o => o.GetCustomAttribute<RestServiceAttribute>() != null))
                {
                    var serviceAtt = t.GetCustomAttribute<RestServiceAttribute>();
                    object instance = Activator.CreateInstance(t);
                    foreach (var mi in t.GetRuntimeMethods().Where(o => o.GetCustomAttribute<RestOperationAttribute>() != null))
                    {
                        var operationAtt = mi.GetCustomAttribute<RestOperationAttribute>();
                        var faultMethod = operationAtt.FaultProvider != null ? t.GetRuntimeMethod(operationAtt.FaultProvider, new Type[] { typeof(Exception) }) : null;
                        String pathMatch = String.Format("{0}:{1}{2}", operationAtt.Method, serviceAtt.BaseAddress, operationAtt.UriPath);
                        if (!this.m_services.ContainsKey(pathMatch))
                            lock (this.m_lockObject)
                                this.m_services.Add(pathMatch, new InvokationInformation()
                                {
                                    BindObject = instance,
                                    Method = mi,
                                    FaultProvider = faultMethod
                                });

                    }

                }

                // Get loopback
                var loopback = GetLocalIpAddress();

                // Core always on 9200
                this.m_listener.Prefixes.Add(String.Format("http://{0}:9200/", loopback));

                this.m_acceptThread = new Thread(() =>
                {
                    // Process the request
                    while (this.m_listener != null)
                    {
                        try
                        {
                            var iAsyncResult = this.m_listener.BeginGetContext(null, null);
                            iAsyncResult.AsyncWaitHandle.WaitOne();
                            var context = this.m_listener.EndGetContext(iAsyncResult);
                            //new Thread(this.HandleRequest).Start(context);
                            this.m_threadPool.QueueUserWorkItem(this.HandleRequest, context);
                        }
                        catch (Exception e)
                        {
                            this.m_tracer.TraceError("Listener Error: {0}", e);
                        }
                    }
                });

                this.m_listener.Start();
                this.m_acceptThread.IsBackground = true;
                this.m_acceptThread.Start();
                this.m_tracer.TraceInfo("Started internal IMS services...");
                this.Started?.Invoke(this, EventArgs.Empty);

                return true;
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error starting IMS : {0}", ex);
                return false;
            }
        }

        /// <summary>
        /// Don't know why but the device doesn't have a loopback interface by default?
        /// </summary>
        /// <returns></returns>
        private static string GetLocalIpAddress()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                string address = ((IPEndPoint)listener.LocalEndpoint).Address.ToString();
                return address;
            }
            finally
            {
                listener.Stop();
            }
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        private void HandleRequest(Object state)
        {

            HttpListenerContext context = state as HttpListenerContext;
            var request = context.Request;
            var response = context.Response;
            try
            {
                MiniImsServer.CurrentContext = context;

                // Scan
                if (this.m_scanApplets.Count == 0)
                    lock (this.m_lockObject)
                    {
                        if (AndroidApplicationContext.Current.LoadedApplets.DefaultApplet != null)
                            this.m_scanApplets.Add(AndroidApplicationContext.Current.LoadedApplets.DefaultApplet.Assets[0]);
                        this.m_scanApplets.AddRange(
                            AndroidApplicationContext.Current.LoadedApplets.Where(o => o != AndroidApplicationContext.Current.LoadedApplets.DefaultApplet)
                            .Select(o => o.Assets[0]));
                    }

                // Attempt to find a service which implements the path
                var rootPath = String.Format("{0}:{1}", request.HttpMethod.ToUpper(), request.Url.AbsolutePath);
                InvokationInformation invoke = null;
                if (this.m_services.TryGetValue(rootPath, out invoke))
                {

                    this.m_tracer.TraceVerbose("Matched path {0} to handler {1}.{2}", rootPath, invoke.BindObject.GetType().FullName, invoke.Method.Name);
                    // Get the method information 
                    var parmInfo = invoke.Method.GetParameters();
                    object result = null;

                    try
                    {
                        if (parmInfo.Length == 0)
                            result = invoke.Method.Invoke(invoke.BindObject, new object[] { });
                        else
                        {
                            var serializer = this.m_contentTypeHandler.GetSerializer(request.ContentType, parmInfo[0].ParameterType);
                            var pValue = serializer.DeSerialize(request.InputStream);
                            result = invoke.Method.Invoke(invoke.BindObject, new object[] { pValue });
                        }
                        response.StatusCode = 200;
                    }
                    catch(TargetInvocationException e) {
                        this.m_tracer.TraceError("Error executing service request: {0}", e);
                        if (invoke.FaultProvider != null)
                        {
                            response.StatusCode = 500;
                            result = invoke.FaultProvider.Invoke(invoke.BindObject, new object[] { e.InnerException });
                        }
                        else
                            throw e.InnerException;
                    }

                    // Serialize the response
                    if (request.Headers["Accept"] != null)
                    {
                        var serializer = this.m_contentTypeHandler.GetSerializer(request.Headers["Accept"].Split(',')[0], result.GetType());
                        if (serializer != null)
                        {
                            response.ContentType = request.Headers["Accept"].Split(',')[0];
                            serializer.Serialize(response.OutputStream, result);
                        }
                        else
                            throw new ArgumentOutOfRangeException(Strings.err_invalid_accept);
                    }
                    else // Use the contract values
                        switch (invoke.Method.ReturnParameter.GetCustomAttribute<RestMessageAttribute>().MessageFormat)
                        {
                            case RestMessageFormat.Json:
                                response.ContentType = "application/json";
                                this.m_contentTypeHandler.GetSerializer("application/json", invoke.Method.ReturnType).Serialize(response.OutputStream, result.GetType());
                                break;
                            case RestMessageFormat.Xml:
                                response.ContentType = "application/xml";
                                this.m_contentTypeHandler.GetSerializer("application/xml", invoke.Method.ReturnType).Serialize(response.OutputStream, result.GetType());
                                break;
                        }
                }
                else
                    this.HandleAssetRenderRequest(request, response);

            }
            catch (UnauthorizedAccessException ex)
            {
                this.m_tracer.TraceError(ex.ToString());
                response.StatusCode = 403;
                var errAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("/org.openiz.core/views/errors/403.html",
                        this.m_scanApplets[0]);
                var buffer = AndroidApplicationContext.Current.LoadedApplets.RenderAssetContent(errAsset, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                response.OutputStream.Write(buffer, 0, buffer.Length);

            }
            catch (SecurityException ex)
            {
                this.m_tracer.TraceError(ex.ToString());
                if (ApplicationContext.Current.Principal == null)
                {
                    string redirectLocation = String.Format("{0}?returnUrl={1}",
                        AndroidApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().AuthenticationAsset
                        , request.RawUrl);
                    response.Redirect(redirectLocation);
                }
                else
                {
                    response.StatusCode = 403;

                    var errAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("/org.openiz.core/views/errors/403.html",
                        this.m_scanApplets[0]);
                    var buffer = AndroidApplicationContext.Current.LoadedApplets.RenderAssetContent(errAsset, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }

            }
            catch (FileNotFoundException ex)
            {
                this.m_tracer.TraceError(ex.ToString());
                response.StatusCode = 404;
                var errAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("/org.openiz.core/views/errors/404.html",
                    this.m_scanApplets[0]);
                var buffer = AndroidApplicationContext.Current.LoadedApplets.RenderAssetContent(errAsset, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                response.OutputStream.Write(buffer, 0, buffer.Length);

            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError(ex.ToString());
                response.StatusCode = 500;
                var errAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("/org.openiz.core/views/errors/500.html",
                    this.m_scanApplets[0]);
                var buffer = AndroidApplicationContext.Current.LoadedApplets.RenderAssetContent(errAsset, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                try
                {
                    response.Close();
                }
                catch { }

                MiniImsServer.CurrentContext = null;
            }

        }

        /// <summary>
        /// Handle render asset request
        /// </summary>
        /// <param name="context"></param>
        private void HandleAssetRenderRequest(HttpListenerRequest request, HttpListenerResponse response)
        {

            this.m_tracer.TraceInfo("Intercept request for {0}...", request.Url);

            // Try to demand policy 

            // Navigate asset
            AppletAsset navigateAsset = null;

            if (!this.m_cacheApplets.TryGetValue(request.Url.AbsolutePath, out navigateAsset))
            {

                navigateAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(request.Url.AbsolutePath);

                if (navigateAsset == null)
                    throw new FileNotFoundException(request.RawUrl);

                lock (m_lockObject)
                    if (!this.m_cacheApplets.ContainsKey(request.Url.AbsolutePath))
                        this.m_cacheApplets.Add(request.Url.AbsolutePath, navigateAsset);
            }

            // Block access to HTML f
            if (navigateAsset.MimeType == "text/html")
            {
                string auth = request.Headers["Authorization"];

                var expectedAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", AndroidApplicationContext.Current.Application.Name, AndroidApplicationContext.Current.Application.ApplicationSecret)));
                if (expectedAuth != auth)
                    throw new UnauthorizedAccessException();
            }

#if DEBUG
            response.AddHeader("Cache-Control", "no-cache");
#endif

            // Navigate policy?
            if (navigateAsset.Policies != null)
                foreach (var policy in navigateAsset.Policies)
                    new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, policy).Demand();

            response.ContentType = navigateAsset.MimeType;
            // Write asset
            if (navigateAsset.Content == null)
            {
                String itmPath = System.IO.Path.Combine(
                            ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().AppletDirectory,
                            "assets",
                            navigateAsset.Manifest.Info.Id,
                            navigateAsset.Name);
                using (var fs = File.OpenRead(itmPath))
                {
                    int br = 8092;
                    byte[] buffer = new byte[8092];
                    while (br == 8092)
                    {
                        br = fs.Read(buffer, 0, 8092);
                        response.OutputStream.Write(buffer, 0, br);
                    }
                }
            }
            else
            {
                var content = AndroidApplicationContext.Current.LoadedApplets.RenderAssetContent(navigateAsset, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                response.OutputStream.Write(content, 0, content.Length);

            }

        }

        /// <summary>
        /// Stop the listener
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.m_tracer.TraceInfo("Stopping IMS services...");
            this.m_listener.Stop();
            this.m_listener = null;
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Represents service invokation information
        /// </summary>
        private class InvokationInformation
        {

            /// <summary>
            /// Bind object
            /// </summary>
            public Object BindObject { get; set; }

            /// <summary>
            /// Fault provider
            /// </summary>
            public MethodInfo FaultProvider { get; set; }

            /// <summary>
            /// The method for the specified URL template
            /// </summary>
            public MethodInfo Method { get; set; }

        }
    }
}