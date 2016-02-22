using System;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using OpenIZ.Mobile.Core.Configuration;
using Android.Content.Res;
using OpenIZ.Mobile.Core.Applets;
using OpenIZ.Mobile.Core.Android.Http;
using OpenIZ.Mobile.Core.Android.Diagnostics;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Security.Cryptography;
using OpenIZ.Mobile.Core.Configuration.Data;

namespace OpenIZ.Mobile.Core.Android.Configuration
{
	/// <summary>
	/// Configuration manager for the application
	/// </summary>
	public class ConfigurationManager
	{

		// Registered applets
		private List<AppletManifest> m_applets = new List<AppletManifest> ();

		// Tracer
		private Tracer m_tracer;

		// Configuration
		private OpenIZConfiguration m_configuration;

		// Configuration manager
		private static ConfigurationManager s_configManager;

		// Configuration path
		private readonly String m_configPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "OpenIZ.config");

		/// <summary>
		/// Returns true if OpenIZ is configured
		/// </summary>
		/// <value><c>true</c> if this instance is configured; otherwise, <c>false</c>.</value>
		public bool IsConfigured {
			get {
				return File.Exists (this.m_configPath);
			}
		}

		/// <summary>
		/// Get a bare bones configuration
		/// </summary>
		public OpenIZConfiguration GetDefaultConfiguration ()
		{
			// TODO: Bring up initial settings dialog and utility
			var retVal = new OpenIZConfiguration ();

			// Inital data source
			DataConfigurationSection dataSection = new DataConfigurationSection () {
				MainDataSourceConnectionStringName = "openIzData",
				MessageQueueConnectionStringName = "openIzQueue",
				ConnectionString = new System.Collections.Generic.List<ConnectionString> () {
					new ConnectionString () {
						Name = "openIzData",
						Value = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "OpenIZ.sqlite")
					},
					new ConnectionString () {
						Name = "openIzQueue",
						Value = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "MessageQueue.sqlite")
					},
				}
			};

			// Initial Applet configuration
			AppletConfigurationSection appletSection = new AppletConfigurationSection () {
				AppletDirectory = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "applets"),
				AppletGroupOrder = new System.Collections.Generic.List<string> () {
					"Patient Management",
					"Encounter Management",
					"Stock Management",
					"Administration"
				}
			};

			// Initial applet style
			ApplicationConfigurationSection appSection = new ApplicationConfigurationSection () {
				Style = StyleSchemeType.Dark,
				UserPrefDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "userpref")
			};

			// Security configuration
			SecurityConfigurationSection secSection = new SecurityConfigurationSection () {
			};

			// Rest Client Configuration
			ServiceClientConfigurationSection serviceSection = new ServiceClientConfigurationSection () {
				RestClientType = typeof(RestClient)
			};

			// Trace writer
			#if DEBUG
			DiagnosticsConfigurationSection diagSection = new DiagnosticsConfigurationSection () {
				TraceWriter = new System.Collections.Generic.List<TraceWriterConfiguration> () {
					new TraceWriterConfiguration () { 
						Filter = System.Diagnostics.Tracing.EventLevel.LogAlways,
						InitializationData = "OpenIZ",
						TraceWriter = new LogTraceWriter (System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ")
					},
					new TraceWriterConfiguration() {
						Filter = System.Diagnostics.Tracing.EventLevel.LogAlways,
						InitializationData = "OpenIZ",
						TraceWriter = new FileTraceWriter(System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ")
					}
				}
			};
			#else 
			DiagnosticsConfigurationSection diagSection = new DiagnosticsConfigurationSection () {
				new TraceWriterConfiguration() {
				Filter = System.Diagnostics.Tracing.EventLevel.Error,
				InitializationData = "OpenIZ",
				TraceWriter = new FileTraceWriter(System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ")
				}
			}
			#endif
			retVal.Sections.Add (appletSection);
			retVal.Sections.Add (dataSection);
			retVal.Sections.Add (diagSection);
			retVal.Sections.Add (appSection);
			retVal.Sections.Add (secSection);
			retVal.Sections.Add (serviceSection);

			return retVal;
		}

		/// <summary>
		/// Creates a new instance of the configuration manager with the specified configuration file
		/// </summary>
		private ConfigurationManager ()
		{

			// Configuration exists?
			if (this.IsConfigured)
				using (var fs = File.OpenRead (this.m_configPath)) {
					this.m_configuration = OpenIZConfiguration.Load (fs);
				}
			else {
				this.m_configuration = this.GetDefaultConfiguration ();
			}



			this.m_tracer = Tracer.CreateTracer (this.GetType (), this.m_configuration);

			// Load configured applets
			var configuredApplets = this.m_configuration.GetSection<AppletConfigurationSection> ().Applets;
			// Load all user-downloaded applets in the data directory
			foreach (var appletInfo in configuredApplets)// Directory.GetFiles(this.m_configuration.GetSection<AppletConfigurationSection>().AppletDirectory)) {
				try {
					this.m_tracer.TraceInfo ("Loading applet {0}", appletInfo);
					String appletPath = Path.Combine (this.m_configuration.GetSection<AppletConfigurationSection> ().AppletDirectory, appletInfo.Id);
					using (var fs = File.OpenRead (appletPath)) {
						AppletManifest manifest = AppletManifest.Load (fs);
						// Is this applet in the allowed applets

						// public key token match?
						if (appletInfo.PublicKeyToken != manifest.Info.PublicKeyToken ||
						    !this.VerifyManifest (manifest, appletInfo)) {
							this.m_tracer.TraceWarning ("Applet {0} failed validation", appletInfo);
							; // TODO: Raise an error
						}

						this.LoadApplet (manifest);
					}
				} catch (Exception e) {
					this.m_tracer.TraceError ("Loading applet {0} failed: {1}", appletInfo, e.ToString ());
				}

			// Ensure data migration exists
			try {
				// If the DB File doesn't exist we have to clear the migrations
				if (!File.Exists (this.m_configuration.GetConnectionString (this.m_configuration.GetSection<DataConfigurationSection> ().MainDataSourceConnectionStringName).Value)) {
					this.m_tracer.TraceWarning ("Can't find the OpenIZ database, will re-install all migrations");
					this.m_configuration.GetSection<DataConfigurationSection> ().MigrationLog.Entry.Clear ();
				}

				DataMigrator migrator = new DataMigrator (this.m_configuration);
				migrator.Ensure ();
				if (this.IsConfigured)
					this.Save ();
			} catch (Exception e) {
				this.m_tracer.TraceError (e.ToString ());
				if (this.IsConfigured)
					this.Save ();
				
				throw;
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
			var appletSection = this.m_configuration.GetSection<AppletConfigurationSection> ();
			
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

			using (MemoryStream ms = new MemoryStream (package.Manifest))
				this.LoadApplet (AppletManifest.Load (ms));

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
		[XmlIgnore]
		public IEnumerable<AppletManifest> LoadedApplets {
			get { 
				return this.m_applets;
			}
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
		/// Save the configuration to the default location
		/// </summary>
		public void Save ()
		{
			try {
				this.m_tracer.TraceInfo ("Saving configuration to {0}", this.m_configPath);
				if (!Directory.Exists (Path.GetDirectoryName (this.m_configPath)))
					Directory.CreateDirectory (Path.GetDirectoryName (this.m_configPath));
				
				using (FileStream fs = File.Create (this.m_configPath)) {
					this.m_configuration.Save (fs);
					fs.Flush ();
				}
			} catch (Exception e) {
				this.m_tracer.TraceError (e.ToString ());
			}
		}

		/// <summary>
		/// Unload the configuration
		/// </summary>
		public static void Unload ()
		{
			s_configManager = null;
		}

		/// <summary>
		/// Get the current configuration manager singleton
		/// </summary>
		public static ConfigurationManager Current {
			get {
				if (s_configManager == null)
					s_configManager = new ConfigurationManager ();
				return s_configManager;
			}
		}

		/// <summary>
		/// Get the configuration
		/// </summary>
		public OpenIZConfiguration Configuration {
			get {
				return this.m_configuration;
			}
		}

	}
}

