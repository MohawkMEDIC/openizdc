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
 * User: justi
 * Date: 2016-6-14
 */
using System;
using SQLite.Net;
using System.Xml.Serialization;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Configuration.Data;
using System.IO;
using Newtonsoft.Json;

namespace OpenIZ.Mobile.Core.Configuration
{

	/// <summary>
	/// Security configuration section
	/// </summary>
	[XmlType(nameof(SecurityConfigurationSection), Namespace = "http://openiz.org/mobile/configuration"), JsonObject(nameof(SecurityConfigurationSection))]
	public class SecurityConfigurationSection : IConfigurationSection
	{


		/// <summary>
		/// Gets the real/domain to which the application is currently joined
		/// </summary>
		[XmlElement("domain"), JsonProperty("domain")]
		public String Domain {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the allowed token type
		/// </summary>
		/// <value>The type of the token.</value>
		[XmlElement("tokenType")]
		public String TokenType {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the token algorithms.
		/// </summary>
		/// <value>The token algorithms.</value>
		[XmlElement("tokenAlg")]
		public List<String> TokenAlgorithms {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the token symmetric secrets.
		/// </summary>
		/// <value>The token symmetric secrets.</value>
		[XmlElement("secret")]
		public List<Byte[]> TokenSymmetricSecrets
		{
			get;set;
		}

		/// <summary>
		/// Gets or sets the configured device name
		/// </summary>
		/// <value>The name of the device.</value>
		[XmlElement("deviceName"), JsonProperty("deviceName")]
		public String DeviceName {
			get;
			set;
		}

		/// <summary>
		/// Sets the device secret.
		/// </summary>
		/// <value>The device secret.</value>
		[XmlElement("deviceSecret")]
		public String DeviceSecret {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets teh device certificate
        /// </summary>
        [XmlElement("deviceCertificate")]
        public ServiceCertificateConfiguration DeviceCertificate { get; set; }
    }

}

