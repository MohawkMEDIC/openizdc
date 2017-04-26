using Newtonsoft.Json;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Mobile pivot definition
    /// </summary>
    [XmlType(nameof(ReportPivotDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    public class ReportPivotDefinition
    {
        /// <summary>
        /// Gets or sets the columns
        /// </summary>
        [XmlAttribute("column"), JsonProperty("column")]
        public string Columns { get; set; }

        /// <summary>
        /// Gets or sets the key column
        /// </summary>
        [XmlAttribute("key"), JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value columns
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public string Value { get; set; }
    }
}