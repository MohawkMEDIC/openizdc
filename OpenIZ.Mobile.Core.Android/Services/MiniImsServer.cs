/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-7-18
 */
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
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Core.Model;
using OpenIZ.Core.Services;

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
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                    try
                    {
                        foreach (var t in a.DefinedTypes.Where(o => o.GetCustomAttribute<RestServiceAttribute>() != null))
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
                    }
                    catch(Exception e)
                    {
                        this.m_tracer.TraceWarning("Could not load assembly {0} : {1}", a, e);
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
                            //var iAsyncResult = this.m_listener.BeginGetContext(null, null);
                            //iAsyncResult.AsyncWaitHandle.WaitOne();
                            var context = this.m_listener.GetContext(); //this.m_listener.EndGetContext(iAsyncResult);
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
                this.m_acceptThread.Name = "MiniIMS";
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
                            if (parmInfo[0].GetCustomAttribute<RestMessageAttribute>()?.MessageFormat == RestMessageFormat.SimpleJson)
                            {
                                using (StreamReader sr = new StreamReader(request.InputStream))
                                {
                                    var dserMethod = typeof(JsonViewModelSerializer).GetMethod("DeSerialize").MakeGenericMethod(new Type[] { parmInfo[0].ParameterType });
                                    var pValue = dserMethod.Invoke(null, new object[] { sr.ReadToEnd() });
                                    result = invoke.Method.Invoke(invoke.BindObject, new object[] { pValue });
                                }
                            }
                            else
                            {
                                var serializer = this.m_contentTypeHandler.GetSerializer(request.ContentType, parmInfo[0].ParameterType);
                                var pValue = serializer.DeSerialize(request.InputStream);
                                result = invoke.Method.Invoke(invoke.BindObject, new object[] { pValue });
                            }
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
                    if (request.Headers["Accept"] != null && invoke.Method.ReturnParameter.GetCustomAttribute<RestMessageAttribute>()?.MessageFormat != RestMessageFormat.Raw &&
                        invoke.Method.ReturnParameter.GetCustomAttribute<RestMessageAttribute>()?.MessageFormat != RestMessageFormat.SimpleJson)
                    {
                        var serializer = this.m_contentTypeHandler.GetSerializer(request.Headers["Accept"].Split(',')[0], result?.GetType() ?? typeof(IdentifiedData));
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
                            case RestMessageFormat.Raw:
                                byte[] buffer = new byte[2048];
                                int br = 2048;
                                while (br == 2048)
                                {
                                    br = (result as Stream).Read(buffer, 0, 2048);
                                    response.OutputStream.Write(buffer, 0, br);
                                }
                                break;
                            case RestMessageFormat.SimpleJson:
                                response.ContentType = "application/json";
                                if (result is IdentifiedData)
                                {
                                    using (StreamWriter sw = new StreamWriter(response.OutputStream))
                                    {
                                        sw.Write(JsonViewModelSerializer.Serialize((result as IdentifiedData).GetLocked()));
                                    }
                                }
                                else if(result != null)
                                    this.m_contentTypeHandler.GetSerializer("application/json", result.GetType()).Serialize(response.OutputStream, result);

                                break;
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
                var errAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("/org.openiz.core/views/errors/403.html");
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

                    var errAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("/org.openiz.core/views/errors/403.html");
                    var buffer = AndroidApplicationContext.Current.LoadedApplets.RenderAssetContent(errAsset, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }

            }
            catch (FileNotFoundException ex)
            {
                this.m_tracer.TraceError(ex.ToString());
                response.StatusCode = 404;
                var errAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("/org.openiz.core/views/errors/404.html");
                var buffer = AndroidApplicationContext.Current.LoadedApplets.RenderAssetContent(errAsset, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                response.OutputStream.Write(buffer, 0, buffer.Length);

            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError(ex.ToString());
                response.StatusCode = 500;
                var errAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("/org.openiz.core/views/errors/500.html");
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
		/// Handles the process of rendering an asset.
		/// </summary>
		/// <param name="request">The HTTP request.</param>
		/// <param name="response">The HTTP response.</param>
        private void HandleAssetRenderRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
            // Try to demand policy 

            // Navigate asset
            AppletAsset navigateAsset = null;
            String appletPath = request.Url.AbsolutePath.ToLower();
            if (!this.m_cacheApplets.TryGetValue(appletPath, out navigateAsset))
            {

                navigateAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(appletPath);

                if (navigateAsset == null)
				{
					throw new FileNotFoundException(request.RawUrl);
				}

                lock (m_lockObject)
				{
					if (!this.m_cacheApplets.ContainsKey(appletPath))
					{
						this.m_cacheApplets.Add(appletPath, navigateAsset);
					}
				}           
            }
#if DEBUG
            response.AddHeader("Cache-Control", "no-cache");
#endif

            // Navigate policy?
            if (navigateAsset.Policies != null)
			{
				foreach (var policy in navigateAsset.Policies)
				{
					new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, policy).Demand();
				}
			}

            response.ContentType = navigateAsset.MimeType;

            // Write asset
            if (navigateAsset.Content == null)
            {
                var content = AndroidApplicationContext.Current.GetAppletAssetFile(navigateAsset);
                response.OutputStream.Write(content, 0, content.Length);
            }
            else
            {
				string languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

				if (languageCode == null)
				{
					languageCode = "en";
				}

				var content = AndroidApplicationContext.Current.LoadedApplets.RenderAssetContent(navigateAsset, languageCode);

                response.OutputStream.Write(content, 0, content.Length);
            }
        }

		/// <summary>
		/// Stops the IMS listener.
		/// </summary>
		/// <returns>Returns true if the listener stopped successfully.</returns>
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