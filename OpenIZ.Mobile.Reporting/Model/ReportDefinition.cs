using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Report definition
    /// </summary>
    [XmlType(nameof(ReportDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [XmlRoot("Report", Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportDefinition))]
    public class ReportDefinition
    {

        // Serializer
        private static XmlSerializer s_serializer = new XmlSerializer(typeof(ReportDefinition));

        /// <summary>
        /// Load report
        /// </summary>
        public static ReportDefinition Load(Stream source)
        {
            return s_serializer.Deserialize(source) as ReportDefinition;
        }

        /// <summary>
        /// Save report
        /// </summary>
        public void Save(Stream dest)
        {
            s_serializer.Serialize(dest, this);
        }


        /// <summary>
        /// Information description
        /// </summary>
        [XmlElement("info"), JsonProperty("info")]
        public ReportDescriptionDefinition Description { get; set; }

        /// <summary>
        /// Gets or sets the connection
        /// </summary>
        [XmlElement("connection"), JsonProperty("connection")]
        public List<ReportConnectionString> ConnectionString { get; set; }

        /// <summary>
        /// Format controls
        /// </summary>
        [XmlElement("format"), JsonProperty("format")]
        public ReportFormatDefinition Formatter { get; set; }

        /// <summary>
        /// Report parameter definition
        /// </summary>
        [XmlElement("parameter"), JsonProperty("parameter")]
        public List<ReportParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Report datasets
        /// </summary>
        [XmlElement("dataset"), JsonProperty("dataset")]
        public List<ReportDatasetDefinition> Datasets { get; set; }

        /// <summary>
        /// Gets or
        /// </summary>
        [XmlElement("view"), JsonProperty("view")]
        public List<ReportViewDefinition> Views { get; set; }
    }
}
