/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2017-9-1
 */
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

        /// <summary>
        /// Utilization
        /// </summary>
        [JsonProperty("utilization")]
        public string Utilization { get; set; }
    }
}
