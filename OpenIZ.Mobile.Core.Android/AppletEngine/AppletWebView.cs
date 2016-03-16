using System;
using System.Linq;
using Android.Webkit;
using OpenIZ.Mobile.Core.Applets;
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
		public event EventHandler AppletChanged;

		// A web-view
		private AppletManifest m_manifest;

		// Queue
		private Stack<AppletManifest> m_backQueue = new Stack<AppletManifest> ();

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


		}

		/// <summary>
		/// Go back to previous page
		/// </summary>
		public override void GoBack ()
		{
			this.m_tracer.TraceVerbose ("Applet navigation : back");
			base.GoBack ();
			this.m_tracer.TraceVerbose ("Applet history stack size: {0}", this.m_backQueue.Count);
			this.Applet = this.m_backQueue.Pop ();
		}

		/// <summary>
		/// Gets or sets the applet
		/// </summary>
		/// <value>The applet.</value>
		public AppletManifest Applet {
			get {
				return this.m_manifest;
			}
			set {
				this.m_tracer.TraceVerbose ("Applet Set {0} > {1}", this.m_manifest?.Info.Id, value?.Info.Id);
				if (this.m_manifest != null)
					this.m_backQueue.Push (this.m_manifest);
				
				this.m_manifest = value;
				var webClient = new AppletWebViewClient();
				this.SetWebViewClient (webClient);
				this.SetWebChromeClient (new AppletWebChromeClient (this.Context));

				Application.SynchronizationContext.Post(_=>this.AppletChanged?.Invoke (this, EventArgs.Empty), null); 


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
		/// Navigate to asset name
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetName">Asset name.</param>
		public void NavigateAsset(String assetName)
		{
			this.m_tracer.TraceVerbose ("Navigate to {0}", assetName);
			this.ClearCache (true);
			this.LoadUrl ("app://openiz.org/applet/" + this.Applet.Info.Id + "/" + assetName);
		}

		/// <summary>
		/// Applet web view client.
		/// </summary>
		private class AppletWebViewClient : WebViewClient
		{

			// Tracer
			private Tracer m_tracer = Tracer.GetTracer(typeof(AppletWebView));

			// Applet scheme - Accessing asset in another applet
			private const string APPLET_SCHEME = "app://openiz.org/applet/";

			// Applet scheme - Accessing asset in another applet
			private const string ASSET_SCHEME = "app://openiz.org/asset/";

			// Drawable scheme - Accessing drawable
			private const string DRAWABLE_SCHEME = "app://openiz.org/drawable/";

			/// <summary>
			/// Initializes a new instance of the
			/// <see cref="OpenIZ.Mobile.Core.Android.AppletEngine.AppletWebView+AppletWebViewClient"/> class.
			/// </summary>
			/// <param name="applet">Applet.</param>
			public AppletWebViewClient ()
			{
			}

			/// <summary>
			/// Load asset
			/// </summary>
			private byte[] GetAssetContent(AppletAsset asset)
			{
				this.m_tracer.TraceInfo("Get asset content for {0}", asset.Name);

				// Render content
				if (asset.Content is String)
					return Encoding.UTF8.GetBytes(asset.Content as String);
				else if (asset.Content is XElement) {
					XElement xe = asset.Content as XElement;
					xe = xe.FirstNode as XElement;
					if (xe.Name.LocalName != "html") {

						XNamespace xhtml = "http://www.w3.org/1999/xhtml";
							
						// HTML header elements
						XElement[] cssLinks = new XElement[] {
							new XElement(xhtml + "meta", new XAttribute("content", "true"), new XAttribute("name", "HandheldFriendly")), 
							new XElement(xhtml + "meta", new XAttribute("content", "width=640px, initial-scale=0.50, maximum-scale=0.50, minimum-scale=0.50, user-scalable=0"), new XAttribute("name", "viewport")), 
							new XElement (xhtml + "link", new XAttribute ("href", "app://openiz.org/asset/css/bootstrap.css"), new XAttribute ("rel", "stylesheet")),
							new XElement (xhtml + "link", new XAttribute ("href", "app://openiz.org/asset/css/select2.min.css"), new XAttribute ("rel", "stylesheet")),

//							new XElement (xhtml + "link", new XAttribute ("href", "app://openiz.org/asset/css/bootstrap-theme.css"), new XAttribute ("rel", "stylesheet"))
							//new XElement (xhtml + "link", new XAttribute ("href", "app://openiz.org/asset/css/jquery-ui.custom.css"), new XAttribute ("rel", "stylesheet")),
							//new XElement (xhtml + "link", new XAttribute ("href", "app://openiz.org/asset/css/jquery-ui.theme.css"), new XAttribute ("rel", "stylesheet")),
							new XElement(xhtml+"script", new XAttribute("src", "app://openiz.org/asset/js/jquery.js"), new XAttribute("type", "text/javascript"), new XText("// Imported data")),
							new XElement(xhtml+"script", new XAttribute("src", "app://openiz.org/asset/js/angular.min.js"), new XAttribute("type", "text/javascript"), new XText("// Imported data")),
							new XElement(xhtml+"script", new XAttribute("src", "app://openiz.org/asset/js/bootstrap.js"), new XAttribute("type", "text/javascript"), new XText("// Imported data")),
							//new XElement(xhtml+"script", new XAttribute("src", "app://openiz.org/asset/js/jquery.easing.min.js"), new XAttribute("type", "text/javascript"), new XText("// Imported data")),
							//new XElement(xhtml+"script", new XAttribute("src", "app://openiz.org/asset/js/jqBootstrapValidation.js"), new XAttribute("type", "text/javascript"), new XText("// Imported data")),
							//new XElement(xhtml+"script", new XAttribute("src", "app://openiz.org/asset/js/jquery-ui.js"), new XAttribute("type", "text/javascript"), new XText("// Imported data")),
							new XElement(xhtml+"script", new XAttribute("src", "app://openiz.org/asset/js/select2.min.js"), new XAttribute("type", "text/javascript"), new XText("// Imported data")),
							new XElement(xhtml+"script", new XAttribute("src", asset.Name + "-controller"), new XAttribute("type", "text/javascript"), new XText("// Imported data")),
						},
						jsLinks = new XElement[] {
							new XElement(xhtml+"script", new XAttribute("src", "app://openiz.org/asset/js/openiz-applet-helper.js"), new XAttribute("type", "text/javascript"), new XText("// Imported data")),

						};


						// Add default CSS and JavaScript components here
						xe = xe.Name.LocalName == "body" ? xe : new XElement(xhtml+"body", xe);

						var xeScriptComment = xe.Nodes ().OfType<XComment>().SingleOrDefault (o=>o.Value.Trim() == "OpenIZ:Scripts");
						if (xeScriptComment != null)
							xeScriptComment.AddAfterSelf (jsLinks);
						else
							xe.Add (jsLinks);
						xe = new XElement(xhtml+"html",
							new XAttribute("ng-app", asset.Name),
							new XElement(xhtml+"head", cssLinks),
							xe
						);
					}

					using (MemoryStream ms = new MemoryStream ())
					using (XmlWriter xw = XmlWriter.Create (ms, new XmlWriterSettings() { Indent = true })) {
						xe.WriteTo (xw);
						xw.Flush ();
						ms.Flush ();
						return ms.ToArray ();
					}
				}
				else
					return asset.Content as byte[];
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
				if (!url.StartsWith(APPLET_SCHEME) &&
					!url.StartsWith(ASSET_SCHEME) &&
					!url.StartsWith(DRAWABLE_SCHEME) &&
					url.Contains(":"))
					return false;

				this.m_tracer.TraceInfo ("Overridding url {0}", url);

				// Are we navigating away?
				if(url.StartsWith(APPLET_SCHEME))
				{
					string assetPath = url.Substring (APPLET_SCHEME.Length);
					String path = null;
					var assetLink = this.ResolveAppletAsset (assetPath, out path);
					if (assetLink != null)
						(view as AppletWebView).Applet = assetLink;
						
				}
				view.LoadUrl (url);
				return true;
			}

			/// <summary>
			/// Resolve asset
			/// </summary>
			private AppletManifest ResolveAppletAsset(string assetPath, out string path)
			{
				this.m_tracer.TraceVerbose ("Resolving asset path {0}", assetPath);

				String targetApplet = assetPath;
				// Page in the target applet
				if (targetApplet.Contains ("/")) {
					path = targetApplet.Substring (targetApplet.IndexOf ("/") + 1);
					if (String.IsNullOrEmpty (path))
						path = "index";
					targetApplet = targetApplet.Substring (0, targetApplet.IndexOf ("/"));
				} else
					path = "index";

				// Now set scope
				return AndroidApplicationContext.Current.GetApplet (targetApplet);
			}

			/// <summary>
			/// Intercept the request
			/// </summary>
			public override WebResourceResponse ShouldInterceptRequest (WebView view, string url)
			{

				// Set scope to applet viewer scope
				if (url.StartsWith (APPLET_SCHEME) || !url.Contains(":")) {

					this.m_tracer.TraceInfo ("Intercepting request {0}", url);

					AppletManifest scope = (view as AppletWebView).Applet;
					string assetPath = url;
					if(assetPath.StartsWith(APPLET_SCHEME))
					{
						assetPath = assetPath.Substring (APPLET_SCHEME.Length);
						scope = this.ResolveAppletAsset(assetPath, out assetPath);
					}

					// Applet scope not found
					if (scope == null) {
						this.m_tracer.TraceError ("Applet scope {0} not found", url);
						return null;
					}

					if (assetPath.Contains ("?"))
						assetPath = assetPath.Substring (0, assetPath.IndexOf ("?") - 1);

					string language = view.Resources.Configuration.Locale.DisplayName;

					// Get the appropriate asset name
					var asset = scope.Assets.Find (o => o.Name == assetPath && o.Language == language);
					if (asset == null)
						asset = scope.Assets.Find (o => o.Name == assetPath && o.Language == null);
					if (asset == null)
						return null;
					else {

						try {
							var data = this.GetAssetContent (asset);
							this.m_tracer.TraceVerbose("Intercept request for {0} ({2} bytes) as: {1}", url, Encoding.UTF8.GetString (data), data.Length);
							var ms = new MemoryStream (data);
							return new WebResourceResponse (asset.MimeType, "UTF-8", ms);
						} catch (Exception ex) {
							this.m_tracer.TraceError(ex.ToString ());
							return null;

						}
					}

				} else if (url.StartsWith (ASSET_SCHEME)) { 
					String assetPath = url.Substring (ASSET_SCHEME.Length);
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

				} else if (url.StartsWith (DRAWABLE_SCHEME)) {
					String assetPath = url.Substring (DRAWABLE_SCHEME.Length);
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

