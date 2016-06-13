using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services.Impl;
using OpenIZ.Mobile.Core.Data;
using OpenIZ.Mobile.Core.Configuration.Data;
using OpenIZ.Core.Model.EntityLoader;
using System.Diagnostics;

namespace OpenIZ.Mobile.Core.Test
{
    /// <summary>
    /// Test application context
    /// </summary>
    public class TestApplicationContext : ApplicationContext
    {

        private OpenIZConfiguration m_configuration;
        private TestContext m_context;
        /// <summary>
        /// Gets or sets the unit test context
        /// </summary>
        public TestContext UnitTestContext
        {
            get
            {
                return this.m_context;
            }
            set
            {
                this.m_context = value;

                if (m_configuration == null)
                {
                    // TODO: Bring up initial settings dialog and utility
                    m_configuration = new OpenIZConfiguration();

                    // Inital data source
                    DataConfigurationSection dataSection = new DataConfigurationSection()
                    {
                        MainDataSourceConnectionStringName = "openIzData",
                        MessageQueueConnectionStringName = "openIzQueue",
                        ConnectionString = new System.Collections.Generic.List<ConnectionString>() {
                    new ConnectionString () {
                        Name = "openIzData",
                        Value = Path.Combine (this.UnitTestContext.TestDeploymentDir, "OpenIZ.sqlite")
                    },
                    new ConnectionString () {
                        Name = "openIzQueue",
                        Value = Path.Combine (this.UnitTestContext.TestDeploymentDir, "MessageQueue.sqlite")
                    },
                }
                    };

                    // Initial Applet configuration
                    AppletConfigurationSection appletSection = new AppletConfigurationSection()
                    {
                        AppletDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "applets"),
                        AppletGroupOrder = new System.Collections.Generic.List<string>() {
                    "Patient Management",
                    "Encounter Management",
                    "Stock Management",
                    "Administration"
                },
                        StartupAsset = "app://openiz.org/applet/org.openiz.applet.core/index",
                        AuthenticationAsset = "app://openiz.org/applet/org.openiz.applet.core.authentication/login"
                    };

                    // Initial applet style
                    ApplicationConfigurationSection appSection = new ApplicationConfigurationSection()
                    {
                        Style = StyleSchemeType.Dark,
                        UserPrefDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "userpref"),
                        ServiceTypes = new List<string>() {
                    typeof(LocalPolicyDecisionService).AssemblyQualifiedName,
                    typeof(LocalPolicyInformationService).AssemblyQualifiedName,
                    typeof(LocalConceptService).AssemblyQualifiedName,
                    typeof(SHA256PasswordHasher).AssemblyQualifiedName,
                    typeof(LocalIdentityService).AssemblyQualifiedName,
                    typeof(LocalPersistenceService).AssemblyQualifiedName,
                    typeof(LocalEntitySource).AssemblyQualifiedName
                }
                    };




                    SecurityConfigurationSection secSection = new SecurityConfigurationSection()
                    {
                        DeviceName = String.Format("TESTDEV")
                    };

                    // Trace writer
                    DiagnosticsConfigurationSection diagSection = new DiagnosticsConfigurationSection()
                    {
                        TraceWriter = new System.Collections.Generic.List<TraceWriterConfiguration>() {
                    new TraceWriterConfiguration () {
                        Filter = System.Diagnostics.Tracing.EventLevel.LogAlways,
                        InitializationData = "OpenIZ",
                        TraceWriter = new TestTraceWriter(System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ")
                    }
                }
                    };

                    m_configuration.Sections.Add(appletSection);
                    m_configuration.Sections.Add(dataSection);
                    m_configuration.Sections.Add(diagSection);
                    m_configuration.Sections.Add(appSection);
                    m_configuration.Sections.Add(secSection);

                    try
                    {
                        // If the DB File doesn't exist we have to clear the migrations
                        if (!File.Exists(this.m_configuration.GetConnectionString(this.m_configuration.GetSection<DataConfigurationSection>().MainDataSourceConnectionStringName).Value))
                        {
                            this.m_configuration.GetSection<DataConfigurationSection>().MigrationLog.Entry.Clear();
                        }

                        DataMigrator migrator = new DataMigrator();
                        migrator.Ensure();

                        // Set the entity source
                        if (EntitySource.Current == null)
                            EntitySource.Current = new EntitySource(this.GetService<IEntitySourceProvider>());

                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// Application 
        /// </summary>
        public override SecurityApplication Application
        {
            get
            {
                return new SecurityApplication() { Name = "TEST" };
            }
        }

        /// <summary>
        /// Gets the configuration
        /// </summary>
        public override OpenIZConfiguration Configuration
        {
            get
            {
                return this.m_configuration;
            }
        }

        /// <summary>
        /// Gets the test device
        /// </summary>
        public override SecurityDevice Device
        {
            get
            {
                return new SecurityDevice() { Name = "TEST" };

            }
        }
    }
}
