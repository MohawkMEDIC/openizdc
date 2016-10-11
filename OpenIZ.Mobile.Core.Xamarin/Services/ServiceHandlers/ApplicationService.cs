using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Mobile.Core.Alerting;
using OpenIZ.Core.Model.Query;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Services;
using Newtonsoft.Json;
using OpenIZ.Core.Applets.Model;
using System.Reflection;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using OpenIZ.Core.Alert.Alerting;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents a service for interacting with the application asynchronously
    /// </summary>
    [RestService("/__app")]
    public class ApplicationService
    {

        /// <summary>
        /// Application version information
        /// </summary>
        [JsonObject]
        public class VersionInfo
        {
            public VersionInfo()
            {

            }
            /// <summary>
            /// Version information
            /// </summary>
            public VersionInfo(Assembly asm)
            {
                if (asm == null) return;
                this.Version = asm.GetName().Version.ToString();
                this.InformationalVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                this.Copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
                this.Product = asm.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
                this.Name = asm.GetName().Name;
                this.Company = asm.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
                this.Info = asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            }

            [JsonProperty("version")]
            public String Version { get; set; }
            [JsonProperty("infoVersion")]
            public String InformationalVersion { get; set; }
            [JsonProperty("copyright")]
            public String Copyright { get; set; }

            [JsonProperty("company")]
            public String Company { get; set; }

            [JsonProperty("product")]
            public String Product { get; set; }
            /// <summary>
            /// Gets or sets the name
            /// </summary>
            [JsonProperty("name")]
            public String Name { get; set; }
            /// <summary>
            /// Gets or sets the informational value
            /// </summary>
            [JsonProperty("info")]
            public String Info { get; set; }
        }

        /// <summary>
        /// Application information
        /// </summary>
        [JsonObject]
        public class ApplicationInfo : VersionInfo
        {
            private Tracer m_tracer = Tracer.GetTracer(typeof(ApplicationService));

            /// <summary>
            /// Application information
            /// </summary>
            public ApplicationInfo() : base(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.DefinedTypes.Any(t => t.Name == "SplashActivity")) ?? typeof(OpenIZConfiguration).Assembly)
            {
                this.OpenIZ = new VersionInfo(typeof(OpenIZ.Mobile.Core.ApplicationContext).Assembly);
                this.Applets = AndroidApplicationContext.Current.LoadedApplets.Select(o => o.Info).ToList();
                this.Assemblies = AppDomain.CurrentDomain.GetAssemblies().Select(o => new VersionInfo(o)).ToList();
                this.Configuration = ApplicationContext.Current.Configuration;
                this.EnvironmentInfo = new EnvironmentInfo()
                {
                    OSVersion = String.Format("{0} v{1}", System.Environment.OSVersion.Platform, System.Environment.OSVersion.Version),
                    Is64Bit = System.Environment.Is64BitOperatingSystem,
                    ProcessorCount = System.Environment.ProcessorCount, 
                    Version = System.Environment.Version.ToString(),
                    UsedMemory = GC.GetTotalMemory(false)
                };

                // Files of interest
                try
                {
                    // Configuration files
                    var mainDb = new FileInfo(ApplicationContext.Current.Configuration.GetConnectionString("openIzData").Value);
                    var queueDb = new FileInfo(ApplicationContext.Current.Configuration.GetConnectionString("openIzQueue").Value);
                    this.FileInfo = new List<RuntimeFileInfo>()
                    {
                        new RuntimeFileInfo() { FileDescription = "Core Data", FileName = mainDb.FullName, LastWriteDate = mainDb.LastWriteTime, FileSize = mainDb.Length },
                        new RuntimeFileInfo() { FileDescription = "Queue Data", FileName = queueDb.FullName, LastWriteDate = queueDb.LastWriteTime, FileSize = queueDb.Length }
                    };

                    this.SyncInfo = Synchronization.SynchronizationLog.Current.GetAll().Select(o=>new RemoteSyncInfo()
                    {
                        Etag = o.LastETag,
                        LastSync = o.LastSync,
                        ResourceName = o.ResourceType,
                        Filter = o.Filter
                    }).ToList();

                }
                catch(Exception e)
                {
                    this.m_tracer.TraceError("Error gathering system info {0}", e);
                }
            }

            /// <summary>
            /// Environment information
            /// </summary>
            [JsonProperty("environment")]
            public EnvironmentInfo EnvironmentInfo { get; set; }

            /// <summary>
            /// Open IZ information
            /// </summary>
            [JsonProperty("openiz")]
            public VersionInfo OpenIZ { get; set; }

            /// <summary>
            /// Gets or sets the assemblies
            /// </summary>
            [JsonProperty("assembly")]
            public List<VersionInfo> Assemblies { get; set; }

            /// <summary>
            /// Gets or sets the applets
            /// </summary>
            [JsonProperty("applet")]
            public List<AppletInfo> Applets { get; set; }

            /// <summary>
            /// Gets or sets file info
            /// </summary>
            [JsonProperty("fileInfo")]
            public List<RuntimeFileInfo> FileInfo { get; set; }

            /// <summary>
            /// Gets the sync info
            /// </summary>
            [JsonProperty("syncInfo")]
            public List<RemoteSyncInfo> SyncInfo { get; set; }

            /// <summary>
            /// Configuration
            /// </summary>
            [JsonProperty("config")]
            public OpenIZConfiguration Configuration { get; set; }


        }

        /// <summary>
        /// Environment information
        /// </summary>
        [JsonObject]
        public class EnvironmentInfo
        {
            /// <summary>
            /// Is platform 64 bit
            /// </summary>
            [JsonProperty("is64bit")]
            public bool Is64Bit { get; internal set; }
            /// <summary>
            /// OS Version
            /// </summary>
            [JsonProperty("osVersion")]
            public String OSVersion { get; internal set; }
            /// <summary>
            /// CPU count
            /// </summary>
            [JsonProperty("processorCount")]
            public int ProcessorCount { get; internal set; }
            /// <summary>
            /// Used memory
            /// </summary>
            [JsonProperty("usedMem")]
            public long UsedMemory { get; internal set; }
            /// <summary>
            /// Version
            /// </summary>
            [JsonProperty("version")]
            public String Version { get; internal set; }
        }

        /// <summary>
        /// Remote sync info
        /// </summary>
        [JsonObject]
        public class RemoteSyncInfo
        {
            [JsonProperty("resource")]
            public String ResourceName { get; set; }

            [JsonProperty("etag")]
            public String Etag { get; set; }

            [JsonProperty("lastSync")]
            public DateTime LastSync { get; set; }
            /// <summary>
            /// Filter used to sync
            /// </summary>
            [JsonProperty("filter")]
            public String Filter { get; set; }
        }

        /// <summary>
        /// Runtime file inforamtion
        /// </summary>
        [JsonObject]
        public class RuntimeFileInfo
        {
            /// <summary>
            /// Gets or sets the file name
            /// </summary>
            [JsonProperty("file")]
            public String FileName { get; set; }

            /// <summary>
            /// Description
            /// </summary>
            [JsonProperty("description")]
            public String FileDescription { get; set; }

            /// <summary>
            /// Size of the file
            /// </summary>
            [JsonProperty("size")]
            public long FileSize { get; set; }

            /// <summary>
            /// Last write date
            /// </summary>
            [JsonProperty("lastWrite")]
            public DateTime LastWriteDate { get; set; }
        }

        // App information
        private ApplicationInfo m_appInfo;

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(ApplicationService));

        /// <summary>
        /// Get the alerts from the service
        /// </summary>
        [RestOperation(UriPath = "/info", Method = "GET")]
        public ApplicationInfo GetInfo()
        {
            try
            {
                return new ApplicationInfo();
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Could not retrieve app info {0}...", e);
                throw;
            }
        }

        /// <summary>
        /// Get the alerts from the service
        /// </summary>
        [RestOperation(UriPath = "/alerts", Method = "GET")]
        public List<AlertMessage> GetAlerts()
        {
            try
            {
                // Gets the specified alert messages
                NameValueCollection query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

				var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();

				List<string> key = null;

				if (query.ContainsKey("id") && query.TryGetValue("id", out key))
				{
					var id = key?.FirstOrDefault();

					return new List<AlertMessage> { alertService.Get(Guid.Parse(id)) };
				}

                var predicate = QueryExpressionParser.BuildLinqExpression<AlertMessage>(query);
                int offset = query.ContainsKey("_offset") ? Int32.Parse(query["_offset"][0]) : 0,
                    count = query.ContainsKey("_count") ? Int32.Parse(query["_count"][0]) : 100;

                

				int totalCount = 0;

                return alertService.Find(predicate, offset, count, out totalCount).ToList();
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Could not retrieve alerts {0}...", e);
                throw;
            }
        }

        /// <summary>
        /// Get the alerts from the service
        /// </summary>
        [RestOperation(UriPath = "/alerts", Method = "POST")]
        public AlertMessage SaveAlert(AlertMessage alert)
        {
            try
            {
                // Gets the specified alert messages
                var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
                alertService.Save(alert);
                return alert;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Could not retrieve alerts {0}...", e);
                return null;
            }
        }

    }

}