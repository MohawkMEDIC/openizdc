using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Mobile.Reporting.Model;
using OpenIZ.Core;
using OpenIZ.Core.Applets.Services;
using System.Xml.Serialization;
using System.IO;

namespace OpenIZ.Mobile.Reporting
{
    /// <summary>
    /// Represents a simple report repository
    /// </summary>
    public class AppletReportRepository : IReportRepository
    {

        // Reports
        private List<ReportDefinition> m_reports = null;

        /// <summary>
        /// Get all reports in this repository
        /// </summary>
        public IEnumerable<ReportDescriptionDefinition> Reports
        {
            get
            {
                return this.GetReports().Select(o => o.Description);
            }
        }

        /// <summary>
        /// Get reports
        /// </summary>
        private IEnumerable<ReportDefinition> GetReports()
        {
            if (this.m_reports == null)
            {
                XmlSerializer xsz = new XmlSerializer(typeof(ReportDefinition));
                var appletService = ApplicationServiceContext.Current.GetService(typeof(IAppletManagerService)) as IAppletManagerService;
                var candidates = appletService.Applets.SelectMany(o => o.Assets).Where(o => o.Name.StartsWith("reports"));

                if (appletService.Applets.CachePages)
                {
                    this.m_reports = new List<ReportDefinition>();
                    foreach (var c in candidates)
                    {
                        using (var ms = new MemoryStream(appletService.Applets.RenderAssetContent(c)))
                            this.m_reports.Add(xsz.Deserialize(ms) as ReportDefinition);
                    }
                }
                else
                {
                    var retVal = new List<ReportDefinition>();
                    foreach (var c in candidates)
                    {
                        using (var ms = new MemoryStream(appletService.Applets.RenderAssetContent(c)))
                            retVal.Add(xsz.Deserialize(ms) as ReportDefinition);
                    }
                    return retVal;
                }
            }
            return this.m_reports;
        }


        /// <summary>
        /// Get report
        /// </summary>
        public ReportDefinition GetReport(string name)
        {
            return this.GetReports().FirstOrDefault(o => o.Description?.Name == name);
        }
    }
}
