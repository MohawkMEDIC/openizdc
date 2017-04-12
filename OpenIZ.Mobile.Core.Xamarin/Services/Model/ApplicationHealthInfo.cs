using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Model
{
    /// <summary>
    /// Represent health information
    /// </summary>
    [JsonObject("ApplicationHealthInfo")]
    public class ApplicationHealthInfo
    {
        /// <summary>
        /// Concurrency of the pool
        /// </summary>
        [JsonProperty("concurrency")]
        public int Concurrency { get; set; }

        /// <summary>
        /// Active threads
        /// </summary>
        [JsonProperty("active")]
        public int Active { get; set; }

        /// <summary>
        /// Get  the threads
        /// </summary>
        public String[] Threads { get; set; }

        /// <summary>
        /// Wait state threads
        /// </summary>
        [JsonProperty("wait")]
        public int WaitState { get; set; }

        /// <summary>
        /// Timer threads
        /// </summary>
        [JsonProperty("timer")]
        public int Timers { get; set; }

        /// <summary>
        /// Nonqueued threads
        /// </summary>
        [JsonProperty("nonq")]
        public int NonQueued { get; set; }
    }
}
