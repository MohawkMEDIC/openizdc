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
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using System.Globalization;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Xamarin.Diagnostics;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Mobile.Core.Interop;
using OpenIZ.Core.Model.AMI.Diagnostics;
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using OpenIZ.Mobile.Core.Xamarin.Threading;
using OpenIZ.Core.Applets.Services;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents a service for interacting with the application asynchronously
    /// </summary>
    [RestService("/__app")]
    public partial class ApplicationService
    {

        // App information
        private ApplicationInfo m_appInfo;

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(ApplicationService));

        /// <summary>
        /// Submits a bug report via the AMI interface
        /// </summary>
        [RestOperation(UriPath = "/bug", Method = "POST", FaultProvider = nameof(ApplicationServiceFault))]
        [Demand(PolicyIdentifiers.Login)]
        [return: RestMessage(RestMessageFormat.Json)]
        public DiagnosticReport PostBugReport([RestMessage(RestMessageFormat.Json)] BugReport report)
        {
            report.ApplicationInfo = new ApplicationInfo(false);

            if (report.IncludeData)
            {
                var logConfig = ApplicationContext.Current.Configuration.GetSection<DiagnosticsConfigurationSection>().TraceWriter.FirstOrDefault(o => o.TraceWriter.GetType() == typeof(FileTraceWriter));

                using (MemoryStream ms = new MemoryStream())
                {
                    using(GZipStream gz = new GZipStream(ms, CompressionMode.Compress))
                        ApplicationContext.Current.Configuration.Save(gz);

                    report.Attachments = new List<DiagnosticAttachmentInfo>() {
                        this.CreateGZipLogAttachment(logConfig.InitializationData + ".log"),
                        this.CreateGZipLogAttachment(logConfig.InitializationData + ".log.001", true),
                        new DiagnosticBinaryAttachment() { Content = ms.ToArray(), FileDescription = "OpenIZ.config.gz", FileName = "OpenIZ.config.gz", Id = "config" }
                    };

                }
            }

            // submit
            AmiServiceClient amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));
            try
            {
                return amiClient.SubmitDiagnosticReport(new DiagnosticReport()
                {
                    ApplicationInfo = (report.ApplicationInfo as ApplicationInfo)?.ToDiagnosticReport(),
                    CreatedBy = report.CreatedBy,
                    CreatedByKey = report.CreatedByKey,
                    CreationTime = DateTime.Now,
                    Attachments = report.Attachments,
                    Note = report.Note,
                    Submitter = AuthenticationContext.Current.Session.UserEntity
                });

            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error filing bug report: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Get menu information
        /// </summary>
        [RestOperation(UriPath = "/menu", Method = "GET", FaultProvider = nameof(ApplicationServiceFault))]
        [Demand(PolicyIdentifiers.Login)]
        [return: RestMessage(RestMessageFormat.Json)]
        public List<MenuInformation> GetMenus()
        {
            try
            {

                // Cannot have menus if not logged in
                if (!AuthenticationContext.Current.Principal.Identity.IsAuthenticated) return null;

                var rootMenus = ApplicationContext.Current.GetService<IAppletManagerService>().Applets.SelectMany(o => o.Menus).OrderBy(o => o.Order).ToArray();
                List<MenuInformation> retVal = new List<MenuInformation>();

                // Create menus
                foreach (var mnu in rootMenus)
                    this.ProcessMenuItem(mnu, retVal);
                retVal.RemoveAll(o => o.Action == null && o.Menu?.Count == 0);


                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error retrieving menus: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Process menu item
        /// </summary>
        private void ProcessMenuItem(AppletMenu menu, List<MenuInformation> retVal)
        {
            // TODO: Demand permission
            if (menu.Asset != null &&
                !ApplicationContext.Current.GetService<IAppletManagerService>().Applets.ResolveAsset(menu.Asset, menu.Manifest.Assets[0])?.Policies?.Any(p => ApplicationContext.Current.PolicyDecisionService.GetPolicyOutcome(AuthenticationContext.Current.Principal, p) == OpenIZ.Core.Model.Security.PolicyGrantType.Deny) == false)
                return;

            // Get text for menu item
            string menuText = menu.GetText(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            var existing = retVal.Find(o => o.Text == menuText && o.Icon == menu.Icon);
            if (existing == null)
            {
                existing = new MenuInformation()
                {
                    Action = menu.Launch,
                    Icon = menu.Icon,
                    Text = menuText
                };
                retVal.Add(existing);
            }
            if (menu.Menus.Count > 0)
            {
                existing.Menu = new List<MenuInformation>();
                foreach (var child in menu.Menus)
                    this.ProcessMenuItem(child, existing.Menu);
            }
        }


        /// <summary>
        /// Handle a fault
        /// </summary>
        public ErrorResult ApplicationServiceFault(Exception e)
        {
            return new ErrorResult()
            {
                Error = e.Message,
                ErrorDescription = e.InnerException?.Message,
                ErrorType = e.GetType().Name
            };
        }

        /// <summary>
        /// Get the specified log file
        /// </summary>
        [RestOperation(UriPath = "/log", Method = "GET", FaultProvider = nameof(ApplicationServiceFault))]
        [Demand(PolicyIdentifiers.Login)]
        public List<DiagnosticTextAttachment> GetLogs()
        {
            var query = MiniImsServer.CurrentContext.Request.QueryString;
            if (query["_id"] != null)
            {
                return new List<DiagnosticTextAttachment>() { this.CreateLogAttachment(query["_id"]) };
            }
            else
            {
                var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log");
                List<DiagnosticTextAttachment> retVal = new List<DiagnosticTextAttachment>();
                foreach (var itm in Directory.GetFiles(logDir))
                {
                    var logFile = new FileInfo(itm);
                    retVal.Add(new DiagnosticTextAttachment()
                    {
                        Id = Path.GetFileName(itm),
                        FileDescription = Path.GetFileName(itm),
                        FileName = logFile.FullName,
                        LastWriteDate = logFile.LastWriteTime,
                        FileSize = logFile.Length
                    });
                }
                return retVal;
            }

        }

        /// <summary>
        /// Create full log information
        /// </summary>
        private DiagnosticTextAttachment CreateLogAttachment(string logFileId)
        {
            var logFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log", logFileId);
            if (!File.Exists(logFileName))
                return null;
            var logFile = new FileInfo(logFileName);
            DiagnosticTextAttachment retVal = new DiagnosticTextAttachment()
            {
                Id = Path.GetFileName(logFileName),
                FileDescription = Path.GetFileName(logFileName),
                FileName = logFile.FullName,
                LastWriteDate = logFile.LastWriteTime,
                FileSize = logFile.Length,
                Content = File.ReadAllText(logFileName)
            };
            return retVal;
        }

        /// <summary>
        /// Create full log information
        /// </summary>
        private DiagnosticBinaryAttachment CreateGZipLogAttachment(string logFileId, bool truncate = false)
        {
            var logFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log", logFileId);
            if (!File.Exists(logFileName))
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(ms, CompressionMode.Compress))
                {
                    using (FileStream fs = File.OpenRead(logFileName))
                    {
                        if (truncate && fs.Length > 8096)
                            fs.Seek(fs.Length - 8096, SeekOrigin.Begin);
                        int br = 8096;
                        byte[] buffer = new byte[8096];
                        while (br == 8096)
                        {
                            br = fs.Read(buffer, 0, 8096);
                            gz.Write(buffer, 0, br);
                        }
                    }
                }
                var logFile = new FileInfo(logFileName);
                DiagnosticBinaryAttachment retVal = new DiagnosticBinaryAttachment()
                {
                    Id = Path.GetFileName(logFileName),
                    FileDescription = Path.GetFileName(logFileName) + ".gz",
                    FileName = logFile.FullName + ".gz",
                    LastWriteDate = logFile.LastWriteTime,
                    FileSize = logFile.Length,
                    Content = ms.ToArray()
                };
                return retVal;
            }
        }

        /// <summary>
        /// Get the alerts from the service
        /// </summary>
        [RestOperation(UriPath = "/info", Method = "GET")]
        public ApplicationInfo GetInfo()
        {
            try
            {
                return new ApplicationInfo(false);
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
        [RestOperation(UriPath = "/health", Method = "GET")]
        public ApplicationHealthInfo GetHealth()
        {
            try
            {
                var thdp = ApplicationContext.Current.GetService<OpenIZThreadPool>();
                if (thdp == null) return null;

                return new ApplicationHealthInfo()
                {
                    Concurrency = thdp.Concurrency,
                    Threads = thdp.Threads.ToArray(),
                    Active = thdp.ActiveThreads,
                    WaitState = thdp.WaitingThreads,
                    Timers = thdp.ActiveTimers,
                    NonQueued = thdp.NonQueueThreads,
                    Utilization = String.Format("{0:#0}%", (thdp.ActiveThreads / (float)thdp.Concurrency) * 100)
                };
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
        [RestOperation(UriPath = "/info.max", Method = "GET")]
        public ApplicationInfo GetInfoMax()
        {
            try
            {
                return new ApplicationInfo(true);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Could not retrieve app info {0}...", e);
                throw;
            }
        }

        /// <summary>
        /// Perform an update operation
        /// </summary>
        [RestOperation(Method = "POST", UriPath = "/update", FaultProvider = nameof(ApplicationServiceFault))]
        [Demand(PolicyIdentifiers.UnrestrictedAdministration)]
        public void DoUpdate(JObject appObject)
        {

            if (appObject["appId"] == null)
                throw new InvalidOperationException("Missing application id");

            var appId = appObject["appId"].Value<String>();
            // Update
            ApplicationContext.Current.GetService<IUpdateManager>().Install(appId);
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
            catch (Exception e)
            {
                this.m_tracer.TraceError("Could not retrieve alerts {0}...", e);
                throw;
            }
        }

        /// <summary>
        /// Get the alerts from the service
        /// </summary>
        [RestOperation(UriPath = "/alerts", Method = "POST")]
        public AlertMessage SaveAlert([RestMessage(RestMessageFormat.SimpleJson)]AlertMessage alert)
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