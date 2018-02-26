using OizDebug.Options;
using OpenIZ.Core;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Xamarin;
using OpenIZ.Mobile.Core.Xamarin.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Applets.Services;
using System.IO;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Mobile.Core.Configuration.Data;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Services;
using System.Reflection;

namespace OizDebug.Core
{
    /// <summary>
    /// Represents a debugger application context
    /// </summary>
    public class DebugApplicationContext : XamarinApplicationContext
    {

        // The application
        private static readonly OpenIZ.Core.Model.Security.SecurityApplication c_application = new OpenIZ.Core.Model.Security.SecurityApplication()
        {
            ApplicationSecret = "A1CF054D04D04CD1897E114A904E328D",
            Key = Guid.Parse("3E16EE70-639D-465B-86DE-043043F41098"),
            Name = "org.openiz.debugger"
        };

        private DebugConfigurationManager m_configurationManager;

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
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("TOAST >>>> {0}", subject);
            Console.ResetColor();
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
        static DebugApplicationContext()
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
        /// Start the application context
        /// </summary>
        public static bool Start(DebuggerParameters consoleParms)
        {

            var retVal = new DebugApplicationContext();
            retVal.m_configurationManager = new DebugConfigurationManager(consoleParms);

            try
            {
                // Set master application context
                ApplicationContext.Current = retVal;

                retVal.ConfigurationManager.Load();
                retVal.m_tracer = Tracer.GetTracer(typeof(DebugApplicationContext), retVal.ConfigurationManager.Configuration);

                var appService = retVal.GetService<IAppletManagerService>();

                retVal.SetProgress("Loading configuration", 0.2f);

                if (consoleParms.References != null)
                {
                    // Load references
                    foreach (var appletInfo in consoleParms.References)// Directory.GetFiles(this.m_configuration.GetSection<AppletConfigurationSection>().AppletDirectory)) {
                        try
                        {
                            retVal.m_tracer.TraceInfo("Loading applet {0}", appletInfo);
                            String appletPath = appletInfo;
                            if (!Path.IsPathRooted(appletInfo))
                                appletPath = Path.Combine(Environment.CurrentDirectory, appletPath);
                            using (var fs = File.OpenRead(appletPath))
                            {

                                var package = AppletPackage.Load(fs);
                                retVal.m_tracer.TraceInfo("Loading {0} v{1}", package.Meta.Id, package.Meta.Version);

                                // Is this applet in the allowed applets
                                appService.LoadApplet(package.Unpack());
                            }
                        }
                        catch (Exception e)
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
                throw;
            }
            return true;
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        public override void SaveConfiguration()
        {
            if (this.m_configurationManager.IsConfigured)
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
            return true;
        }

        /// <summary>
        /// Show an alert
        /// </summary>
        public override void Alert(string alertText)
        {
            Console.WriteLine("ALERT >>> {0}", alertText);
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

        /// <summary>
        /// Get current context key -- Since miniims is debuggable this is not needed
        /// </summary>
        public override byte[] GetCurrentContextSecurityKey()
        {
            return null;
        }
    }
}
