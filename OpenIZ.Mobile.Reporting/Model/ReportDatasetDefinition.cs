using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Represents a dataset definition
    /// </summary>
    [XmlType(nameof(ReportDatasetDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportDatasetDefinition))]
    public class ReportDatasetDefinition
    {

        /// <summary>
        /// Gets or sets the name of the dataset
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Property information
        /// </summary>
        [XmlElement("property"), JsonProperty("property")]
        public List<ReportPropertyDefinition> Properties { get; set; }

        /// <summary>
        /// Gets or sets the pivot definition
        /// </summary>
        [XmlElement("pivot"), JsonProperty("pivot")]
        public ReportPivotDefinition Pivot { get; set; }

        /// <summary>
        /// Gets or sets the sql
        /// </summary>
        [XmlElement("sql"), JsonProperty("sql")]
        public List<ReportSqlDefinition> Sql { get; set; }
    }
}