/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-14
 */
using System;
using System.Linq;
using Android.Webkit;
using System.Xml.Linq;
using System.IO;
using Webkit = Android.Webkit;
using Android.Content;
using Android.App;
using Android.Util;
using System.Text;
using OpenIZ.Mobile.Core.Android.Configuration;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics;
using OpenIZ.Mobile.Core.Android.AppletEngine.JNI;
using System.Collections.Generic;
using Android.Widget;
using System.Xml;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Diagnostics.Tracing;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Applets;
using System.Security;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Xamarin.Security;
using A = Android;
using System.Net;

namespace OpenIZ.Mobile.Core.Android.AppletEngine
{
    /// <summary>
    /// Represents a window which can execute javascript
    /// </summary>
    public sealed class AppletWebView : WebView
    {


        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletWebView));

        /// <summary>
        /// Thread safe title
        /// </summary>
        public string ThreadSafeTitle { get; internal set; }

        /// <summary>
        /// Create a new webview
        /// </summary>
        /// <param name="context">Context.</param>
        public AppletWebView(Context context) : base(context)
        {
            this.Initialize(context);

        }

        /// <summary>
        /// Attaches the applet executor to a web view
        /// </summary>
        public AppletWebView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.Initialize(context);

        }

        /// <summary>
        /// Initialize
        /// </summary>
        private void Initialize(Context context)
        {
            this.m_tracer.TraceVerbose("Initializing applet web viewer");
#if DEBUG
            this.Settings.CacheMode = CacheModes.Default;

#else
            this.Settings.CacheMode = CacheModes.Default;
#endif

            this.ClearCache(true);
            WebView.SetWebContentsDebuggingEnabled(true);
            this.Settings.JavaScriptEnabled = true;
            this.Settings.BlockNetworkLoads = false;
            this.Settings.BuiltInZoomControls = false;
            this.Settings.DisplayZoomControls = false;
            this.Settings.UserAgentString = $"OpenIZ-DC {ApplicationContext.Current.ExecutionUuid}";
            this.Settings.DatabaseEnabled = false;
            this.Settings.JavaScriptCanOpenWindowsAutomatically = false;
            this.Settings.PluginsEnabled = false;
            this.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
            this.Settings.SetAppCacheEnabled(true);
            this.Settings.SetSupportMultipleWindows(false);
            if (A.OS.Build.VERSION.SdkInt >= A.OS.BuildVersionCodes.Kitkat)
            {
                this.SetLayerType(A.Views.LayerType.Hardware, null);
            }
            else
            {
                this.SetLayerType(A.Views.LayerType.Software, null);
            }

            this.AddJavascriptInterface(new AppletFunctionBridge(context, this), "OpenIZApplicationService");
            //this.AddJavascriptInterface(new ConfigurationServiceBridge(), "OpenIZConfigurationService");
            //this.AddJavascriptInterface(new SessionServiceBridge(), "OpenIZSessionService");
            this.SetWebViewClient(new AppletWebViewClient());
            this.RequestFocus(A.Views.FocusSearchDirection.Down);
            this.SetWebChromeClient(new AppletWebChromeClient(this.Context));
        }

        /// <summary>
        /// Load URL
        /// </summary>
        /// <param name="url"></param>
        public override void LoadUrl(string url)
        {
            Uri uri = null;
            var unlockDictionary = new Dictionary<String, String>()
            {
                {  "X-OIZMagic", ApplicationContext.Current.ExecutionUuid.ToString() }
            };

            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                base.LoadUrl("http://127.0.0.1:9200/views/errors/404.html", unlockDictionary);
            else if (uri.IsAbsoluteUri && uri.Host == "127.0.0.1" &&
                uri.Port == 9200)
                base.LoadUrl(url, unlockDictionary);
            else if (!uri.IsAbsoluteUri)
                base.LoadUrl(new Uri(new Uri("http://127.0.0.1:9200/"), uri.PathAndQuery).ToString(), unlockDictionary);
            else
                base.LoadUrl("http://127.0.0.1:9200/views/errors/404.html", unlockDictionary);
        }
         
        /// <summary>
        /// Execute the specified javascript
        /// </summary>
        /// <param name="javascript">Javascript.</param>
        public string Execute(String javascript)
        {
            this.m_tracer.TraceVerbose("Execute Javascript: {0}", javascript);
            if (String.IsNullOrEmpty(javascript))
                throw new ArgumentNullException(nameof(javascript));
            this.LoadUrl("javascript:" + javascript);
            return "";
        }


        /// <summary>
        /// Applet web view client.
        /// </summary>
        private class AppletWebViewClient : WebViewClient
        {

            // Tracer
            private Tracer m_tracer = Tracer.GetTracer(typeof(AppletWebView));

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="OpenIZ.Mobile.Core.Android.AppletEngine.AppletWebView+AppletWebViewClient"/> class.
            /// </summary>
            /// <param name="applet">Applet.</param>
            public AppletWebViewClient()
            {
            }

            
            /// <summary>
            /// On page is finished
            /// </summary>
            public override void OnLoadResource(WebView view, string url)
            {
                (view as AppletWebView).ThreadSafeTitle = view.Title;
                base.OnLoadResource(view, url);
            }

            /// <param name="view">The WebView that is initiating the callback.</param>
            /// <param name="url">The url to be loaded.</param>
            /// <summary>
            /// Give the host application a chance to take over the control when a new
            ///  url is about to be loaded in the current WebView.
            /// </summary>
            /// <returns>To be added.</returns>
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
            
                this.m_tracer.TraceInfo("Overridding url {0}", url);

                view.LoadUrl(url);
                return true;
            }
            
            ///// <summary>
            ///// Page is finished
            ///// </summary>
            //public override void OnPageFinished(WebView view, string url)
            //{
            //    AndroidApplicationContext.Current.SetProgress(null, 1.0f);
            //}
        }

        /// <summary>
        /// Chrome client
        /// </summary>
        private class AppletWebChromeClient : WebChromeClient
        {

            // Tracer
            private Tracer m_tracer = Tracer.GetTracer(typeof(AppletWebChromeClient));

            // Context
            private Context m_context;

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="OpenIZ.Mobile.Core.Android.AppletEngine.AppletWebView+AppletWebChromeClient"/> class.
            /// </summary>
            /// <param name="context">Context.</param>
            public AppletWebChromeClient(Context context)
            {
                this.m_context = context;
            }

            /// <param name="view">The WebView that initiated the callback.</param>
            /// <param name="url">The url of the page requesting the dialog.</param>
            /// <param name="message">Message to be displayed in the window.</param>
            /// <param name="result">A JsResult to confirm that the user hit enter.</param>
            /// <summary>
            /// Javascript alert
            /// </summary>
            /// <returns>To be added.</returns>
            public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
            {
                var alertDialogBuilder = new AlertDialog.Builder(this.m_context)
                     .SetMessage(message)
                    .SetCancelable(false)
                    .SetPositiveButton(this.m_context.Resources.GetString(Resource.String.confirm), (sender, args) =>
                    {
                        result.Confirm();
                    });


                alertDialogBuilder.Create().Show();

                return true;

            }

            /// <param name="consoleMessage">Object containing details of the console message.</param>
            /// <summary>
            /// Report a JavaScript console message to the host application.
            /// </summary>
            /// <returns>To be added.</returns>
            public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
            {
                var retVal = base.OnConsoleMessage(consoleMessage);
                
                // Start off verbose
                EventLevel eventLevel = EventLevel.Verbose;
                if (consoleMessage.InvokeMessageLevel() == Webkit.ConsoleMessage.MessageLevel.Error)
                {
                    //Toast.MakeText(this.m_context, "This applet reported an error", ToastLength.Long).Show();
                    eventLevel = EventLevel.Error;
                }
                else if (consoleMessage.InvokeMessageLevel() == Webkit.ConsoleMessage.MessageLevel.Warning)
                    eventLevel = EventLevel.Warning;
                else if (consoleMessage.InvokeMessageLevel() == Webkit.ConsoleMessage.MessageLevel.Log)
                    eventLevel = EventLevel.Informational;

                this.m_tracer.TraceEvent(eventLevel, "[{0}:{1}] {2}", consoleMessage.SourceId(), consoleMessage.LineNumber(), consoleMessage.Message());
                return retVal;
            }

            /// <param name="view">The WebView that initiated the callback.</param>
            /// <param name="url">The url of the page requesting the dialog.</param>
            /// <param name="message">Message to be displayed in the window.</param>
            /// <param name="result">A JsResult used to send the user's response to
            ///  javascript.</param>
            /// <summary>
            /// Fired when a javascript confirm should be shown
            /// </summary>
            /// <returns>To be added.</returns>
            public override bool OnJsConfirm(WebView view, string url, string message, JsResult result)
            {
                var alertDialogBuilder = new AlertDialog.Builder(this.m_context)
                    .SetMessage(message)
                    .SetCancelable(false)
                    .SetPositiveButton(this.m_context.Resources.GetString(Resource.String.confirm), (sender, args) =>
                    {
                        result.Confirm();
                    })
                    .SetNegativeButton(this.m_context.Resources.GetString(Resource.String.cancel), (sender, args) =>
                    {
                        result.Cancel();
                    });

                alertDialogBuilder.Create().Show();


                return true;

            }

            /// <summary>
            /// Progress has changed
            /// </summary>
            public override void OnProgressChanged(WebView view, int newProgress)
            {
                base.OnProgressChanged(view, newProgress);
                AndroidApplicationContext.Current.SetProgress(view.Url, (float)newProgress / 100);
            }
        }
    }
}

