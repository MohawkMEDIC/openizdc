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
 * Date: 2017-6-28
 */
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Formatting control
    /// </summary>
    [XmlType(nameof(ReportFormatDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportFormatDefinition))]
    public class ReportFormatDefinition
    {
        /// <summary>
        /// Gets the format specifier for date
        /// </summary>
        [XmlElement("date"), JsonProperty("date")]
        public String Date { get; set; }

        /// <summary>
        /// Date time formatting
        /// </summary>
        [XmlElement("dateTime"), JsonProperty("dateTime")]
        public string DateTime { get; set; }
    }
}