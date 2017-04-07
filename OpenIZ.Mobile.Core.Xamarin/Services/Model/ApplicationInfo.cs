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
using OpenIZ.Mobile.Core.Services;

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
        /// Updates
        /// </summary>
        [JsonProperty("update")]
        public List<AppletInfo> Updates { get; private set; }

        /// <summary>
        /// Application information
        /// </summary>
        public ApplicationInfo(bool checkForUpdates) : base(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.DefinedTypes.Any(t => t.Name == "SplashActivity")) ?? typeof(OpenIZConfiguration).Assembly)
        {
            this.OpenIZ = new DiagnosticVersionInfo(typeof(OpenIZ.Mobile.Core.ApplicationContext).Assembly);

            this.Applets = XamarinApplicationContext.Current.LoadedApplets.Select(o => o.Info).ToList();

            if(checkForUpdates)
                try
                {
                    this.Updates = XamarinApplicationContext.Current.LoadedApplets.Select(o => ApplicationContext.Current.GetService<IUpdateManager>().GetServerVersion(o.Info.Id)).ToList();
                    this.Updates.RemoveAll(o => new Version(XamarinApplicationContext.Current.GetApplet(o.Id).Info.Version).CompareTo(new Version(o.Version)) > 0);
                }
                catch { }

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
                var logFileName = ApplicationContext.Current.Configuration.GetSection<DiagnosticsConfigurationSection>().TraceWriter.FirstOrDefault(o => o.TraceWriter.GetType() == typeof(FileTraceWriter)).InitializationData;

                logFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log", logFileName + ".log");
                var logFile = new FileInfo(logFileName);

                // File information
                this.FileInfo = new List<DiagnosticAttachmentInfo>()
                {
                    new DiagnosticAttachmentInfo() { FileDescription = "Log File", FileSize = logFile.Length, FileName = logFile.Name, Id = "log", LastWriteDate = logFile.LastWriteTime }
                };

                foreach (var con in ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().ConnectionString)
                {
                    var fi = new FileInfo(con.Value);
                    this.FileInfo.Add(new DiagnosticAttachmentInfo() { FileDescription = con.Name, FileName = fi.FullName, LastWriteDate = fi.LastWriteTime, FileSize = fi.Length, Id = "db" });
                    
                    // Existing path
                    if(File.Exists(Path.ChangeExtension(con.Value, "bak")))
                    {
                        fi = new FileInfo(Path.ChangeExtension(con.Value, "bak"));
                        this.FileInfo.Add(new DiagnosticAttachmentInfo() { FileDescription = con.Name + " Backup", FileName = fi.FullName, LastWriteDate = fi.LastWriteTime, FileSize = fi.Length, Id = "bak" });
                    }
                }


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
