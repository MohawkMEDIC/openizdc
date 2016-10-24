using Newtonsoft.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.AMI.Diagnostics;
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
    [JsonObject("BugReport")]
    public class BugReport : DiagnosticReport
    {

        /// <summary>
        /// Include diagnostics data
        /// </summary>
        [JsonProperty("_includeData")]
        public bool IncludeData { get; set; }

    }
}
