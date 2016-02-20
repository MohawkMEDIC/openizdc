
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Android.Configuration;
using System.IO;
using OpenIZ.Mobile.Core.Applets;
using System.Threading.Tasks;
using Android.Util;

namespace OpenIZMobile
{
	[Activity (Label = "OpenIZ", Theme="@style/OpenIZ.Splash", MainLauncher = true, Icon = "@mipmap/icon", NoHistory = true)]			
	public class SplashActivity : Activity
	{
		/// <summary>
		/// Create the activity
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			this.SetContentView (Resource.Layout.Splash);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			this.FindViewById<TextView> (Resource.Id.txt_splash_version).Text = String.Format ("V {0} ({1})",
				typeof(SplashActivity).Assembly.GetName ().Version,
				typeof(SplashActivity).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute> ().InformationalVersion
			);

			Task startupWork = new Task(() => {
				
				this.DoConfigure();
			});

			startupWork.ContinueWith(t => {
				StartActivity(new Intent(this.ApplicationContext, typeof(LoginActivity)));
			}, TaskScheduler.FromCurrentSynchronizationContext());

			startupWork.Start();

		}

		/// <summary>
		/// Startup is complete
		/// </summary>
		private void DoConfigure()
		{
			ConfigurationManager.Current.Unload();
			// Now we load the configuration
			var configuration = ConfigurationManager.Current;
			if (!File.Exists (configuration.Configuration.AppDataFile))
				; // TODO: Configuration screen here
			// Load applets
			foreach (var itm in Assets.List ("Applets")) {
				try
				{
					Log.Info("Configuration", "Attempting to load applet {0}", itm);
					AppletManifest manifest = AppletManifest.Load (Assets.Open (String.Format("Applets/{0}", itm)));
					configuration.Configuration.Applets.Add (manifest);
				}
				catch(Exception e) {
					Log.Error ("Configuration", "Applet load failed: {0}", e.Message);
				}
			}

		}

	}
}

