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
    /// Represents basic application configuration
    /// </summary>
    [XmlType(nameof(ApplicationConfigurationSection), Namespace = "http://openiz.org/mobile/configuration"), JsonObject]
    public class ApplicationConfigurationSection : IConfigurationSection
    {

        // Services
        private List<Object> m_services;


        /// <summary>
        /// The location of the directory where user preferences are stored
        /// </summary>
        /// <value>The user preference dir.</value>
        [XmlElement("userPrefDir"), JsonIgnore]
        public String UserPrefDir {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        [XmlElement("style"), JsonProperty("style")]
        public StyleSchemeType Style {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        /// <value>The services.</value>
        [XmlElement("service"), JsonProperty("service")]
        public List<String> ServiceTypes {
            get;
            set;
        }


        /// <summary>
        /// General extended application settings
        /// </summary>
        [XmlElement("setting"), JsonProperty("setting")]
        public List<AppSettingKeyValuePair> AppSettings {
            get;
            set;
        }

		/// <summary>
		/// Sets the services.
		/// </summary>
		/// <value>The services.</value>
		[XmlIgnore, JsonIgnore]
		public List<Object> Services {
			get {
				if (this.m_services == null) {
					this.m_services = new List<object> ();
					foreach (var itm in this.ServiceTypes) {
						Type t = Type.GetType (itm);
						this.m_services.Add (Activator.CreateInstance (t));
					}
				}
				return this.m_services;
			}
		}

        /// <summary>
        /// Gets or sets the cache configuration
        /// </summary>
        [XmlElement("caching")]
        public CacheConfiguration Cache { get; set; }
    }

    /// <summary>
    /// Cache configuration
    /// </summary>
    [XmlType(nameof(CacheConfiguration), Namespace = "http://openiz.org/mobile/configuration"), JsonObject]
    public class CacheConfiguration
    {
        /// <summary>
        /// Maximum size
        /// </summary>
        [XmlAttribute("maxSize")]
        public int MaxSize { get; set; }

        /// <summary>
        /// Max age
        /// </summary>
        [XmlAttribute("maxAge")]
        public long MaxAge { get; set; }
        /// <summary>
        /// Maximum time that can pass without cleaning
        /// </summary>
        [XmlAttribute("maxDirty")]
        public long MaxDirtyAge { get; set; }
        /// <summary>
        /// Maximum time that can pass withut reducing pressure
        /// </summary>
        [XmlAttribute("maxPressure")]
        public long MaxPressureAge { get; set; }
    }

    /// <summary>
    /// Application key/value pair setting
    /// </summary>
    [XmlType(nameof(AppSettingKeyValuePair), Namespace = "http://openiz.org/mobile/configuration")]
    public class AppSettingKeyValuePair
    {

        /// <summary>
        /// The key of the setting
        /// </summary>
        [XmlAttribute("key"), JsonProperty("key")]
        public String Key { get; set; }

        /// <summary>
        /// The value of the setting
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public String Value { get; set; }

    }

    /// <summary>
    /// Style scheme type
    /// </summary>
    [XmlType(nameof(StyleSchemeType), Namespace = "http://openiz.org/mobile/configuration")]
	public enum StyleSchemeType
	{
		Dark,
		Light
	}

}

