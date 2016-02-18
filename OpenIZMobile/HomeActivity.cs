
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
using System.IO;
using OpenIZMobile.Layout;

namespace OpenIZMobile
{
	[Activity (Label = "OpenIZ Mobile", Icon = "@mipmap/icon")]
	public class HomeActivity : Activity
	{

		// Home layout
		private UIFlowLayout m_homeLayout;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.Home);


			this.m_homeLayout = FindViewById<UIFlowLayout> (Resource.Id.home_app_layout);
			// Add apps to the home screen
			this.AddAppletTiles();
		}

		/// <summary>
		/// Add applet tiles
		/// </summary>
		private void AddAppletTiles()
		{
			// TODO: Configure this
			// Load all built-in applets
			foreach (var itm in Assets.List ("Applets")) {
				AppletManifest manifest = AppletManifest.Load (Assets.Open (String.Format("Applets/{0}", itm)));
				UIAppletTile appletTile = new UIAppletTile (this, manifest);

				// TODO: Query configuration to see if the user has this tile enabled
				this.m_homeLayout.AddView(appletTile);
			}

			// Load all user-downloaded applets
			foreach (var itm in Directory.GetFiles(ConfigurationManager.Current.Configuration.AppletDir)) {
				using (var fs = File.OpenRead (itm)) {
					AppletManifest manifest = AppletManifest.Load (fs);
				}
			}

		}
	}
}

