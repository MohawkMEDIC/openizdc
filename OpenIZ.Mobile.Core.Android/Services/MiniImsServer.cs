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

namespace OpenIZ.Mobile.Core.Android.Services
{
    /// <summary>
    /// Represents a mini IMS server that the web-view can access
    /// </summary>
    [Service(Enabled = true)]
    public class MiniImsServer : IDaemonService
    {

        // Mini-listener
        private HttpListener m_listener;
        private Thread m_acceptThread;
        private Tracer m_tracer = Tracer.GetTracer(typeof(MiniImsServer));
        private List<AppletAsset> m_scanApplets = new List<AppletAsset>();
        private Dictionary<String, AppletAsset> m_cacheApplets = new Dictionary<string, AppletAsset>();
        private object m_lockObject = new object();

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
                AndroidApplicationContext.Current.SetProgress("IMS Service Bus", 0);
                this.m_listener = new HttpListener();

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
                            OpenIZThreadPool.Current.QueueUserWorkItem(this.HandleRequest, context);
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

            // Scan
            if(this.m_scanApplets.Count == 0)
                lock(this.m_lockObject)
                {
                    if(AndroidApplicationContext.Current.LoadedApplets.DefaultApplet != null)
                        this.m_scanApplets.Add(AndroidApplicationContext.Current.LoadedApplets.DefaultApplet.Assets[0]);
                    this.m_scanApplets.AddRange(
                        AndroidApplicationContext.Current.LoadedApplets.Where(o => o != AndroidApplicationContext.Current.LoadedApplets.DefaultApplet)
                        .Select(o => o.Assets[0]));
                }

            HttpListenerContext context = state as HttpListenerContext;

            // classify request
            HttpListenerRequest request = context.Request;
            if (request.Url.AbsolutePath.StartsWith("/__imsi"))
                this.HandleImsiReqeust(context);
            else
            {
                this.HandleAssetRenderRequest(context);
            }
        }

        /// <summary>
        /// Handle IMSI request
        /// </summary>
        private void HandleImsiReqeust(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle render asset request
        /// </summary>
        /// <param name="context"></param>
        private void HandleAssetRenderRequest(HttpListenerContext context)
        {

            this.m_tracer.TraceInfo("Intercept request for {0}...", context.Request.Url);
            var request = context.Request;
            var response = context.Response;
            
            // Try to demand policy 
            try
            {

                // Navigate asset
                AppletAsset navigateAsset = null;

                if (!this.m_cacheApplets.TryGetValue(request.Url.AbsolutePath, out navigateAsset))
                {
                    
                    navigateAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(request.Url.AbsolutePath);

                   if (navigateAsset == null)
                        throw new FileNotFoundException();

                    lock (m_lockObject)
                        if (!this.m_cacheApplets.ContainsKey(request.Url.AbsolutePath))
                            this.m_cacheApplets.Add(request.Url.AbsolutePath, navigateAsset);
                }


                if (navigateAsset.MimeType == "text/html")
                {
                    string auth = request.Headers["X-Authorization"],
                         magic = request.Headers["X-MAGIC"];
                    var expectedAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", AndroidApplicationContext.Current.Application.Name, AndroidApplicationContext.Current.Application.ApplicationSecret)));
                    var expectedMagic = Convert.ToBase64String(AndroidApplicationContext.Current.Device.Key.Value.ToByteArray());
                    if (expectedAuth != auth &&
                        expectedMagic != magic)
                        throw new UnauthorizedAccessException();
                }

#if DEBUG
                response.AddHeader("Cache-Control", "no-cache");
#endif

                // Navigate policy?
                if (navigateAsset.Policies != null)
                    foreach (var policy in navigateAsset.Policies)
                        new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, policy).Demand();

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
                        int br = 1024;
                        byte[] buffer = new byte[1024];
                        while (br == 1024)
                        {
                            br = fs.Read(buffer, 0, 1024);
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
            catch (UnauthorizedAccessException ex)
            {
                this.m_tracer.TraceError(ex.ToString());
                string redirectLocation = String.Format("/org.openiz.core/views/errors/403.html?returnUrl={0}",
                        request.RawUrl);
                response.Redirect(redirectLocation);
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
    }
}