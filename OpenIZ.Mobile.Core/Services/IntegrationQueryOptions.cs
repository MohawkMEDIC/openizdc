/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2017-3-31
 */
using OpenIZ.Core.Http;
using OpenIZ.Core.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
	/// <summary>
	/// Query options to control data coming back from the server
	/// </summary>
	public class IntegrationQueryOptions
	{
		/// <summary>
		/// Gets or sets the If-Modified-Since header
		/// </summary>
		public DateTime? IfModifiedSince { get; set; }

		/// <summary>
		/// Gets or sets the If-None-Match
		/// </summary>
		public String IfNoneMatch { get; set; }

		/// <summary>
		/// Gets or sets the timeout
		/// </summary>
		public int? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the infrastructure options
        /// </summary>
        public NameValueCollection InfrastructureOptions { get; set; }

        /// <summary>
        /// Lean
        /// </summary>
        public bool Lean { get; set; }

        /// <summary>
        /// Generates an event hander for the integration options
        /// </summary>
        public static EventHandler<RestRequestEventArgs> CreateRequestingHandler(IntegrationQueryOptions options)
        {
            return (o, e) =>
            {
                if (options == null) return;
                else if (options?.IfModifiedSince.HasValue == true)
                    e.AdditionalHeaders[HttpRequestHeader.IfModifiedSince] = options?.IfModifiedSince.Value.ToString();
                else if (!String.IsNullOrEmpty(options?.IfNoneMatch))
                    e.AdditionalHeaders[HttpRequestHeader.IfNoneMatch] = options?.IfNoneMatch;
                if (options?.Lean == true)
                    e.Query.Add("_lean", "true");
                if (options?.InfrastructureOptions?.Count > 0)
                    foreach (var inf in options?.InfrastructureOptions)
                        e.Query.Add(inf.Key, inf.Value);
            };
        }
    }
}
