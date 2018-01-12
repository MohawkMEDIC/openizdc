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
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Report parameter types
    /// </summary>
    [XmlType(nameof(ReportPropertyType), Namespace = "http://openiz.org/mobile/reporting")]
    public enum ReportPropertyType
    {
        /// <summary>
        /// Integer
        /// </summary>
        [XmlEnum("int")]
        Integer, 
        [XmlEnum("string")]
        String,
        [XmlEnum("date")]
        Date,
        [XmlEnum("dateTime")]
        DateTime,
        [XmlEnum("uuid")]
        Uuid,
        [XmlEnum("decimal")]
        Decimal,
        [XmlEnum("bytea")]
        ByteArray
    }

    /// <summary>
    /// Report parameter definition
    /// </summary>
    [XmlType(nameof(ReportParameterDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportParameterDefinition))]
    public class ReportParameterDefinition : ReportPropertyDefinition
    {
        
        /// <summary>
        /// Gets or sets the label
        /// </summary>
        [XmlAttribute("label"), JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Value set
        /// </summary>
        [XmlElement("valueSet"), JsonProperty("valueSet")]
        public ReportDatasetDefinition ValueSet { get; set; }

        /// <summary>
        /// Gets or sets the required
        /// </summary>
        [XmlAttribute("required"), JsonProperty("required")]
        public bool Required { get; set; }

        /// <summary>
        /// When true indicates the value should be the ceiling of the entered value
        /// </summary>
        [XmlAttribute("ceil"), JsonProperty("ceil")]
        public bool Ceil { get; set; }
    }
}