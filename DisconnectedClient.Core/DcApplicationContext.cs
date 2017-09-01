/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2017-4-3
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services.Impl;
using OpenIZ.Mobile.Core.Data;
using OpenIZ.Mobile.Core.Configuration.Data;
using OpenIZ.Core.Model.EntityLoader;
using System.Diagnostics;
using System.Security.Principal;
using OpenIZ.Core.Applets;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Diagnostics.Tracing;
using OpenIZ.Mobile.Core;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Xamarin;
using OpenIZ.Mobile.Core.Xamarin.Configuration;
using System.Xml.Linq;
using System.Reflection;
using System.Globalization;
using OpenIZ.Protocol.Xml;
using OpenIZ.Protocol.Xml.Model;
using System.IO.Compression;
using OpenIZ.Mobile.Core.Services;
using System.Xml.Serialization;
using System.Security.Cryptography;
using OpenIZ.Core.Applets.Services;
using DisconnectedClient.Core.Resources;
using OpenIZ.Mobile.Core.Xamarin.Data;

namespace DisconnectedClient.Core
{
    /// <summary>
    /// Test application context
    /// </summary>
    public class DcApplicationContext : XamarinApplicationContext
    {

		// Dialog provider
		private IDialogProvider m_dialogProvider = null;

        // XSD OpenIZ
        private static readonly XNamespace xs_openiz = "http://openiz.org/applet";

        // The application
        private static readonly OpenIZ.Core.Model.Security.SecurityApplication c_application = new OpenIZ.Core.Model.Security.SecurityApplication()
        {
            ApplicationSecret = "FE78825ADB56401380DBB406411221FD",
            Key = Guid.Parse("B7ECA9F3-805E-4BE9-A5C7-30E6E495939A"),
            Name = "org.openiz.disconnected_client.win32"
        };

        // Applet bas directory
        private Dictionary<AppletManifest, String> m_appletBaseDir = new Dictionary<AppletManifest, string>();

        private DcConfigurationManager m_configurationManager;

        /// <summary>
        /// Configuration manager
        /// </summary>
        public override IConfigurationManager ConfigurationManager
        {
            get
            {
                return this.m_configurationManager;
            }
        }

        /// <summary>
        /// Show toast
        /// </summary>
        public override void ShowToast(string subject)
        {
            
        }

        /// <summary>
        /// Get the configuration
        /// </summary>
        public override OpenIZConfiguration Configuration
        {
            get
            {
                return this.ConfigurationManager.Configuration;
            }
        }

        /// <summary>
        /// Get the application
        /// </summary>
        public override SecurityApplication Application
        {
            get
            {
                return c_application;
            }
        }

        /// <summary>
        /// Static CTOR bind to global handlers to log errors
        /// </summary>
        /// <value>The current.</value>
        static DcApplicationContext()
        {

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (XamarinApplicationContext.Current != null)
                {
                    Tracer tracer = Tracer.GetTracer(typeof(XamarinApplicationContext));
                    tracer.TraceEvent(EventLevel.Critical, "Uncaught exception: {0}", e.ExceptionObject.ToString());
                }
            };


        }

		/// <summary>
		/// Initializes a new instance of the <see cref="DisconnectedClient.DcApplicationContext"/> class.
		/// </summary>
		/// <param name="dialogProvider">Dialog provider.</param>
		public DcApplicationContext (IDialogProvider dialogProvider)
		{
			this.m_dialogProvider = dialogProvider;
		}

        /// <summary>
		/// Starts the application context using in-memory default configuration for the purposes of 
		/// configuring the software
		/// </summary>
		/// <returns><c>true</c>, if temporary was started, <c>false</c> otherwise.</returns>
		public static bool StartTemporary(IDialogProvider dialogProvider)
        {
            try
            {
                var retVal = new DcApplicationContext(dialogProvider);
                retVal.SetProgress("Run setup", 0);

                retVal.m_configurationManager = new DcConfigurationManager(DcConfigurationManager.GetDefaultConfiguration());

                ApplicationContext.Current = retVal;
                ApplicationServiceContext.Current = ApplicationContext.Current;
                ApplicationServiceContext.HostType = OpenIZHostType.OtherClient;

                retVal.m_tracer = Tracer.GetTracer(typeof(DcApplicationContext));
                retVal.ThreadDefaultPrincipal = AuthenticationContext.SystemPrincipal;

                var appletService = retVal.GetService<IAppletManagerService>();

                retVal.SetProgress("Loading configuration", 0.2f);
                // Load all user-downloaded applets in the data directory
                foreach (var appPath in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Applets")))
                    try
                    {

                        retVal.m_tracer.TraceInfo("Installing applet {0}", appPath);
                        using (var fs = File.OpenRead(appPath))
                        {
                            AppletPackage package = AppletPackage.Load(fs);
                            appletService.Install(package, true);
                        }
                    }
                    catch (Exception e)
                    {
                        retVal.m_tracer.TraceError("Loading applet {0} failed: {1}", appPath, e.ToString());
                        throw;
                    }

                retVal.Start();
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("OpenIZ FATAL: {0}", e.ToString());
                return false;
            }
        }


        /// <summary>
        /// Start the application context
        /// </summary>
        public static bool StartContext(IDialogProvider dialogProvider)
        {

            var retVal = new DcApplicationContext(dialogProvider);
            retVal.m_configurationManager = new DcConfigurationManager();
            // Not configured
            if (!retVal.ConfigurationManager.IsConfigured)
            {
                return false;
            }
            else
            { 
                // load configuration
                try
                {
                    // Set master application context
                    ApplicationContext.Current = retVal;

                    try
                    {
                        retVal.ConfigurationManager.Load();
                        retVal.ConfigurationManager.Backup();
                    }
                    catch
                    {
                        if (retVal.ConfigurationManager.HasBackup() && retVal.Confirm(Strings.err_configuration_invalid_restore_prompt))
                        {
                            retVal.ConfigurationManager.Restore();
                            retVal.ConfigurationManager.Load();
                        }
                        else
                            throw;
                    }
                    retVal.AddServiceProvider(typeof(XamarinBackupService));

                    // Is there a backup, and if so, does the user want to restore from that backup?
                    var backupSvc = retVal.GetService<IBackupService>();
                    if (backupSvc.HasBackup(BackupMedia.Public) &&
                        retVal.Configuration.GetAppSetting("ignore.restore") == null &&
                        retVal.Confirm(Strings.locale_confirm_restore))
                    {
                        backupSvc.Restore(BackupMedia.Public);
                    }

                    // Ignore restoration
                    retVal.Configuration.GetSection<ApplicationConfigurationSection>().AppSettings.Add(new AppSettingKeyValuePair()
                    {
                        Key = "ignore.restore",
                        Value = "true"
                    });

                    retVal.m_tracer = Tracer.GetTracer(typeof(DcApplicationContext), retVal.ConfigurationManager.Configuration);

                    retVal.SetProgress("Loading configuration", 0.2f);
                    // Load all user-downloaded applets in the data directory
                    var configuredApplets = retVal.Configuration.GetSection<AppletConfigurationSection>().Applets;

                    var appletService = retVal.GetService<IAppletManagerService>();

                    foreach (var appletInfo in configuredApplets)// Directory.GetFiles(this.m_configuration.GetSection<AppletConfigurationSection>().AppletDirectory)) {
                        try
                        {
                            retVal.m_tracer.TraceInfo("Loading applet {0}", appletInfo);
                            String appletPath = Path.Combine(retVal.Configuration.GetSection<AppletConfigurationSection>().AppletDirectory, appletInfo.Id);
                            using (var fs = File.OpenRead(appletPath))
                            {
                                AppletManifest manifest = AppletManifest.Load(fs);
                                // Is this applet in the allowed applets

                                // public key token match?
                                if (appletInfo.PublicKeyToken != manifest.Info.PublicKeyToken)
                                {
                                    retVal.m_tracer.TraceWarning("Applet {0} failed validation", appletInfo);
                                    ; // TODO: Raise an error
                                }

                                appletService.LoadApplet(manifest);
                            }
                        }
                        catch (AppDomainUnloadedException) { throw; }
                        catch (Exception e)
                        {
                            if (retVal.Confirm(String.Format(Strings.err_applet_corrupt_reinstall, appletInfo.Id)))
                            {
                                String appletPath = Path.Combine(retVal.Configuration.GetSection<AppletConfigurationSection>().AppletDirectory, appletInfo.Id);
                                if (File.Exists(appletPath))
                                    File.Delete(appletPath);
                            }
                            else
                            {
                                retVal.m_tracer.TraceError("Loading applet {0} failed: {1}", appletInfo, e.ToString());
                                throw;
                            }
                        }


                    // Ensure data migration exists
                    try
                    {
                        // If the DB File doesn't exist we have to clear the migrations
                        if (!File.Exists(retVal.Configuration.GetConnectionString(retVal.Configuration.GetSection<DataConfigurationSection>().MainDataSourceConnectionStringName).Value))
                        {
                            retVal.m_tracer.TraceWarning("Can't find the OpenIZ database, will re-install all migrations");
                            retVal.Configuration.GetSection<DataConfigurationSection>().MigrationLog.Entry.Clear();
                        }
                        retVal.SetProgress("Migrating databases", 0.6f);

                        DataMigrator migrator = new DataMigrator();
                        migrator.Ensure();

                        // Set the entity source
                        EntitySource.Current = new EntitySource(retVal.GetService<IEntitySourceProvider>());

                        // Prepare clinical protocols
                        //retVal.GetService<ICarePlanService>().Repository = retVal.GetService<IClinicalProtocolRepositoryService>();
                        ApplicationServiceContext.Current = ApplicationContext.Current;
                        ApplicationServiceContext.HostType = OpenIZHostType.OtherClient;

                    }
                    catch (Exception e)
                    {
                        retVal.m_tracer.TraceError(e.ToString());
                        throw;
                    }
                    finally
                    {
                        retVal.ConfigurationManager.Save();
                    }

                    // Set the tracer writers for the PCL goodness!
                    foreach (var itm in retVal.Configuration.GetSection<DiagnosticsConfigurationSection>().TraceWriter)
                    {
                        OpenIZ.Core.Diagnostics.Tracer.AddWriter(itm.TraceWriter, itm.Filter);
                    }

                    // Start daemons
                    retVal.GetService<IThreadPoolService>().QueueUserWorkItem(o => { retVal.Start(); });

                    //retVal.Start();

                }
                catch (Exception e)
                {
                    retVal.m_tracer?.TraceError(e.ToString());
                    //ApplicationContext.Current = null;
                    retVal.m_configurationManager= new DcConfigurationManager(DcConfigurationManager.GetDefaultConfiguration());
                    AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);
                    throw;
                }
                return true;
            }
        }




        private static Dictionary<String, String> mime = new Dictionary<string, string>()
        {
            { ".eot", "application/vnd.ms-fontobject" },
            { ".woff", "application/font-woff" },
            { ".woff2", "application/font-woff2" },
            { ".ttf", "application/octet-stream" },
            { ".svg", "image/svg+xml" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".png", "image/png" },
            { ".bmp", "image/bmp" },
            { ".json", "application/json" }

        };

        /// <summary>
        /// Process the specified directory
        /// </summary>
        private IEnumerable<AppletAsset> ProcessDirectory(string source, String path)
        {
            List<AppletAsset> retVal = new List<AppletAsset>();
            foreach (var itm in Directory.GetFiles(source))
            {
                Console.WriteLine("\t Processing {0}...", itm);

                if (Path.GetFileName(itm).ToLower() == "manifest.xml")
                    continue;
                else
                    switch (Path.GetExtension(itm))
                    {
                        case ".html":
                        case ".htm":
                        case ".xhtml":
                            XElement xe = XElement.Load(itm);
                            // Now we have to iterate throuh and add the asset\

                            var demand = xe.DescendantNodes().OfType<XElement>().Where(o => o.Name == xs_openiz + "demand").Select(o => o.Value).ToList();

                            var includes = xe.DescendantNodes().OfType<XComment>().Where(o => o?.Value?.Trim().StartsWith("#include virtual=\"") == true).ToList();
                            foreach (var inc in includes)
                            {
                                String assetName = inc.Value.Trim().Substring(18); // HACK: Should be a REGEX
                                if (assetName.EndsWith("\""))
                                    assetName = assetName.Substring(0, assetName.Length - 1);
                                if (assetName == "content")
                                    continue;
                                var includeAsset = ResolveName(assetName);
                                inc.AddAfterSelf(new XComment(String.Format("#include virtual=\"{0}\"", includeAsset)));
                                inc.Remove();

                            }


                            var xel = xe.Descendants().OfType<XElement>().Where(o => o.Name.Namespace == xs_openiz).ToList();
                            if (xel != null)
                                foreach (var x in xel)
                                    x.Remove();
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "text/html",
                                Content = null,
                                Policies = demand

                            });
                            break;
                        case ".css":
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "text/css",
                                Content = null
                            });
                            break;
                        case ".js":
                        case ".json":
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "text/javascript",
                                Content = null
                            });
                            break;
                        default:
                            string mt = null;
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = mime.TryGetValue(Path.GetExtension(itm), out mt) ? mt : "application/octet-stream",
                                Content = null
                            });
                            break;

                    }
            }

            // Process sub directories
            foreach (var dir in Directory.GetDirectories(source))
                if (!Path.GetFileName(dir).StartsWith("."))
                    retVal.AddRange(ProcessDirectory(dir, path));
                else
                    Console.WriteLine("Skipping directory {0}", dir);

            return retVal;
        }

        /// <summary>
        /// Resolve the specified applet name
        /// </summary>
        private static String ResolveName(string value)
        {

            return value?.ToLower().Replace("\\", "/");
        }

        
        /// <summary>
        /// Save configuration
        /// </summary>
        public override void SaveConfiguration()
        {
            if(this.m_configurationManager.IsConfigured)
                this.m_configurationManager.Save(); 
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        public override void Exit()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Confirmation
        /// </summary>
        public override bool Confirm(string confirmText)
        {
			return this.m_dialogProvider.Confirm(confirmText, String.Empty);
        }

        /// <summary>
        /// Show an alert
        /// </summary>
        public override void Alert(string alertText)
        {
			this.m_dialogProvider.Alert(alertText);
        }


        /// <summary>
        /// Performance log!
        /// </summary>
        public override void PerformanceLog(string className, string methodName, string tagName, TimeSpan counter)
        {
        }

        /// <summary>
        /// In the OpenIZ DC setting the current context security key is the current windows user SID (since we're storing data in appdata it is 
        /// encrypted per user SID)
        /// </summary>
        public override byte[] GetCurrentContextSecurityKey()
        {
#if NOCRYPT
            return null;
#else
			if(Environment.OSVersion.Platform == PlatformID.Win32NT) {
				var sid = WindowsIdentity.GetCurrent().User;
	            byte[] retVal = new byte[sid.BinaryLength];
	            WindowsIdentity.GetCurrent().User.GetBinaryForm(retVal, 0);
	            return retVal;
			}
			else 
			{
				// Use MAC Address (best we can do)
				var macadd = this.GetService<INetworkInformationService>().GetInterfaces().Where(o=>!String.IsNullOrEmpty(o.MacAddress)).FirstOrDefault();
				return macadd.MacAddress == null ? null : Encoding.UTF8.GetBytes(macadd.MacAddress);
			}
			//	return null;
#endif
        }
    }
}
