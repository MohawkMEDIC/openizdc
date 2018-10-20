/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
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
using OpenIZ.Mobile.Core.Android.Configuration;
using OpenIZ.Mobile.Core.Android.AppletEngine;
using OpenIZ.Mobile.Core.Android;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Applets;
using System.Xml;
using System.Threading.Tasks;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Services;
using Android.Content.PM;
using System.Threading;

namespace OpenIZMobile
{

	/// <summary>
	/// Applet activity
	/// </summary>
	[Activity (Label = "Applet", Theme = "@style/OpenIZ", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize )]			
	public class AppletActivity : OpenIZApplicationActivity
	{


        // Home layout
        private AppletWebView m_webView;
        private ProgressBar m_progressBar;
        private TextView m_textView;
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletActivity));


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

            (AndroidApplicationContext.Current as AndroidApplicationContext).CurrentActivity = this;

			this.SetContentView (Resource.Layout.Applet);
			this.m_webView = FindViewById<AppletWebView> (Resource.Id.applet_view);
            //this.m_webView.Asset = asset;
            var assetLink = this.Intent.Extras.Get("assetLink").ToString();
            this.m_tracer.TraceInfo("Navigating to {0}", assetLink);
            if(!String.IsNullOrEmpty(assetLink))
                this.m_webView.LoadUrl(assetLink);

            // Progress bar
            this.m_progressBar = new ProgressBar(this, null, Android.Resource.Attribute.ProgressBarStyleHorizontal);
            this.m_textView = new TextView(this, null);
            this.m_textView.SetText(Resource.String.loading);
            this.m_progressBar.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 24);
            this.m_textView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            var decorView = this.Window.DecorView as FrameLayout;
            decorView.AddView(this.m_progressBar);
            decorView.AddView(this.m_textView);

            // Find the applet
            //AppletAsset asset = AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(
            //             assetLink, language: this.Resources.Configuration.Locale.Language);
            //if (asset == null) {
            //             UserInterfaceUtils.ShowMessage(this, (o, e) => { this.Finish(); }, String.Format("FATAL: {0} not found (installed: {1})", assetLink, AndroidApplicationContext.Current.LoadedApplets.Count));

            //}

           

            // Progress has changed
            AndroidApplicationContext.ProgressChanged += (o, e) =>
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(e.ProgressText) && e.Progress > 0 && e.Progress < 1.0f)
                        {
                            this.m_textView.Visibility = ViewStates.Visible;
                            this.m_progressBar.Progress = (int)(this.m_progressBar.Max * e.Progress);
                            this.m_textView.Text = String.Format("{0} {1}", e.ProgressText, e.Progress > 0 ? String.Format("({0:0%})", e.Progress) : null);
                        }
                        else
                            this.m_textView.Visibility = ViewStates.Invisible;
                    }
                    catch { }
                });
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

