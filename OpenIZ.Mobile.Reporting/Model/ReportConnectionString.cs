using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Report connection string
    /// </summary>
    [XmlType(nameof(ReportConnectionString), Namespace = "http://openiz.org/mobile/reporting")]
    public class ReportConnectionString
    {

        /// <summary>
        /// Identifier
        /// </summary>
        [XmlAttribute("id")]
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sts the value of the connection
        /// </summary>
        [XmlText]
        public string Value { get; set; }
    }
}