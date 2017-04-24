using Newtonsoft.Json;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Represents report SQL definition
    /// </summary>
    [XmlType(nameof(ReportSqlDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportSqlDefinition))]
    public class ReportSqlDefinition
    {

        /// <summary>
        /// Provider for this sql
        /// </summary>
        [XmlAttribute("provider"), JsonProperty("provider")]
        public string Provider { get; set; }

        /// <summary>
        /// Text of the sql
        /// </summary>
        [XmlText, JsonProperty("sql")]
        public string Sql { get; set; }
    }
}