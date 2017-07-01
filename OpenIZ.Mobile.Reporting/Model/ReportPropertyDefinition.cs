using Newtonsoft.Json;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Represents a report property definition
    /// </summary>
    [XmlType(nameof(ReportPropertyDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportPropertyDefinition))]
    public class ReportPropertyDefinition
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets type
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public ReportPropertyType Type { get; set; }

    }
}