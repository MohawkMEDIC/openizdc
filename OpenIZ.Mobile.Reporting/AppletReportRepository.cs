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
 * Date: 2017-6-28
 */
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
