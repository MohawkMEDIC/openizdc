using System;
using OpenIZ.Mobile.Core.Android.Configuration;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Applets;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using OpenIZ.Mobile.Core.Configuration.Data;
using System.Xml.Serialization;
using System.Security.Cryptography;
using OpenIZ.Mobile.Core.Security;
using System.Reflection;
using System.Diagnostics.Tracing;
using Android.Util;
using Android.Widget;
using Android.Runtime;
using System.Runtime.InteropServices;

namespace OpenIZ.Mobile.Core.Android
{
	/// <summary>
	/// Represents an application context for Xamarin Android
	/// </summary>
	public class AndroidApplicationContext : ApplicationContext
	{

		// Application Secret
		public static readonly byte[] secret = new byte[]{
			0xFF, 0x00, 0x43, 0x23, 0x55, 0x98, 0xA0, 0x20,
			0xC3, 0xE3, 0xE2, 0xA1, 0x42, 0x92, 0x81, 0xE3
		};


		// Applets
		private List<AppletManifest> m_applets = new List<AppletManifest>();

		// The tracer
		private Tracer m_tracer;

		/// <summary>
		/// Fired when no configuration is found
		/// </summary>
		public static event EventHandler NoConfiguration;

		// Configuration manager
		private ConfigurationManager m_configurationManager;

		/// <summary>
		/// Static CTOR bind to global handlers to log errors
		/// </summary>
		/// <value>The current.</value>
		static AndroidApplicationContext() {

			AppDomain.CurrentDomain.UnhandledException += (s,e)=> {
				if(AndroidApplicationContext.Current != null)
				{
					Tracer tracer = Tracer.GetTracer(typeof(AndroidApplicationContext));
					tracer.TraceEvent(EventLevel.Critical, "Uncaught exception: {0}", e.ExceptionObject.ToString());
				}
				else
					Log.Error("AndroindApplicationContext::UncaughtException", e.ExceptionObject.ToString());
				
			};
			AndroidEnvironment.UnhandledExceptionRaiser += (s, e) => {
				if(AndroidApplicationContext.Current != null)
				{
					Tracer tracer = Tracer.GetTracer(typeof(AndroidApplicationContext));
					tracer.TraceEvent(EventLevel.Critical, "Uncaught exception: {0}", e.Exception.ToString());
				}
				else
					Log.Error("AndroindApplicationContext::UncaughtException", e.Exception.ToString());
				e.Handled = true;
			};

		}

		/// <summary>
		/// Gets the current application context
		/// </summary>
		/// <value>The current.</value>
		public static AndroidApplicationContext Current { get { return ApplicationContext.Current as AndroidApplicationContext; } }

		/// <summary>
		/// Starts the application context using in-memory default configuration for the purposes of 
		/// configuring the software
		/// </summary>
		/// <returns><c>true</c>, if temporary was started, <c>false</c> otherwise.</returns>
		public static bool StartTemporary()
		{
			var retVal = new AndroidApplicationContext();
			retVal.m_configurationManager = new ConfigurationManager (ConfigurationManager.GetDefaultConfiguration());
			retVal.Principal = new ClaimsPrincipal (new ClaimsIdentity ("SYSTEM", true, new Claim[] {
				new Claim(ClaimTypes.OpenIzGrantedPolicyClaim, PolicyIdentifiers.AccessClientAdministrativeFunction)
			}));
			ApplicationContext.Current = retVal;
			retVal.m_tracer = Tracer.GetTracer (typeof(AndroidApplicationContext));

			return true;
		}

		/// <summary>
		/// Start the application context
		/// </summary>
		public static bool Start ()
		{

			var retVal = new AndroidApplicationContext ();
			retVal.m_configurationManager = new ConfigurationManager ();

			// Not configured
			if (!retVal.ConfigurationManager.IsConfigured) {
				NoConfiguration?.Invoke (null, EventArgs.Empty);
				return false;
			} else { // load configuration
				try {
					retVal.ConfigurationManager.Load ();
					// Set master application context
					ApplicationContext.Current = retVal;
					retVal.m_tracer = Tracer.GetTracer (typeof(AndroidApplicationContext), retVal.ConfigurationManager.Configuration);

					// Load configured applets
					var configuredApplets = retVal.Configuration.GetSection<AppletConfigurationSection> ().Applets;

					// Load all user-downloaded applets in the data directory
					foreach (var appletInfo in configuredApplets)// Directory.GetFiles(this.m_configuration.GetSection<AppletConfigurationSection>().AppletDirectory)) {
					try {
							retVal.m_tracer.TraceInfo ("Loading applet {0}", appletInfo);
							String appletPath = Path.Combine (retVal.Configuration.GetSection<AppletConfigurationSection> ().AppletDirectory, appletInfo.Id);
							using (var fs = File.OpenRead (appletPath)) {
								AppletManifest manifest = AppletManifest.Load (fs);
								// Is this applet in the allowed applets

								// public key token match?
								if (appletInfo.PublicKeyToken != manifest.Info.PublicKeyToken ||
								    !retVal.VerifyManifest (manifest, appletInfo)) {
									retVal.m_tracer.TraceWarning ("Applet {0} failed validation", appletInfo);
									; // TODO: Raise an error
								}

								retVal.LoadApplet (manifest);
							}
						} catch (Exception e) {
							retVal.m_tracer.TraceError ("Loading applet {0} failed: {1}", appletInfo, e.ToString ());
							throw;
						}

					// Ensure data migration exists
					try {
						// If the DB File doesn't exist we have to clear the migrations
						if (!File.Exists (retVal.Configuration.GetConnectionString (retVal.Configuration.GetSection<DataConfigurationSection> ().MainDataSourceConnectionStringName).Value)) {
							retVal.m_tracer.TraceWarning ("Can't find the OpenIZ database, will re-install all migrations");
							retVal.Configuration.GetSection<DataConfigurationSection> ().MigrationLog.Entry.Clear ();
						}

						DataMigrator migrator = new DataMigrator ();
						migrator.Ensure ();
					} catch (Exception e) {
						retVal.m_tracer.TraceError (e.ToString ());
						throw;
					} finally {
						retVal.ConfigurationManager.Save ();
					}
				} catch (Exception e) {
					retVal.m_tracer?.TraceError (e.ToString ());
					ApplicationContext.Current = null;
				}
				return true;
			}
		}

		/// <summary>
		/// Get applet by id
		/// </summary>
		/// <returns>The applet.</returns>
		/// <param name="id">Identifier.</param>
		public AppletManifest GetApplet (String id)
		{
			return this.m_applets.Find (o => o.Info.Id == id);
		}

		/// <summary>
		/// Register applet
		/// </summary>
		/// <param name="applet">Applet.</param>
		public void LoadApplet (AppletManifest applet)
		{
			this.m_applets.Add (applet);
		}

		/// <summary>
		/// Get the registered applets
		/// </summary>
		/// <value>The registered applets.</value>
		public IEnumerable<AppletManifest> LoadedApplets {
			get { 
				return this.m_applets;
			}
		}

		/// <summary>
		/// Install an applet
		/// </summary>
		public void InstallApplet (AppletPackage package, bool isUpgrade = false)
		{

			this.m_tracer.TraceInfo ("Installing applet {0} (IsUpgrade={1})", package.Meta, isUpgrade);

			// TODO: Verify the package

			// Desearialize an prep for install
			var appletSection = this.Configuration.GetSection<AppletConfigurationSection> ();

			// Copy 
			if (!Directory.Exists (appletSection.AppletDirectory))
				Directory.CreateDirectory (appletSection.AppletDirectory);

			String appletPath = Path.Combine (appletSection.AppletDirectory, package.Meta.Id);
			if (File.Exists (appletPath)) {

				if (!isUpgrade)
					throw new InvalidOperationException ("Duplicate package name");

				// Unload the loaded applet version
				this.m_applets.RemoveAll (o => o.Info.Id == package.Meta.Id);
				appletSection.Applets.RemoveAll (o => o.Id == package.Meta.Id);
			}

			// Save the applet
			XmlSerializer xsz = new XmlSerializer (typeof(AppletManifest));
			// Serialize the data to disk
			using (FileStream fs = File.Create (appletPath)) {
				fs.Write (package.Manifest, 0, package.Manifest.Length);
				fs.Flush ();
			}

			// TODO: Sign this with my private key
			// For now sign with SHA256
			SHA256 sha = SHA256.Create ();
			package.Meta.Hash = sha.ComputeHash (package.Manifest);
			appletSection.Applets.Add (package.Meta);

			if (this.ConfigurationManager.IsConfigured)
				this.ConfigurationManager.Save ();

			using (MemoryStream ms = new MemoryStream (package.Manifest))
				this.LoadApplet (AppletManifest.Load (ms));

		}


		/// <summary>
		/// Verifies the manifest against it's recorded signature
		/// </summary>
		/// <returns><c>true</c>, if manifest was verifyed, <c>false</c> otherwise.</returns>
		/// <param name="manifest">Manifest.</param>
		public bool VerifyManifest (AppletManifest manifest, AppletReference configuredInfo)
		{
			return true;
		}

		/// <summary>
		/// Gets the configuration manager.
		/// </summary>
		/// <value>The configuration manager.</value>
		public ConfigurationManager ConfigurationManager {
			get {
				return this.m_configurationManager;
			}
		}

		#region implemented abstract members of ApplicationContext

		/// <summary>
		/// Get the configuration 
		/// </summary>
		/// <value>The configuration.</value>
		public override OpenIZ.Mobile.Core.Configuration.OpenIZConfiguration Configuration {
			get {
				return this.m_configurationManager.Configuration;
			}
		}


		#endregion
	}
}

