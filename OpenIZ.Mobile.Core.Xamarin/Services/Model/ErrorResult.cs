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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using OpenIZ.Core.Exceptions;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Model
{
	/// <summary>
	/// General IMSI error result
	/// </summary>
	[JsonObject]
	public class ErrorResult
	{
        public ErrorResult()
        {

        }

        /// <summary>
        /// Create error result from the specified excepion
        /// </summary>
        /// <param name="e"></param>
        public ErrorResult(Exception e)
        {
            Error = e.Message;
            ErrorType = e.GetType().Name;
            if (e.InnerException != null)
                InnerError = new ErrorResult(e.InnerException);
            if(e is DetectedIssueException)
            {
                this.ErrorDescription = String.Join(";", (e as DetectedIssueException).Issues.Select(o => o.Text));
            }
        }

        [JsonProperty("type")]
        public String ErrorType { get; set; }
        [JsonProperty("error")]
		public String Error { get; set; }
		[JsonProperty("error_description")]
		public String ErrorDescription { get; set; }

        [JsonProperty("caused_by")]
        public ErrorResult InnerError { get; set; }
    }
}