using OpenIZ.Mobile.Core.Xamarin.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Core.Diagnostics;
using OizDebug.Options;
using MohawkCollege.Util.Console.Parameters;
using System.IO;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services.Impl;
using OpenIZ.Mobile.Core.Xamarin.Net;
using OpenIZ.Mobile.Core.Data.Warehouse;
using OpenIZ.Mobile.Core.Xamarin.Rules;
using OpenIZ.Mobile.Core.Data;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Xamarin.Threading;
using OpenIZ.Core.Protocol;
using OpenIZ.Protocol.Xml;
using OpenIZ.Core.Services.Impl;
using OpenIZ.Mobile.Core.Xamarin.Services;
using OpenIZ.Mobile.Core.Xamarin.Http;
using OpenIZ.Mobile.Core.Xamarin.Diagnostics;
using OizDebug.Shell;
using OpenIZ.Mobile.Core.Data.Connection;

namespace OizDebug.Core
{
    /// <summary>
    /// Represents a configuration manager which is used for the debugger
    /// </summary>
    public class DebugConfigurationManager : IConfigurationManager
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(DebugConfigurationManager));

        // Current configuration
        private OpenIZConfiguration m_configuration;

        // Configuration path
        private readonly string m_configPath = String.Empty;

        // Data path
        private readonly string m_dataPath = string.Empty;

        /// <summary>
        /// Creates a new debug configuration manager with the specified parameters
        /// </summary>
        public DebugConfigurationManager(DebuggerParameters parms)
        {
            // Get parameters from args
            if (!String.IsNullOrEmpty(parms.ConfigurationFile))
                this.m_configPath = parms.ConfigurationFile;
            if (!String.IsNullOrEmpty(parms.DatabaseFile))
                this.m_dataPath = parms.DatabaseFile;

        }

        /// <summary>
        /// Gets the application data directory
        /// </summary>
        public string ApplicationDataDirectory
        {
            get
            {
                if (this.m_dataPath != null)
                    return Path.GetDirectoryName(this.m_dataPath);
                else
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenIZDC");
            }
        }

        /// <summary>
        /// Get the configuration
        /// </summary>
        public OpenIZConfiguration Configuration
        {
            get
            {
                if (this.m_configuration == null)
                    this.Load();
                return this.m_configuration;
            }
        }

        /// <summary>
        /// Returns true if the 
        /// </summary>
        public bool IsConfigured
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Perform a backup
        /// </summary>
        public void Backup()
        {
            throw new NotImplementedException("Debug environment cannot backup");
        }

        /// <summary>
        /// Get whether there is a backup
        /// </summary>
        /// <returns></returns>
        public bool HasBackup()
        {
            return false;
        }

        /// <summary>
        /// Load the configuration file
        /// </summary>
        public void Load()
        {
            if (!String.IsNullOrEmpty(this.m_configPath))
                using (var fs = File.OpenRead(this.m_configPath))
                {
                    this.m_configuration = OpenIZConfiguration.Load(fs);
                }
            else
            {
                this.m_configuration = new OpenIZConfiguration();

                // Inital data source
                DataConfigurationSection dataSection = new DataConfigurationSection()
                {
                    MainDataSourceConnectionStringName = "openIzData",
                    MessageQueueConnectionStringName = "openIzData",
                    ConnectionString = new System.Collections.Generic.List<ConnectionString>() {
                    new ConnectionString () {
                        Name = "openIzData",
                        Value = String.IsNullOrEmpty(this.m_dataPath) ?
                            Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "Minims","OpenIZ.sqlite") :
                            this.m_dataPath
                    }
                }
                };

                // Initial Applet configuration
                AppletConfigurationSection appletSection = new AppletConfigurationSection()
                {
                    Security = new AppletSecurityConfiguration()
                    {
                        AllowUnsignedApplets = true,
                        TrustedPublishers = new List<string>() { "84BD51F0584A1F708D604CF0B8074A68D3BEB973" }
                    }
                };

                // Initial applet style
                ApplicationConfigurationSection appSection = new ApplicationConfigurationSection()
                {
                    Style = StyleSchemeType.Dark,
                    UserPrefDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OizDebug", "userpref"),
                    ServiceTypes = new List<string>() {
                    typeof(LocalPolicyDecisionService).AssemblyQualifiedName,
                    typeof(LocalPolicyInformationService).AssemblyQualifiedName,
                    typeof(LocalPatientService).AssemblyQualifiedName,
                    typeof(LocalPlaceService).AssemblyQualifiedName,
                    //typeof(LocalAlertService).AssemblyQualifiedName,
                    typeof(LocalConceptService).AssemblyQualifiedName,
                    typeof(LocalEntityRepositoryService).AssemblyQualifiedName,
                    typeof(LocalOrganizationService).AssemblyQualifiedName,
                    typeof(LocalRoleProviderService).AssemblyQualifiedName,
                    typeof(LocalSecurityService).AssemblyQualifiedName,
                    typeof(LocalMaterialService).AssemblyQualifiedName,
                    typeof(LocalBatchService).AssemblyQualifiedName,
                    typeof(LocalActService).AssemblyQualifiedName,
                    typeof(LocalProviderService).AssemblyQualifiedName,
                    typeof(LocalTagPersistenceService).AssemblyQualifiedName,
                    typeof(NetworkInformationService).AssemblyQualifiedName,
                    typeof(BusinessRulesDaemonService).AssemblyQualifiedName,
                    typeof(LocalEntitySource).AssemblyQualifiedName,
                    typeof(MemoryCacheService).AssemblyQualifiedName,
                    typeof(OpenIZThreadPool).AssemblyQualifiedName,
                    typeof(MemorySessionManagerService).AssemblyQualifiedName,
                    typeof(AmiUpdateManager).AssemblyQualifiedName,
                    typeof(AppletClinicalProtocolRepository).AssemblyQualifiedName,
                    typeof(MemoryQueryPersistenceService).AssemblyQualifiedName,
                    typeof(SimpleQueueFileProvider).AssemblyQualifiedName,
                    typeof(SimpleCarePlanService).AssemblyQualifiedName,
                    typeof(SimplePatchService).AssemblyQualifiedName,
                    typeof(DebugAppletManagerService).AssemblyQualifiedName,
                    typeof(SQLiteConnectionManager).AssemblyQualifiedName,
                    typeof(LocalPersistenceService).AssemblyQualifiedName
                },
                    Cache = new CacheConfiguration()
                    {
                        MaxAge = new TimeSpan(0, 5, 0).Ticks,
                        MaxSize = 1000,
                        MaxDirtyAge = new TimeSpan(0, 20, 0).Ticks,
                        MaxPressureAge = new TimeSpan(0, 2, 0).Ticks
                    }
                };

                appSection.ServiceTypes.Add(typeof(SQLite.Net.Platform.Generic.SQLitePlatformGeneric).AssemblyQualifiedName);

                // Security configuration
                SecurityConfigurationSection secSection = new SecurityConfigurationSection()
                {
                    DeviceName = Environment.MachineName,
                    AuditRetention = new TimeSpan(30, 0, 0, 0, 0)
                };

                // Device key
                //var certificate = X509CertificateUtils.FindCertificate(X509FindType.FindBySubjectName, StoreLocation.LocalMachine, StoreName.My, String.Format("DN={0}.mobile.openiz.org", macAddress));
                //secSection.DeviceSecret = certificate?.Thumbprint;

                // Rest Client Configuration
                ServiceClientConfigurationSection serviceSection = new ServiceClientConfigurationSection()
                {
                    RestClientType = typeof(RestClient)
                };

                // Trace writer
                DiagnosticsConfigurationSection diagSection = new DiagnosticsConfigurationSection()
                {
                    TraceWriter = new System.Collections.Generic.List<TraceWriterConfiguration>() {
                    new TraceWriterConfiguration () {
                        Filter = System.Diagnostics.Tracing.EventLevel.Error,
                        InitializationData = "OpenIZ",
                        TraceWriter = new ConsoleTraceWriter (System.Diagnostics.Tracing.EventLevel.Warning, "OpenIZ")
                    },
                    new TraceWriterConfiguration() {
                        Filter = System.Diagnostics.Tracing.EventLevel.LogAlways,
                        InitializationData = "OpenIZ",
                        TraceWriter = new FileTraceWriter(System.Diagnostics.Tracing.EventLevel.Warning, "OpenIZ")
                    }
                }
                };
                this.m_configuration.Sections.Add(appletSection);
                this.m_configuration.Sections.Add(dataSection);
                this.m_configuration.Sections.Add(diagSection);
                this.m_configuration.Sections.Add(appSection);
                this.m_configuration.Sections.Add(secSection);
                this.m_configuration.Sections.Add(serviceSection);
                this.m_configuration.Sections.Add(new SynchronizationConfigurationSection()
                {
                    PollInterval = new TimeSpan(0, 5, 0)
                });
            }
        }

        /// <summary>
        /// Restoration
        /// </summary>
        public void Restore()
        {
            throw new NotImplementedException("Debug environment cannot restore backups");
        }

        /// <summary>
        /// Save the configuation
        /// </summary>
        public void Save()
        {
        }

        /// <summary>
        /// Save the specified configruation
        /// </summary>
        public void Save(OpenIZConfiguration config)
        {
        }
    }
}
