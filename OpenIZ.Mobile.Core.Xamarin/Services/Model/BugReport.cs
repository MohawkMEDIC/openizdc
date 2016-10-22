using Newtonsoft.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Model
{
    /// <summary>
    /// Represents a bug report
    /// </summary>
    [JsonObject("BugReport"), XmlType(nameof(BugReport), Namespace = "http://openiz.org/model/mobile")]
    public class BugReport : BaseEntityData
    {

        /// <summary>
        /// Gets or sets the note
        /// </summary>
        [XmlText, JsonProperty("note")]
        public String Note { get; set; }

        /// <summary>
        /// Represents the submitter
        /// </summary>
        [XmlIgnore, JsonProperty("submitter")]
        public UserEntity Submitter { get; set; }

        /// <summary>
        /// Represents the most recent logs for the bug report
        /// </summary>
        [XmlElement("log"), JsonProperty("log")]
        public List<LogInfo> Log { get; set; }


    }
}
