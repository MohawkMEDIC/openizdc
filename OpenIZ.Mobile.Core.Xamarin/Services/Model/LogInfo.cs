using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Model
{
    /// <summary>
    /// Represents meta-data about a particular log
    /// </summary>
    [JsonObject("LogInfo"), XmlType(nameof(LogInfo), Namespace = "http://openiz.org/model/mobile")]
    public class LogInfo : RuntimeFileInfo
    {

        /// <summary>
        /// Gets or sets the identiifer
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public String Id { get; set; }

        /// <summary>
        /// The content of the log file
        /// </summary>
        [XmlText, JsonProperty("text")]
        public String Content { get; set; }

    }
}
