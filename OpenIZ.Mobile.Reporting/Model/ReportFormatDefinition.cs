using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Formatting control
    /// </summary>
    [XmlType(nameof(ReportFormatDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportFormatDefinition))]
    public class ReportFormatDefinition
    {
        /// <summary>
        /// Gets the format specifier for date
        /// </summary>
        [XmlElement("date"), JsonProperty("date")]
        public IFormatProvider Date { get; internal set; }

        /// <summary>
        /// Date time formatting
        /// </summary>
        [XmlElement("dateTime"), JsonProperty("dateTime")]
        public string DateTime { get; set; }
    }
}