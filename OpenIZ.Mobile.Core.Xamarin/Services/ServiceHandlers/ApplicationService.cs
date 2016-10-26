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

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents a service for interacting with the application asynchronously
    /// </summary>
    [RestService("/__app")]
    public class ApplicationService
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
            report.ApplicationInfo = new ApplicationInfo();

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

                var rootMenus = XamarinApplicationContext.Current.LoadedApplets.SelectMany(o => o.Menus).OrderBy(o => o.Order).ToArray();
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
            if (menu.Launcher != null &&
                !XamarinApplicationContext.Current.LoadedApplets.ResolveAsset(menu.Launcher, menu.Manifest.Assets[0])?.Policies?.Any(p => ApplicationContext.Current.PolicyDecisionService.GetPolicyOutcome(AuthenticationContext.Current.Principal, p) == OpenIZ.Core.Model.Security.PolicyGrantType.Deny) == false)
                return;

            // Get text for menu item
            string menuText = menu.GetText(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            var existing = retVal.Find(o => o.Text == menuText && o.Icon == menu.Icon);
            if (existing == null)
            {
                existing = new MenuInformation()
                {
                    Action = menu.Launcher,
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