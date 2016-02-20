
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
using OpenIZ.Mobile.Core.Applets;
using OpenIZ.Mobile.Core.Android.Configuration;
using OpenIZ.Mobile.Core.Android.AppletEngine;

namespace OpenIZMobile
{

	/// <summary>
	/// Applet activity
	/// </summary>
	[Activity (Label = "Applet", Theme = "@style/OpenIZ")]			
	public class AppletActivity : Activity
	{

		private AppletWebView m_webView;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your application here
			var activityId = this.Intent.Extras.Get("appletId");
			// Find the applet
			AppletManifest applet = ConfigurationManager.Current.Configuration.Applets.Find(o=>o.Info.Id.ToString() == activityId.ToString());
			if (applet == null) {
				Toast.MakeText (this.ApplicationContext, this.GetString (Resource.String.err_applet_not_found), ToastLength.Short).Show ();
				this.Finish ();
			}

			this.SetContentView (Resource.Layout.Applet);

			this.m_webView = FindViewById<AppletWebView> (Resource.Id.applet_view);
			this.m_webView.AppletChanged += (o, e) => {

				var view = o as AppletWebView;

				// Set the header and stuff
				this.ActionBar.SetTitle(Resource.String.app_name);
				this.ActionBar.Subtitle = view.Applet.Info.GetName(Resources.Configuration.Locale.DisplayLanguage);

				if (view.Applet.Info.Icon?.StartsWith ("@drawable") == true) {
					int iconId = this.Resources.GetIdentifier (view.Applet.Info.Icon.Substring (10), "drawable", "org.openiz.openiz_mobile");
					if (iconId != 0)
						this.ActionBar.SetIcon (iconId);
					else
						this.ActionBar.SetIcon (Resource.Drawable.cogs);
				} else if (view.Applet.Info.Icon != null)
					;
				else
					this.ActionBar.SetIcon (Resource.Drawable.app_alt);
				
			};
			this.m_webView.Applet = applet;
			//this.m_webView.LoadDataWithBaseURL ("applet:index", "<html><body>Hi!</body></html>", "text/html", "UTF-8", null);
			this.m_webView.NavigateAsset("index"); // Navigate to the index page
		}
	}
}

