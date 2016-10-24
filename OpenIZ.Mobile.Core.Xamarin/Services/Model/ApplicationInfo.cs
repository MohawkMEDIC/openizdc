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
using OpenIZ.Core.Model.AMI.Diagnostics;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Model
{


    /// <summary>
    /// Application information
    /// </summary>
    [JsonObject("ApplicationInfo")]
    public class ApplicationInfo : DiagnosticApplicationInfo
    {
        private Tracer m_tracer = Tracer.GetTracer(typeof(ApplicationService));

        /// <summary>
        /// Application information
        /// </summary>
        public ApplicationInfo() : base(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.DefinedTypes.Any(t => t.Name == "SplashActivity")) ?? typeof(OpenIZConfiguration).Assembly)
        {
            this.OpenIZ = new DiagnosticVersionInfo(typeof(OpenIZ.Mobile.Core.ApplicationContext).Assembly);
            this.Applets = XamarinApplicationContext.Current.LoadedApplets.Select(o => o.Info).ToList();
            this.Assemblies = AppDomain.CurrentDomain.GetAssemblies().Select(o => new DiagnosticVersionInfo(o)).ToList();
            this.Configuration = ApplicationContext.Current.Configuration;
            this.EnvironmentInfo = new DiagnosticEnvironmentInfo()
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
                this.FileInfo = new List<DiagnosticAttachmentInfo>()
                    {
                        new DiagnosticAttachmentInfo() { FileDescription = "Core Data", FileName = mainDb.FullName, LastWriteDate = mainDb.LastWriteTime, FileSize = mainDb.Length },
                        new DiagnosticAttachmentInfo() { FileDescription = "Queue Data", FileName = queueDb.FullName, LastWriteDate = queueDb.LastWriteTime, FileSize = queueDb.Length },
                        new DiagnosticAttachmentInfo() { FileDescription = "Log File", FileName = logFile.FullName, LastWriteDate = logFile.LastWriteTime, FileSize = logFile.Length }
                    };

                this.SyncInfo = Synchronization.SynchronizationLog.Current.GetAll().Select(o => new DiagnosticSyncInfo()
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
        /// Create raw diagnostic report information
        /// </summary>
        /// <returns></returns>
        internal DiagnosticApplicationInfo ToDiagnosticReport()
        {
            return new DiagnosticApplicationInfo()
            {
                Applets = this.Applets,
                Assemblies = this.Assemblies,
                Copyright = this.Copyright,
                EnvironmentInfo = this.EnvironmentInfo,
                FileInfo = this.FileInfo,
                Info = this.Info,
                InformationalVersion = this.InformationalVersion,
                Name = this.Name,
                OpenIZ = this.OpenIZ,
                Product = this.Product,
                SyncInfo = this.SyncInfo,
                Version = this.Version
            };
        }

        /// <summary>
        /// Configuration
        /// </summary>
        [JsonProperty("config"), XmlElement("config")]
        public OpenIZConfiguration Configuration { get; set; }
        
    }

  

}
