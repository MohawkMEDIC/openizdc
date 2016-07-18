using System;
using System.Linq;
using OpenIZ.Mobile.Core.Android.Configuration;
using System.Collections.Generic;
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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Services;
using System.Security;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Applets;
using OpenIZ.Core.Applets.Model;
using System.Security.Principal;
using A = Android;
namespace OpenIZ.Mobile.Core.Android
{
	/// <summary>
	/// Represents an application context for Xamarin Android
	/// </summary>
	public class AndroidApplicationContext : ApplicationContext
	{

		// The application
		private static readonly OpenIZ.Core.Model.Security.SecurityApplication c_application = new OpenIZ.Core.Model.Security.SecurityApplication() {
			ApplicationSecret = "C5B645B7D30A4E7E81A1C3D8B0E28F4C",
			Key = Guid.Parse("5248ea19-369d-4071-8947-413310872b7e"),
			Name = "org.openiz.openiz_mobile"
		};

		// Applets
		private AppletCollection m_applets = new AppletCollection();

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
		public static bool StartTemporary(A.Content.Context context)
		{
			try
			{
				var retVal = new AndroidApplicationContext();
                retVal.Context = context;
                retVal.SetProgress(context.GetString(Resource.String.startup_setup), 0);

                retVal.m_configurationManager = new ConfigurationManager (ConfigurationManager.GetDefaultConfiguration());
				retVal.Principal = new ClaimsPrincipal (new ClaimsIdentity ("SYSTEM", true, new Claim[] {
					new Claim(ClaimTypes.OpenIzGrantedPolicyClaim, PolicyIdentifiers.AccessClientAdministrativeFunction)
				}));
				ApplicationContext.Current = retVal;
				retVal.m_tracer = Tracer.GetTracer (typeof(AndroidApplicationContext));
                retVal.StartDaemons();
				return true;
			}
			catch(Exception e) {
				Log.Error ("OpenIZ FATAL", e.ToString ());
				return false;
			}
		}

		/// <summary>
		/// Start the application context
		/// </summary>
		public static bool Start (A.Content.Context context)
		{

			var retVal = new AndroidApplicationContext ();
            retVal.Context = context;
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
                    
                    retVal.SetProgress(context.GetString(Resource.String.startup_configuration  ), 0.2f);
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
                        retVal.SetProgress(context.GetString(Resource.String.startup_data), 0.6f);


                        DataMigrator migrator = new DataMigrator ();
						migrator.Ensure ();

						// Set the entity source
						EntitySource.Current = new EntitySource(retVal.GetService<IEntitySourceProvider>());

					} catch (Exception e) {
						retVal.m_tracer.TraceError (e.ToString ());
						throw;
					} finally {
						retVal.ConfigurationManager.Save ();
					}

                    // Start daemons
                    retVal.StartDaemons();
				} catch (Exception e) {
					retVal.m_tracer?.TraceError (e.ToString ());
					ApplicationContext.Current = null;
                    throw;
				}
				return true;
			}
		}

        /// <summary>
        /// Sets the current principal
        /// </summary>
        internal void SetPrincipal(IPrincipal p)
        {
            if (p != null && !p.Identity.IsAuthenticated)
                throw new InvalidOperationException("Cannot set principal to unauthenticated identity");
            else
                this.Principal = p;
        }

        /// <summary>
        /// Get applet by id
        /// </summary>
        /// <returns>The applet.</returns>
        /// <param name="id">Identifier.</param>
        public AppletManifest GetApplet (String id)
		{
			return this.m_applets.FirstOrDefault (o => o.Info.Id == id);
		}

		/// <summary>
		/// Register applet
		/// </summary>
		/// <param name="applet">Applet.</param>
		public void LoadApplet (AppletManifest applet)
		{
            if (applet.Info.Id == (this.Configuration.GetSection<AppletConfigurationSection>().StartupAsset ?? "org.openiz.core"))
                this.m_applets.DefaultApplet = applet;
			this.m_applets.Add (applet);
		}

		/// <summary>
		/// Get the registered applets
		/// </summary>
		/// <value>The registered applets.</value>
		public AppletCollection LoadedApplets {
			get { 
				return this.m_applets;
			}
		}

		/// <summary>
		/// Install an applet
		/// </summary>
		public void InstallApplet (AppletPackage package, bool isUpgrade = false)
		{

            // Desearialize an prep for install
            var appletSection = this.Configuration.GetSection<AppletConfigurationSection>();
            String appletPath = Path.Combine(appletSection.AppletDirectory, package.Meta.Id);

            try
            {
                this.m_tracer.TraceInfo("Installing applet {0} (IsUpgrade={1})", package.Meta, isUpgrade);

                this.SetProgress(package.Meta.GetName("en"), 0.0f);
                // TODO: Verify the package
                 
              
                // Copy 
                if (!Directory.Exists(appletSection.AppletDirectory))
                    Directory.CreateDirectory(appletSection.AppletDirectory);

                if (File.Exists(appletPath))
                {

                    if (!isUpgrade)
                        throw new InvalidOperationException("Duplicate package name");

                    // Unload the loaded applet version
                    var existingApplet = this.m_applets.FirstOrDefault(o => o.Info.Id == package.Meta.Id);
                    if (existingApplet != null)
                        this.m_applets.Remove(existingApplet);
                    appletSection.Applets.RemoveAll(o => o.Id == package.Meta.Id);
                    
                }

                // Save the applet
                XmlSerializer xsz = new XmlSerializer(typeof(AppletManifest));

                AppletManifest mfst;

                // Install Database stuff
                using (MemoryStream ms = new MemoryStream(package.Manifest))
                {
                    mfst = xsz.Deserialize(ms) as AppletManifest;
                    // Migrate data.
                    if (mfst.DataSetup != null)
                        foreach(var itm in mfst.DataSetup.Action)
                        {
                            Type idpType = typeof(IDataPersistenceService<>);
                            idpType = idpType.MakeGenericType(new Type[] { itm.Element.GetType() });
                            var svc = ApplicationContext.Current.GetService(idpType);
                            idpType.GetMethod(itm.ActionName).Invoke(svc, new object[] { itm.Element });
                        }

                    // Now export all the binary files out
                    var assetDirectory = Path.Combine(appletSection.AppletDirectory, "assets", mfst.Info.Id);
                    if (!Directory.Exists(assetDirectory))
                        Directory.CreateDirectory(assetDirectory);
                    for(int i = 0; i < mfst.Assets.Count; i++)
                    {
                        var itm = mfst.Assets[i];
                        var itmPath = Path.Combine(assetDirectory, itm.Name);
                        this.SetProgress(package.Meta.GetName("en"), 0.1f + (float)(0.8 * (float)i / mfst.Assets.Count));

                        // Get dir name and create
                        if (!Directory.Exists(Path.GetDirectoryName(itmPath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(itmPath));

                        // Extract content
                        if (itm.Content is byte[])
                        {
                            File.WriteAllBytes(itmPath, itm.Content as byte[]);
                            itm.Content = null;
                        }
                        else if(itm.Content is String)
                        {
                            File.WriteAllText(itmPath, itm.Content as String);
                            itm.Content = null;
                        }
                      }

                    // Serialize the data to disk
                    using (FileStream fs = File.Create(appletPath))
                        xsz.Serialize(fs, mfst);
                }

                // TODO: Sign this with my private key
                // For now sign with SHA256
                SHA256 sha = SHA256.Create();
                package.Meta.Hash = sha.ComputeHash(File.ReadAllBytes(appletPath));
                appletSection.Applets.Add(package.Meta.AsReference());

                this.SetProgress(package.Meta.GetName("en"), 0.98f);

                if (this.ConfigurationManager.IsConfigured)
                    this.ConfigurationManager.Save();

                mfst.Initialize();

                this.LoadApplet(mfst);
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error installing applet {0} : {1}", package.Meta.ToString(), e);

                // Remove
                if (File.Exists(appletPath))
                    File.Delete(appletPath);

                throw;
            }
		}


		/// <summary>
		/// Verifies the manifest against it's recorded signature
		/// </summary>
		/// <returns><c>true</c>, if manifest was verifyed, <c>false</c> otherwise.</returns>
		/// <param name="manifest">Manifest.</param>
		public bool VerifyManifest (AppletManifest manifest, AppletName configuredInfo)
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

		/// <summary>
		/// Explicitly authenticate the specified user as the domain context
		/// </summary>
		public void Authenticate(String userName, String password)
		{
			var identityService = this.GetService<IIdentityProviderService>();
			this.Principal = identityService.Authenticate(userName, password);
			if(this.Principal == null)
				throw new SecurityException("err_login_invalidusername");
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

		/// <summary>
		/// Gets the application information for the currently running application.
		/// </summary>
		/// <value>The application.</value>
		public override OpenIZ.Core.Model.Security.SecurityApplication Application {
			get {

				return c_application;
			}
		}
        
        /// <summary>
        /// Gets the current context
        /// </summary>
        public A.Content.Context Context { get; set;  }

        /// <summary>
        /// Gets the device information for the currently running device
        /// </summary>
        /// <value>The device.</value>
        public override OpenIZ.Core.Model.Security.SecurityDevice Device {
			get {
				// TODO: Load this from configuration
				return new OpenIZ.Core.Model.Security.SecurityDevice () {
					Name = this.Configuration.GetSection<SecurityConfigurationSection>().DeviceName,
					DeviceSecret = this.Configuration.GetSection<SecurityConfigurationSection>().DeviceSecret
				};
			}
		}
		#endregion
	}
}

