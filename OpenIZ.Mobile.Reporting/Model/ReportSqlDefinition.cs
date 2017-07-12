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
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Represents report SQL definition
    /// </summary>
    [XmlType(nameof(ReportSqlDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportSqlDefinition))]
    public class ReportSqlDefinition
    {

        /// <summary>
        /// Provider for this sql
        /// </summary>
        [XmlAttribute("provider"), JsonProperty("provider")]
        public string Provider { get; set; }

        /// <summary>
        /// Text of the sql
        /// </summary>
        [XmlText, JsonProperty("sql")]
        public string Sql { get; set; }
    }
}