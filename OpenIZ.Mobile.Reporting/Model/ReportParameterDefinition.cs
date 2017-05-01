using Newtonsoft.Json;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Report parameter types
    /// </summary>
    [XmlType(nameof(ReportPropertyType), Namespace = "http://openiz.org/mobile/reporting")]
    public enum ReportPropertyType
    {
        /// <summary>
        /// Integer
        /// </summary>
        [XmlEnum("int")]
        Integer, 
        [XmlEnum("string")]
        String,
        [XmlEnum("date")]
        Date,
        [XmlEnum("dateTime")]
        DateTime,
        [XmlEnum("uuid")]
        Uuid,
        [XmlEnum("decimal")]
        Decimal,
        [XmlEnum("bytea")]
        ByteArray
    }

    /// <summary>
    /// Report parameter definition
    /// </summary>
    [XmlType(nameof(ReportParameterDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportParameterDefinition))]
    public class ReportParameterDefinition : ReportPropertyDefinition
    {
        
        /// <summary>
        /// Gets or sets the label
        /// </summary>
        [XmlAttribute("label"), JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Value set
        /// </summary>
        [XmlElement("valueSet"), JsonProperty("valueSet")]
        public ReportDatasetDefinition ValueSet { get; set; }

        /// <summary>
        /// Gets or sets the required
        /// </summary>
        [XmlAttribute("required"), JsonProperty("required")]
        public bool Required { get; set; }
    }
}