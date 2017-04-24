using OpenIZ.Mobile.Reporting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Reporting
{
    /// <summary>
    /// Represents a report datasource
    /// </summary>
    public interface IReportDatasource
    {

        /// <summary>
        /// Gets the name of the report datasource
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Execute dataset
        /// </summary>
        IEnumerable<dynamic> ExecuteDataset(String connectionString, String sql, List<Object> sqlParms);
    }
}
