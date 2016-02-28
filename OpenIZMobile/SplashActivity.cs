
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
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Exceptions;
using System.Threading;
using OpenIZ.Mobile.Core.Android;

namespace OpenIZMobile
{
	[Activity (Label = "OpenIZ", Theme = "@style/OpenIZ.Splash", MainLauncher = true, Icon = "@mipmap/icon", NoHistory = true)]			
	public class SplashActivity : Activity
	{
		// Tracer
		private Tracer m_tracer;

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
			OpenIZ.Mobile.Core.ApplicationContext.Current = null;

			this.FindViewById<TextView> (Resource.Id.txt_splash_version).Text = String.Format ("V {0} ({1})",
				typeof(SplashActivity).Assembly.GetName ().Version,
				typeof(SplashActivity).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute> ().InformationalVersion
			);

			CancellationTokenSource ctSource = new CancellationTokenSource ();
			CancellationToken ct = ctSource.Token;

			Task startupWork = new Task (() => {
				Task.Delay (1000);
				if (!this.DoConfigure ())
					ctSource.Cancel ();
			}, ct);

			startupWork.ContinueWith (t => {
				if (!ct.IsCancellationRequested)
					StartActivity (new Intent (this.ApplicationContext, typeof(LoginActivity)));
			}, TaskScheduler.FromCurrentSynchronizationContext ());

			startupWork.Start ();

		}

		/// <summary>
		/// Startup is complete
		/// </summary>
		private bool DoConfigure ()
		{

			try {


				if (!AndroidApplicationContext.Start ()) {

					CancellationTokenSource ctSource = new CancellationTokenSource();
					CancellationToken ct = ctSource.Token;

					Task notifyUserWork = new Task (() => {

						this.RunOnUiThread (() => this.FindViewById<TextView> (Resource.Id.txt_splash_info).Text = GetString (Resource.String.needs_setup));
						Task.Delay (2000);

						try {
							if (AndroidApplicationContext.StartTemporary ()) {
								this.m_tracer = Tracer.GetTracer(typeof(SplashActivity));

								try {
									AppletManifest manifest = AppletManifest.Load (Assets.Open ("Applets/SettingsApplet.xml"));
									// Write data to assets directory
									AndroidApplicationContext.Current.InstallApplet (manifest.CreatePackage (), true);
								} catch (Exception e) {
									this.m_tracer.TraceError (e.ToString ());
								}

							}
						} catch (Exception e) {
							this.m_tracer.TraceError (e.ToString ());
							ctSource.Cancel();
							this.ShowException (e);
						}
					}, ct);

					// Now show the configuration screen.
					notifyUserWork.ContinueWith (t => {
						if(!ct.IsCancellationRequested)
						{
							Intent viewIntent = new Intent (this, typeof(AppletActivity));
							viewIntent.PutExtra ("appletId", "org.openiz.applets.core.settings");
							viewIntent.PutExtra("continueTo", typeof(SplashActivity).AssemblyQualifiedName);
							this.StartActivity (viewIntent);

						}
					}, TaskScheduler.Current);

					notifyUserWork.Start ();
					return false;
				} else {

					RunOnUiThread (() => { 
						this.FindViewById<TextView> (Resource.Id.txt_splash_info).Text = GetString (Resource.String.installing_applets);
					});

					// Upgrade applets from our app manifest
					foreach (var itm in Assets.List ("Applets")) {
						try {
							AppletManifest manifest = AppletManifest.Load (Assets.Open (String.Format ("Applets/{0}", itm)));
							// Write data to assets directory
							AndroidApplicationContext.Current.InstallApplet (manifest.CreatePackage (), true);
						} catch (Exception e) {
							this.m_tracer.TraceError (e.ToString ());
						}
					}
				}
				return true;
			} catch (Exception e) {

				this.ShowException (e);
				return false;
			}
		}


		/// <summary>
		/// Shows an exception message box
		/// </summary>
		/// <param name="e">E.</param>
		private void ShowException (Exception e)
		{
			while (e is TargetInvocationException)
				e = e.InnerException;
			this.RunOnUiThread (() => {
				var alertDialogBuilder = new AlertDialog.Builder (this) 
					.SetMessage (String.Format ("{0} : {1}", Resources.GetString (Resource.String.err_startup), e is TargetInvocationException ? e.InnerException.Message : e.Message)) 
					.SetCancelable (false) 
					.SetPositiveButton (Resource.String.confirm, (sender, args) => { 
					this.Finish ();
				}); 

				alertDialogBuilder.Show ();
			});
		}
	}
}

