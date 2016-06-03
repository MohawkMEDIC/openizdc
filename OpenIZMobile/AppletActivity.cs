
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
using OpenIZ.Mobile.Core.Android.Configuration;
using OpenIZ.Mobile.Core.Android.AppletEngine;
using OpenIZ.Mobile.Core.Android;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Applets;

namespace OpenIZMobile
{

	/// <summary>
	/// Applet activity
	/// </summary>
	[Activity (Label = "Applet", Theme = "@style/OpenIZ", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize )]			
	public class AppletActivity : Activity
	{


        // Home layout
        private AppletWebView m_webView;

        /// <summary>
		/// Called when the activity has detected the user's press of the back
		///  key.
		/// </summary>
		public override void OnBackPressed()
        {
            if (this.m_webView.CanGoBack())
                this.m_webView.GoBack();
            else
                base.OnBackPressed();
        }

        /// <param name="newConfig">The new device configuration.</param>
        /// <summary>
        /// Configuration changed
        /// </summary>
        public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
		}

		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			this.SetContentView (Resource.Layout.Applet);
			this.m_webView = FindViewById<AppletWebView> (Resource.Id.applet_view);

			var assetLink = this.Intent.Extras.Get ("assetLink").ToString();
			// Find the applet
			AppletAsset asset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(
                assetLink, language: this.Resources.Configuration.Locale.Language);
			if (asset == null) {
				Toast.MakeText (this.ApplicationContext, this.GetString (Resource.String.err_applet_not_found), ToastLength.Short).Show ();
				this.Finish ();
				return;
			}

			// Applet has changed
			this.m_webView.AssetChanged += (o, e) => {


				var view = o as AppletWebView;

				// Set the header and stuff
				this.ActionBar.SetTitle(Resource.String.app_name);
                this.ActionBar.Subtitle = (view.Asset.Content as AppletAssetHtml)?.GetTitle(Resources.Configuration.Locale.Language) ?? view.Applet.Info.GetName(Resources.Configuration.Locale.Language);


                if (view.Applet.Info.Icon?.StartsWith (AppletCollection.DRAWABLE_SCHEME) == true) {
					int iconId = this.Resources.GetIdentifier (view.Applet.Info.Icon.Substring (AppletCollection.DRAWABLE_SCHEME.Length), "drawable", "org.openiz.openiz_mobile");
					if (iconId != 0)
						this.ActionBar.SetIcon (iconId);
					else
						this.ActionBar.SetIcon (Resource.Drawable.cogs);
				} else if (view.Applet.Info.Icon != null)
					;
				else
					this.ActionBar.SetIcon (Resource.Drawable.app_alt);

			};
            //this.m_webView.Asset = asset;
            this.m_webView.LoadUrl(assetLink);
			//this.m_webView.LoadDataWithBaseURL ("applet:index", "<html><body>Hi!</body></html>", "text/html", "UTF-8", null);


		}

		/// <summary>
		/// Back
		/// </summary>
		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			// Continue to?
			if (this.Intent.Extras.Get ("continueTo") != null) {
				Type activity = Type.GetType (this.Intent.Extras.Get ("continueTo").ToString());
				Intent navigate = new Intent (this, activity);
				this.StartActivity (navigate);
			}

		}
	}
}

