/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: khannan
 * Date: 2016-9-26
 */
using OpenIZ.Core.Http;
using System;
using System.Collections.Generic;
using System.Linq;
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
	}
}
