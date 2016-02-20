
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
using OpenIZ.Mobile.Core.Android.Configuration;

namespace OpenIZMobile
{
	[Activity (Label = "OpenIZ Mobile", Theme = "@style/OpenIZ")]
	public class HomeActivity : Activity
	{

		// Home layout
		private LinearLayout m_homeLayout;

		protected override void OnCreate (Bundle savedInstanceState)
		{	
			
			base.OnCreate (savedInstanceState);

			this.ActionBar.SetIcon (Resource.Drawable.logo);
			// Create your application here
			SetContentView(Resource.Layout.Home);


			this.m_homeLayout = FindViewById<LinearLayout> (Resource.Id.home_app_layout);
			// Add apps to the home screen
			this.AddAppletTiles();
		}

		/// <summary>
		/// Add applet tiles
		/// </summary>
		private void AddAppletTiles()
		{

			var tiles = ConfigurationManager.Current.Configuration.Applets;
			string language = this.Resources.Configuration.Locale.DisplayLanguage;

			this.m_homeLayout.RemoveAllViews ();
			foreach(var itm in tiles.GroupBy(o => o.Info.GetGroupName(language)  ?? "Uncategorized", o=>o))
			{
				UITileContainer tileContainer = new UITileContainer (this);
				tileContainer.Title = itm.Key;
				foreach (var app in itm.OrderBy(o=>o.Info.GetName(language))) {
					// Applet tiles
					UIAppletTile appletTile = new UIAppletTile (this, app);

					// TODO: Query configuration to see if the user has this tile enabled
					tileContainer.AddView (appletTile);
				}
				this.m_homeLayout.AddView (tileContainer);
			}

		}
	}
}

