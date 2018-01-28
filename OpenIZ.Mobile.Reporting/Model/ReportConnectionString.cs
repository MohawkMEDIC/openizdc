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
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Reporting.Model
{
    /// <summary>
    /// Report connection string
    /// </summary>
    [XmlType(nameof(ReportConnectionString), Namespace = "http://openiz.org/mobile/reporting")]
    public class ReportConnectionString
    {

        /// <summary>
        /// Identifier
        /// </summary>
        [XmlAttribute("id")]
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sts the value of the connection
        /// </summary>
        [XmlText]
        public string Value { get; set; }
    }
}