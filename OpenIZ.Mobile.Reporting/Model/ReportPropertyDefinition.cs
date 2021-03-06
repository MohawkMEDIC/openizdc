﻿/*
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
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Represents a report property definition
    /// </summary>
    [XmlType(nameof(ReportPropertyDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportPropertyDefinition))]
    public class ReportPropertyDefinition
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets type
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public ReportPropertyType Type { get; set; }

    }
}