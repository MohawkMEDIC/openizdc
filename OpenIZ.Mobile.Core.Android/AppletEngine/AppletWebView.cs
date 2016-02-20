using System;
using Android.Webkit;
using OpenIZ.Mobile.Core.Applets;
using System.Xml.Linq;
using System.IO;
using Android.Content;
using Android.App;
using Android.Util;
using System.Text;
using OpenIZ.Mobile.Core.Android.Configuration;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace OpenIZ.Mobile.Core.Android.AppletEngine
{
	/// <summary>
	/// Represents a window which can execute javascript
	/// </summary>
	public class AppletWebView : WebView
	{

		/// <summary>
		/// Occurs when applet changed.
		/// </summary>
		public event EventHandler AppletChanged;

		// A web-view
		private AppletManifest m_manifest;

		/// <summary>
		/// Attaches the applet executor to a web view
		/// </summary>
		public AppletWebView (Context context, IAttributeSet attrs) : base(context, attrs)
		{
			this.Settings.CacheMode = CacheModes.NoCache;
			this.Settings.JavaScriptEnabled = true;
			this.Settings.AllowFileAccess = true;
			this.Settings.AllowFileAccessFromFileURLs = true;
			this.Settings.AllowContentAccess = true;
			this.Settings.AllowUniversalAccessFromFileURLs = true;
			this.Settings.BlockNetworkLoads = true;
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
				this.m_manifest = value;
				var webClient = new AppletWebViewClient();
				this.SetWebViewClient (webClient);
				this.SetWebChromeClient (new AppletWebChromeClient (this.Context));

				this.AppletChanged?.Invoke (this, EventArgs.Empty); 

			}
		}

		/// <summary>
		/// Execute the specified javascript
		/// </summary>
		/// <param name="javascript">Javascript.</param>
		public string Execute(String javascript)
		{
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
			this.ClearCache (true);
			this.LoadUrl ("applet:" + assetName);
		}

		/// <summary>
		/// Applet web view client.
		/// </summary>
		private class AppletWebViewClient : WebViewClient
		{
			// Applet scheme - Accessing asset in another applet
			private const string APPLET_SCHEME = "applet:";

			// Applet scheme - Accessing asset in another applet
			private const string ASSET_SCHEME = "asset:";

			// Drawable scheme - Accessing drawable
			private const string DRAWABLE_SCHEME = "drawable:";

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
							new XElement (xhtml + "link", new XAttribute ("href", "asset:css/bootstrap.css"), new XAttribute ("rel", "stylesheet")),
							new XElement (xhtml + "link", new XAttribute ("href", "asset:css/bootstrap-theme.css"), new XAttribute ("rel", "stylesheet"))
						},
						jsLinks = new XElement[] {
							new XElement(xhtml+"script", new XAttribute("src", "asset:js/bootstrap.js"), new XAttribute("type", "text/javascript")),
							new XElement(xhtml+"script", new XAttribute("src", "asset:js/angular.min.js"), new XAttribute("type", "text/javascript")),
						};


						// Add default CSS and JavaScript components here
						xe = xe.Name.LocalName == "body" ? xe : new XElement(xhtml+"body", xe);
						xe.Add (jsLinks);
						xe = new XElement(xhtml+"html",
							new XElement(xhtml+"head", cssLinks),
							xe
						);
					}

					return Encoding.UTF8.GetBytes(xe.ToString().Replace(" xmlns=\"http://www.w3.org/1999/xhtml\"", ""));
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
					!url.StartsWith(DRAWABLE_SCHEME))
					return false;

				// Are we navigating away?
				if(url.StartsWith(APPLET_SCHEME))
				{
					string assetPath = url.Substring (APPLET_SCHEME.Length);
					String path = null;
					var assetLink = this.ResolveAppletAsset (assetPath, out path);
					if (assetLink != null)
						(view as AppletWebView).Applet = assetLink;
					
					view.LoadUrl ("applet:" + path);
				}
				else
					view.LoadUrl (url);
				return true;
			}

			/// <summary>
			/// Resolve asset
			/// </summary>
			private AppletManifest ResolveAppletAsset(string assetPath, out string path)
			{
				if (assetPath.StartsWith ("/")) {
					String targetApplet = assetPath.Substring (1);
					// Page in the target applet
					if (targetApplet.Contains ("/")) {
						path = targetApplet.Substring (targetApplet.IndexOf ("/") + 1);
						targetApplet = targetApplet.Substring (0, targetApplet.IndexOf ("/"));
					} else
						path = "index";

					// Now set scope
					return ConfigurationManager.Current.Configuration.Applets.Find (o => o.Info.Id == targetApplet);
				}
				path = assetPath;
				return  null;
			}

			/// <summary>
			/// Intercept the request
			/// </summary>
			public override WebResourceResponse ShouldInterceptRequest (WebView view, string url)
			{

				// Set scope to applet viewer scope
				if (url.StartsWith (APPLET_SCHEME)) {
					string assetPath = url.Substring (APPLET_SCHEME.Length);
					AppletManifest scope = this.ResolveAppletAsset(assetPath, out assetPath) ?? (view as AppletWebView).Applet;

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
							Log.Info ("OpenIZ:Applet", "Intercept request for {0} as: {1}", url, Encoding.UTF8.GetString (data));
							var ms = new MemoryStream (data);
							return new WebResourceResponse (asset.MimeType, "UTF-8", ms);
						} catch (Exception ex) {
							Log.Error ("OpenIZ:Applet", ex.ToString ());
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
						}

						// Return
						Log.Info ("OpenIZ:Applet", "Intercept request for {0} with mime {1}", url, mime);
						return new WebResourceResponse (mime, "UTF-8", view.Context.Assets.Open (assetPath));
					} catch (Exception e) {
						Log.Error ("OpenIZ:Applet", e.ToString ());
						return null;
					}

				} else if (url.StartsWith (DRAWABLE_SCHEME)) {
					String assetPath = url.Substring (DRAWABLE_SCHEME.Length);
					try {

						// Return
						Log.Info ("OpenIZ:Applet", "Intercept request for drawable {0}", url);
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
						Log.Error ("OpenIZ:Applet", e.ToString ());
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

