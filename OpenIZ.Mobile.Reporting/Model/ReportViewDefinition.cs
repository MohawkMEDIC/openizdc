using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Represents a report view
    /// </summary>
    [XmlType(nameof(ReportViewDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportViewDefinition))]
    public class ReportViewDefinition
    {

        /// <summary>
        /// Gets or set the name
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the label
        /// </summary>
        [XmlAttribute("label"), JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Body of the view
        /// </summary>
        [XmlElement("body", Namespace = "http://www.w3.org/1999/xhtml"), JsonProperty("body")]
        public XElement Body { get; set; }
    }
}