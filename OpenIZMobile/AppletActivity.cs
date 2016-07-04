
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
using System.Xml;

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
        private ProgressBar m_progressBar;
        private TextView m_textView;

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

            // Progress bar
            this.m_progressBar = new ProgressBar(this, null, Android.Resource.Attribute.ProgressBarStyleHorizontal);
            this.m_textView = new TextView(this, null);
            this.m_textView.SetText(Resource.String.loading);
            this.m_progressBar.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 24);
            this.m_textView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            var decorView = this.Window.DecorView as FrameLayout;
            decorView.AddView(this.m_progressBar);
            decorView.AddView(this.m_textView);

            var assetLink = this.Intent.Extras.Get ("assetLink").ToString();
			// Find the applet
			AppletAsset asset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(
                assetLink, language: this.Resources.Configuration.Locale.Language);
			if (asset == null) {
                UserInterfaceUtils.ShowMessage(this, (o, e) => { this.Finish(); }, String.Format("FATAL: {0} not found (installed: {1})", assetLink, AndroidApplicationContext.Current.LoadedApplets.Count));
                
			}

            // Progress has changed
            this.m_webView.ProgressChanged += (o, e) =>
            {
                try
                {
                    this.m_textView.Visibility = this.m_progressBar.Visibility = this.m_webView.Progress == 0 || this.m_webView.Progress == 100 ? ViewStates.Gone : ViewStates.Visible;
                    this.m_progressBar.Progress = (this.m_progressBar.Max) / 100 * this.m_webView.Progress;
                }
                catch { }
            };

            // Set view 
            EventHandler observer = null;
            observer = (o, e) =>
            {
                try
                {
                    View contentView = decorView.FindViewById(Android.Resource.Id.Content);
                    this.m_progressBar.SetY(contentView.GetY() + contentView.Height - 15);
                    this.m_textView.SetY(contentView.GetY() + contentView.Height - this.m_textView.MeasuredHeight - 15);

                    this.m_progressBar.ViewTreeObserver.GlobalLayout -= observer;
                }
                catch { }
            };
            this.m_progressBar.ViewTreeObserver.GlobalLayout += observer;
            // Applet has changed
            this.m_webView.AssetChanged += (o, e) => {
                
				var view = o as AppletWebView;

				// Set the header and stuff
				this.SetTitle(Resource.String.app_name);
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

