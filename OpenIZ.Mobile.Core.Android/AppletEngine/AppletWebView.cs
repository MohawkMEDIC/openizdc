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
using OpenIZ.Mobile.Core.Android.Security;

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
		/// Occurs when applet changed.
		/// </summary>
		public event EventHandler AssetChanged;

		// A web-view
		private AppletAsset m_asset;

		// Queue
		private Stack<AppletAsset> m_backQueue = new Stack<AppletAsset> ();

		/// <summary>
		/// Create a new webview
		/// </summary>
		/// <param name="context">Context.</param>
		public AppletWebView(Context context) : base(context)
		{
			this.Initialize (context);

		}

		/// <summary>
		/// Attaches the applet executor to a web view
		/// </summary>
		public AppletWebView (Context context, IAttributeSet attrs) : base(context, attrs)
		{
			this.Initialize (context);

		}


		/// <summary>
		/// Initialize
		/// </summary>
		private void Initialize(Context context)
		{
			this.m_tracer.TraceVerbose ("Initializing applet web viewer");
			this.Settings.CacheMode = CacheModes.NoCache;
			this.Settings.JavaScriptEnabled = true;
			this.Settings.BlockNetworkLoads = true;
			this.Settings.BuiltInZoomControls = false;
			this.Settings.DisplayZoomControls = false;
			this.AddJavascriptInterface (new AppletFunctionBridge (context, this), "OpenIZApplicationService");
			this.AddJavascriptInterface (new ConfigurationServiceBridge(), "OpenIZConfigurationService");
			this.AddJavascriptInterface (new ConceptServiceBridge (), "OpenIZConceptService");
            this.AddJavascriptInterface(new SessionServiceBridge(), "OpenIZSessionService");

            this.SetWebViewClient(new AppletWebViewClient());
            this.SetWebChromeClient(new AppletWebChromeClient(this.Context));
		}

		/// <summary>
		/// Go back to previous page
		/// </summary>
		public override void GoBack ()
		{
			this.m_tracer.TraceVerbose ("Applet navigation : back");
			base.GoBack ();

            if (this.m_backQueue.Count > 0)
            {
                this.m_tracer.TraceVerbose("Applet history stack size: {0}", this.m_backQueue.Count);
                this.Asset = this.m_backQueue.Pop();
            }
		}

        /// <summary>
        /// Applet manifest
        /// </summary>
        public AppletManifest Applet {
            get { return this.m_asset?.Manifest; }
            set
            {
                // Find the "index"
				string language = this.Resources.Configuration.Locale.DisplayName;

                var indexValue = value.Assets.Find(o => o.Name == "index" && o.Language == language);
                if (indexValue == null)
                    indexValue = value.Assets.Find(o => o.Name == "index");
                this.Asset = indexValue;
            }
        }

        /// <summary>
        /// Gets or sets the applet
        /// </summary>
        /// <value>The applet.</value>
        public AppletAsset Asset {
			get {
				return this.m_asset;
			}
			set {
				this.m_tracer.TraceVerbose ("Asset Set {0} > {1}", this.m_asset, value);
				if (this.m_asset != null)
					this.m_backQueue.Push (this.m_asset);
				
				this.m_asset = value;
				//var webClient = new AppletWebViewClient();
				//this.SetWebViewClient (webClient);
				//this.SetWebChromeClient (new AppletWebChromeClient (this.Context));
				Application.SynchronizationContext.Post(_=>this.AssetChanged?.Invoke (this, EventArgs.Empty), null); 
			}
		}

		/// <summary>
		/// Execute the specified javascript
		/// </summary>
		/// <param name="javascript">Javascript.</param>
		public string Execute(String javascript)
		{
			this.m_tracer.TraceVerbose ("Execute Javascript: {0}", javascript); 
			if (String.IsNullOrEmpty (javascript))
				throw new ArgumentNullException (nameof (javascript));
			this.LoadUrl ("javascript:" + javascript);
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
			public AppletWebViewClient ()
			{
			}

			
			/// <param name="view">The WebView that is initiating the callback.</param>
			/// <param name="url">The url to be loaded.</param>
			/// <summary>
			/// Give the host application a chance to take over the control when a new
			///  url is about to be loaded in the current WebView.
			/// </summary>
			/// <returns>To be added.</returns>
			public override bool ShouldOverrideUrlLoading (WebView view, string url)
			{
				if (!url.StartsWith(AppletCollection.APPLET_SCHEME) &&
					!url.StartsWith(AppletCollection.ASSET_SCHEME) &&
					!url.StartsWith(AppletCollection.DRAWABLE_SCHEME) &&
					url.Contains(":"))
					return false;

				this.m_tracer.TraceInfo ("Overridding url {0}", url);

				view.LoadUrl (url);
				return true;
			}

            public override void OnLoadResource(WebView view, string url)
            {
                base.OnLoadResource(view, url);
            }

            /// <summary>
            /// Intercept the request
            /// </summary>
            public override WebResourceResponse ShouldInterceptRequest (WebView view, string url)
			{
				// Set scope to applet viewer scope
				if (url.StartsWith (AppletCollection.APPLET_SCHEME) || !url.Contains(":")) {

					this.m_tracer.TraceInfo ("Intercepting request {0}", url);

                    // Language
                    string language = view.Resources.Configuration.Locale.Language;
                    AppletAsset scope = (view as AppletWebView).Asset;
                    var navigateAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(url, scope, language);

                    if (navigateAsset == null)
                        navigateAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("app://openiz.org/applet/org.openiz.applet.core.error/404");
                    
                    try
                    {
                        return this.RenderWebResource(url, navigateAsset);
                    }
                    catch(SecurityException ex)
                    {
                        this.m_tracer.TraceError(ex.ToString());
                        if (ApplicationContext.Current.Principal == null)
                        {
                            // HACK: Can't return a fake 302
                            String redirectUrl = String.Format("<html><head><meta http-equiv=\"refresh\" content=\"0; url={0}?returnUrl={1}\"/></head></html>", AndroidApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().AuthenticationAsset, url);
                            return new WebResourceResponse("text/html", "UTF-8", new MemoryStream(Encoding.UTF8.GetBytes(redirectUrl)));
                        }
                        else
                            navigateAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("app://openiz.org/applet/org.openiz.applet.core.error/403");
                        return this.RenderWebResource(url, navigateAsset);

                    }
                    catch (Exception ex)
                    {
                        this.m_tracer.TraceError(ex.ToString());
                        navigateAsset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset("app://openiz.org/applet/org.openiz.applet.core.error/500");
                        return this.RenderWebResource(url, navigateAsset);
                    }
                    finally
                    {
                        // Switch asset
                        (view as AppletWebView).Asset = navigateAsset;
                    }
                } else if (url.StartsWith (AppletCollection.ASSET_SCHEME)) { 
					String assetPath = url.Substring (AppletCollection.ASSET_SCHEME.Length);
					try {

						// Determine mime
						String ext = MimeTypeMap.GetFileExtensionFromUrl (url), mime = "text/plain";
						switch (ext) {
						case "css":
							mime = "text/css";
							break;
						case "js":
							mime = "text/javascript";
							break;
						case "png":
							mime = "image/png";
							break;
						case "jpeg":
							mime = "image/jpeg";
							break;
						case "woff":
							mime= "application/x-font-woff";
							break;
						case "otf":
							mime = "font/opentype";
							break;
						case "eot":
							mime = "application/vnd.ms-fontobject";
							break;
						case "woff2":
							mime= "application/font-woff2";
							break;
						}

						// Return
						this.m_tracer.TraceInfo("Intercept request for {0} with mime {1}", url, mime);
						return new WebResourceResponse (mime, "UTF-8", view.Context.Assets.Open (assetPath));

					} catch (Exception e) {
						this.m_tracer.TraceError(e.ToString ());
						return null;
					}

				} else if (url.StartsWith (AppletCollection.DRAWABLE_SCHEME)) {
					String assetPath = url.Substring (AppletCollection.DRAWABLE_SCHEME.Length);
					try {

						// Return
						this.m_tracer.TraceInfo ("Intercept request for drawable {0}", url);
						var identifier = view.Resources.GetIdentifier(assetPath, "drawable", view.Context.PackageName);
						if(identifier > 0)
						{
							BitmapDrawable d = view.Resources.GetDrawable(identifier) as BitmapDrawable;
							var ms = new MemoryStream();
							d.Bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
							ms.Seek(0, SeekOrigin.Begin);
							return new WebResourceResponse ("image/png", "UTF-8", ms);
						}
						return null;
					} catch (Exception e) {
						this.m_tracer.TraceError (e.ToString ());
						return null;
					}

				}
				else
					return base.ShouldInterceptRequest(view, url);
			}

            /// <summary>
            /// Render web resource
            /// </summary>
            /// <param name="navigateAsset"></param>
            /// <returns></returns>
            private WebResourceResponse RenderWebResource(String url, AppletAsset navigateAsset)
            {
                // Demand policies
                if(navigateAsset.Manifest.Info.Policies != null)
                    foreach(var policy in navigateAsset.Manifest.Info.Policies)
                        new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, policy).Demand();

                var data = AndroidApplicationContext.Current.LoadedApplets.RenderAssetContent(navigateAsset);
                this.m_tracer.TraceVerbose("Intercept request for {0} ({2} bytes) as: {1}", url, Encoding.UTF8.GetString(data), data.Length);
                var ms = new MemoryStream(data);
                return new WebResourceResponse(navigateAsset.MimeType, "UTF-8", ms);
            }
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
			public AppletWebChromeClient (Context context)
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
			public override bool OnJsAlert (WebView view, string url, string message, JsResult result)
			{
				var alertDialogBuilder = new AlertDialog.Builder (this.m_context) 
				 	.SetMessage (message) 
					.SetCancelable (false) 
					.SetPositiveButton (this.m_context.Resources.GetString(Resource.String.confirm), (sender, args) => { 
						result.Confirm (); 
				}); 
				

				alertDialogBuilder.Create ().Show (); 

				return true; 

			}

			/// <param name="consoleMessage">Object containing details of the console message.</param>
			/// <summary>
			/// Report a JavaScript console message to the host application.
			/// </summary>
			/// <returns>To be added.</returns>
			public override bool OnConsoleMessage (ConsoleMessage consoleMessage)
			{
				var retVal = base.OnConsoleMessage (consoleMessage);
				if (consoleMessage.InvokeMessageLevel() == Webkit.ConsoleMessage.MessageLevel.Error) {

				}

				// Start off verbose
				EventLevel eventLevel = EventLevel.Verbose;
				if (consoleMessage.InvokeMessageLevel () == Webkit.ConsoleMessage.MessageLevel.Error) {
					Toast.MakeText (this.m_context, "This applet reported an error", ToastLength.Long).Show ();
					eventLevel = EventLevel.Error;
				}
				else if(consoleMessage.InvokeMessageLevel () == Webkit.ConsoleMessage.MessageLevel.Warning)
					eventLevel = EventLevel.Warning;
				else if(consoleMessage.InvokeMessageLevel () == Webkit.ConsoleMessage.MessageLevel.Log)
					eventLevel = EventLevel.Informational;

				this.m_tracer.TraceEvent(eventLevel,"[{0}:{1}] {2}", consoleMessage.SourceId(), consoleMessage.LineNumber(), consoleMessage.Message());
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
			public override bool OnJsConfirm (WebView view, string url, string message, JsResult result)
			{
				var alertDialogBuilder = new AlertDialog.Builder (this.m_context) 
					.SetMessage (message) 
					.SetCancelable (false) 
					.SetPositiveButton (this.m_context.Resources.GetString(Resource.String.confirm), (sender, args) => { 
					result.Confirm (); 
				}) 
					.SetNegativeButton (this.m_context.Resources.GetString(Resource.String.cancel), (sender, args) => { 
					result.Cancel (); 
				}); 
			
				alertDialogBuilder.Create ().Show (); 


				return true; 

			}
		}
	}
}

