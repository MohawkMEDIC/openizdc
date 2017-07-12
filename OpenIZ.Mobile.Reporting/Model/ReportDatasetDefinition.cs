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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Represents a dataset definition
    /// </summary>
    [XmlType(nameof(ReportDatasetDefinition), Namespace = "http://openiz.org/mobile/reporting")]
    [JsonObject(nameof(ReportDatasetDefinition))]
    public class ReportDatasetDefinition
    {

        /// <summary>
        /// Gets or sets the name of the dataset
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Property information
        /// </summary>
        [XmlElement("property"), JsonProperty("property")]
        public List<ReportPropertyDefinition> Properties { get; set; }

        /// <summary>
        /// Gets or sets the pivot definition
        /// </summary>
        [XmlElement("pivot"), JsonProperty("pivot")]
        public ReportPivotDefinition Pivot { get; set; }

        /// <summary>
        /// Gets or sets the sql
        /// </summary>
        [XmlElement("sql"), JsonProperty("sql")]
        public List<ReportSqlDefinition> Sql { get; set; }
    }
}