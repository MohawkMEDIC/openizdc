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
        /// Max local session
        /// </summary>
        public SecurityConfigurationSection()
        {
            this.MaxLocalSession = new TimeSpan(0, 30, 0);
        }

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
		[XmlElement("tokenType"), JsonIgnore]
		public String TokenType {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the token algorithms.
		/// </summary>
		/// <value>The token algorithms.</value>
		[XmlElement("tokenAlg"), JsonIgnore]
		public List<String> TokenAlgorithms {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the token symmetric secrets.
		/// </summary>
		/// <value>The token symmetric secrets.</value>
		[XmlElement("secret"), JsonIgnore]
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
		[XmlElement("deviceSecret"), JsonIgnore]
		public String DeviceSecret {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets teh device certificate
        /// </summary>
        [XmlElement("deviceCertificate"), JsonIgnore]
        public ServiceCertificateConfiguration DeviceCertificate { get; set; }

        /// <summary>
        /// Audit retention
        /// </summary>
        [XmlElement("auditRetention"), JsonProperty("auditRetention")]
        public String AuditRetentionXml
        {
            get
            {
                return this.AuditRetention.ToString();
            }
            set
            {
                this.AuditRetention = TimeSpan.Parse(value);
            }
        }

        /// <summary>
        /// Audit retention
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public TimeSpan AuditRetention { get; set; }

        /// <summary>
        /// When true, only allow login from subscribed facilities
        /// </summary>
        [XmlElement("onlySubscribedAuth"), JsonProperty("onlySubscribedAuth")]
        public bool OnlySubscribedFacilities { get; set; }

        /// <summary>
        /// Local session length
        /// </summary>
        [XmlElement("localSessionLength"), JsonProperty("localSessionLength")]
        public String MaxLocalSessionXml
        {
            get
            {
                return this.MaxLocalSession.ToString();
            }
            set
            {
                this.MaxLocalSession = TimeSpan.Parse(!String.IsNullOrEmpty(value) ? value : "00:30:00");
            }
        }

        /// <summary>
        /// Local session
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public TimeSpan MaxLocalSession { get; set; }

        /// <summary>
        /// Maximum invalid logins
        /// </summary>
        [XmlElement("maxInvalidLogins"), JsonProperty("maxInvalidLogins")]
        public int? MaxInvalidLogins { get; set; }
    }

}

