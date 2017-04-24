using OpenIZ.Mobile.Reporting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Reporting
{
    /// <summary>
    /// Report repository
    /// </summary>
    public interface IReportRepository
    {

        /// <summary>
        /// Gets report description
        /// </summary>
        IEnumerable<ReportDescriptionDefinition> Reports { get; }

        /// <summary>
        /// Get report
        /// </summary>
        ReportDefinition GetReport(String name);

    }
}
