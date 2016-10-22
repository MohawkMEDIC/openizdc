using Newtonsoft.Json;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Mobile.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers;
using OpenIZ.Mobile.Core.Xamarin.Diagnostics;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Model
{
    /// <summary>
    /// Application version information
    /// </summary>
    [JsonObject("VersionInfo"), XmlType(nameof(VersionInfo), Namespace = "http://openiz.org/model/mobile")]
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


        [JsonProperty("version"), XmlAttribute("version")]
        public String Version { get; set; }
        [JsonProperty("infoVersion"), XmlAttribute("infoVersion")]
        public String InformationalVersion { get; set; }
        [JsonProperty("copyright"), XmlElement("copyright")]
        public String Copyright { get; set; }

        [JsonProperty("company"), XmlElement("company")]
        public String Company { get; set; }

        [JsonProperty("product"), XmlElement("product")]
        public String Product { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public String Name { get; set; }
        /// <summary>
        /// Gets or sets the informational value
        /// </summary>
        [JsonProperty("info"), XmlElement("info")]
        public String Info { get; set; }
    }

    /// <summary>
    /// Application information
    /// </summary>
    [JsonObject("ApplicationInfo"), XmlType(nameof(ApplicationInfo), Namespace = "http://openiz.org/model/mobile")]
    public class ApplicationInfo : VersionInfo
    {
        private Tracer m_tracer = Tracer.GetTracer(typeof(ApplicationService));

        /// <summary>
        /// Application information
        /// </summary>
        public ApplicationInfo() : base(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.DefinedTypes.Any(t => t.Name == "SplashActivity")) ?? typeof(OpenIZConfiguration).Assembly)
        {
            this.OpenIZ = new VersionInfo(typeof(OpenIZ.Mobile.Core.ApplicationContext).Assembly);
            this.Applets = XamarinApplicationContext.Current.LoadedApplets.Select(o => o.Info).ToList();
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
                var logFileName = ApplicationContext.Current.Configuration.GetSection<DiagnosticsConfigurationSection>().TraceWriter.FirstOrDefault(o => o.TraceWriter.GetType() == typeof(FileTraceWriter)).InitializationData;
                logFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log", logFileName + ".log");
                var logFile = new FileInfo(logFileName);
                this.FileInfo = new List<RuntimeFileInfo>()
                    {
                        new RuntimeFileInfo() { FileDescription = "Core Data", FileName = mainDb.FullName, LastWriteDate = mainDb.LastWriteTime, FileSize = mainDb.Length },
                        new RuntimeFileInfo() { FileDescription = "Queue Data", FileName = queueDb.FullName, LastWriteDate = queueDb.LastWriteTime, FileSize = queueDb.Length },
                        new RuntimeFileInfo() { FileDescription = "Log File", FileName = logFile.FullName, LastWriteDate = logFile.LastWriteTime, FileSize = logFile.Length }
                    };

                this.SyncInfo = Synchronization.SynchronizationLog.Current.GetAll().Select(o => new RemoteSyncInfo()
                {
                    Etag = o.LastETag,
                    LastSync = o.LastSync,
                    ResourceName = o.ResourceType,
                    Filter = o.Filter
                }).ToList();

            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error gathering system info {0}", e);
            }
        }

        /// <summary>
        /// Environment information
        /// </summary>
        [JsonProperty("environment"), XmlElement("environment")]
        public EnvironmentInfo EnvironmentInfo { get; set; }

        /// <summary>
        /// Open IZ information
        /// </summary>
        [JsonProperty("openiz"), XmlElement("openiz")]
        public VersionInfo OpenIZ { get; set; }

        /// <summary>
        /// Gets or sets the assemblies
        /// </summary>
        [JsonProperty("assembly"), XmlElement("assembly")]
        public List<VersionInfo> Assemblies { get; set; }

        /// <summary>
        /// Gets or sets the applets
        /// </summary>
        [JsonProperty("applet"), XmlElement("applet")]
        public List<AppletInfo> Applets { get; set; }

        /// <summary>
        /// Gets or sets file info
        /// </summary>
        [JsonProperty("fileInfo"), XmlElement("fileInfo")]
        public List<RuntimeFileInfo> FileInfo { get; set; }

        /// <summary>
        /// Gets the sync info
        /// </summary>
        [JsonProperty("syncInfo"), XmlElement("syncInfo")]
        public List<RemoteSyncInfo> SyncInfo { get; set; }

        /// <summary>
        /// Configuration
        /// </summary>
        [JsonProperty("config"), XmlElement("config")]
        public OpenIZConfiguration Configuration { get; set; }


    }

    /// <summary>
    /// Environment information
    /// </summary>
    [JsonObject("EnvironmentInfo"), XmlType(nameof(EnvironmentInfo), Namespace = "http://openiz.org/model/mobile")]
    public class EnvironmentInfo
    {
        /// <summary>
        /// Is platform 64 bit
        /// </summary>
        [JsonProperty("is64bit"), XmlAttribute("is64Bit")]
        public bool Is64Bit { get; internal set; }
        /// <summary>
        /// OS Version
        /// </summary>
        [JsonProperty("osVersion"), XmlAttribute("osVersion")]
        public String OSVersion { get; internal set; }
        /// <summary>
        /// CPU count
        /// </summary>
        [JsonProperty("processorCount"), XmlAttribute("processorCount")]
        public int ProcessorCount { get; internal set; }
        /// <summary>
        /// Used memory
        /// </summary>
        [JsonProperty("usedMem"), XmlElement("mem")]
        public long UsedMemory { get; internal set; }
        /// <summary>
        /// Version
        /// </summary>
        [JsonProperty("version"), XmlElement("version")]
        public String Version { get; internal set; }
    }

    /// <summary>
    /// Remote sync info
    /// </summary>
    [JsonObject("RemoteSyncInfo"), XmlType(nameof(RemoteSyncInfo), Namespace = "http://openiz.org/model/mobile")]
    public class RemoteSyncInfo
    {
        [JsonProperty("resource"), XmlAttribute("resource")]
        public String ResourceName { get; set; }

        [JsonProperty("etag"), XmlAttribute("etag")]
        public String Etag { get; set; }

        [JsonProperty("lastSync"), XmlAttribute("lastSync")]
        public DateTime LastSync { get; set; }
        /// <summary>
        /// Filter used to sync
        /// </summary>
        [JsonProperty("filter"), XmlText]
        public String Filter { get; set; }
    }

    /// <summary>
    /// Runtime file inforamtion
    /// </summary>
    [JsonObject("RuntimeFileInfo"), XmlType(nameof(RuntimeFileInfo), Namespace = "http://openiz.org/model/mobile")]
    public class RuntimeFileInfo 
    {
        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        [JsonProperty("file"), XmlAttribute("file")]
        public String FileName { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty("description"), XmlElement("description")]
        public String FileDescription { get; set; }

        /// <summary>
        /// Size of the file
        /// </summary>
        [JsonProperty("size"), XmlAttribute("size")]
        public long FileSize { get; set; }

        /// <summary>
        /// Last write date
        /// </summary>
        [JsonProperty("lastWrite"), XmlAttribute("lastWrite")]
        public DateTime LastWriteDate { get; set; }
    }

}
