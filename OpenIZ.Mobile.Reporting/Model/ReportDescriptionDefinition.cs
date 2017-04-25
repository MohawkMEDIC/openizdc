using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Identifies a report
    /// </summary>
    [XmlType(nameof(ReportDescriptionDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportDescriptionDefinition))]
    public class ReportDescriptionDefinition
    {

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlAttribute("title"), JsonProperty("title")]
        public String Title { get; set; }

        /// <summary>
        /// Aurhors
        /// </summary>
        [XmlElement("author"), JsonProperty("authors")]
        public List<string> Authors { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public string Description { get; set; }
    }
}