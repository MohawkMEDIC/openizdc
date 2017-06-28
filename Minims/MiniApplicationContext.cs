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
 * Date: 2017-3-31
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
using OpenIZ.Core.Applets.Services;

namespace Minims
{
    /// <summary>
    /// Test application context
    /// </summary>
    public class MiniApplicationContext : XamarinApplicationContext
    {
      
        // The application
        private static readonly OpenIZ.Core.Model.Security.SecurityApplication c_application = new OpenIZ.Core.Model.Security.SecurityApplication()
        {
            ApplicationSecret = "A1CF054D04D04CD1897E114A904E328D",
            Key = Guid.Parse("4C5A581C-A6EE-4267-9231-B0D3D50CC08A"),
            Name = "org.openiz.minims"
        };

          private MiniConfigurationManager m_configurationManager;

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
            Console.WriteLine("TOAST >>>> {0}", subject);
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
        static MiniApplicationContext()
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
		/// Starts the application context using in-memory default configuration for the purposes of 
		/// configuring the software
		/// </summary>
		/// <returns><c>true</c>, if temporary was started, <c>false</c> otherwise.</returns>
		public static bool StartTemporary(ConsoleParameters consoleParms)
        {
            try
            {
                var retVal = new MiniApplicationContext();
                retVal.SetProgress("Run setup", 0);

                retVal.m_configurationManager = new MiniConfigurationManager(MiniConfigurationManager.GetDefaultConfiguration());
               
                ApplicationContext.Current = retVal;
                ApplicationServiceContext.Current = ApplicationContext.Current;
                ApplicationServiceContext.HostType = OpenIZHostType.OtherClient;

                retVal.m_tracer = Tracer.GetTracer(typeof(MiniApplicationContext));
                retVal.ThreadDefaultPrincipal = AuthenticationContext.SystemPrincipal;

                retVal.SetProgress("Loading configuration", 0.2f);
                // Load all user-downloaded applets in the data directory
                foreach (var appletDir in consoleParms.AppletDirectories)// Directory.GetFiles(this.m_configuration.GetSection<AppletConfigurationSection>().AppletDirectory)) {
                    try
                    {
                        if (!Directory.Exists(appletDir) || !File.Exists(Path.Combine(appletDir, "manifest.xml")))
                            continue;

                        retVal.m_tracer.TraceInfo("Loading applet {0}", appletDir);
                        String appletPath = Path.Combine(appletDir, "manifest.xml");
                        var appService = retVal.GetService<IAppletManagerService>();
                        using (var fs = File.OpenRead(appletPath))
                        {
                            AppletManifest manifest = AppletManifest.Load(fs);
                            (appService as MiniAppletManagerService).m_appletBaseDir.Add(manifest, appletDir);
                            // Is this applet in the allowed applets

                            // public key token match?
                            appService.LoadApplet(manifest);
                        }
                    }
                    catch (Exception e)
                    {
                        retVal.m_tracer.TraceError("Loading applet {0} failed: {1}", appletDir, e.ToString());
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
        public static bool Start(ConsoleParameters consoleParms)
        {

            var retVal = new MiniApplicationContext();
            retVal.m_configurationManager = new MiniConfigurationManager();
            // Not configured
            if (!retVal.ConfigurationManager.IsConfigured)
            {
                return false;
            }
            else
            { // load configuration
                try
                {
                    retVal.ConfigurationManager.Load();

                    // Set master application context
                    ApplicationContext.Current = retVal;
                    retVal.m_tracer = Tracer.GetTracer(typeof(MiniApplicationContext), retVal.ConfigurationManager.Configuration);

                    var appService = retVal.GetService<IAppletManagerService>();

                    retVal.SetProgress("Loading configuration", 0.2f);
                    // Load all user-downloaded applets in the data directory
                    foreach (var appletDir in consoleParms.AppletDirectories)// Directory.GetFiles(this.m_configuration.GetSection<AppletConfigurationSection>().AppletDirectory)) {
                        try
                        {
                            if (!Directory.Exists(appletDir) || !File.Exists(Path.Combine(appletDir, "manifest.xml")))
                                continue;

                            retVal.m_tracer.TraceInfo("Loading applet {0}", appletDir);
                            String appletPath = Path.Combine(appletDir, "manifest.xml");
                            using (var fs = File.OpenRead(appletPath))
                            {
                                AppletManifest manifest = AppletManifest.Load(fs);
                                (appService as MiniAppletManagerService).m_appletBaseDir.Add(manifest, appletDir);
                                // Is this applet in the allowed applets

                                // public key token match?
                                appService.LoadApplet(manifest);
                            }
                        }
                        catch (Exception e)
                        {
                            retVal.m_tracer.TraceError("Loading applet {0} failed: {1}", appletDir, e.ToString());
                            throw;
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

                    if(!retVal.Configuration.GetSection<DiagnosticsConfigurationSection>().TraceWriter.Any(o=>o.TraceWriterClassXml.Contains("Console")))
                        retVal.Configuration.GetSection<DiagnosticsConfigurationSection>().TraceWriter.Add(new TraceWriterConfiguration()
                        {
                            TraceWriter = new ConsoleTraceWriter(EventLevel.Warning, "")
                        });


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
                    ApplicationContext.Current = null;
                    throw;
                }
                return true;
            }
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
            return System.Windows.Forms.MessageBox.Show(confirmText, String.Empty, System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK;
        }

        /// <summary>
        /// Show an alert
        /// </summary>
        public override void Alert(string alertText)
        {
            System.Windows.Forms.MessageBox.Show(alertText);
        }

        /// <summary>
        /// Performance log!
        /// </summary>
        public override void PerformanceLog(string className, string methodName, string tagName, TimeSpan counter)
        {
            this.GetService<IThreadPoolService>().QueueUserWorkItem(o =>
            {
                lock (this.m_configurationManager)
                {
                    var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), this.ExecutionUuid.ToString() + ".perf.txt");
                    File.AppendAllText(path, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - {className}.{methodName}@{tagName} - {counter}\r\n");
                }
            });
        }
    }
}
