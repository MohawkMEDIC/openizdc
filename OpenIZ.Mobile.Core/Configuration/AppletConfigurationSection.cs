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
using OpenIZ.Core.Applets.Model;

namespace OpenIZ.Mobile.Core.Configuration
{

	/// <summary>
	/// Represents configuration related to applets
	/// </summary>
	[JsonObject, XmlType(nameof(AppletConfigurationSection), Namespace = "http://openiz.org/mobile/configuration")]
	public class AppletConfigurationSection : IConfigurationSection
	{
		/// <summary>
		/// Coniguration section
		/// </summary>
		public AppletConfigurationSection ()
		{
			this.AppletConfiguration = new List<AppletConfiguration> ();
			this.AppletGroupOrder = new List<string> ();
			this.Applets = new List<AppletName> ();
		}

        /// <summary>
        /// Gets or sets the applet which is used for authentication requests
        /// </summary>
        [XmlElement("startup")]
        public String StartupAsset { get; set; }

        /// <summary>
        /// Gets or sets the asset which is used for authentication
        /// </summary>
        [XmlElement("login")]
        public String AuthenticationAsset { get; set; }

        /// <summary>
        /// Gets or sets the directory where applets are stored
        /// </summary>
        /// <value>The applet directory.</value>
        [XmlAttribute("appletDirectory"), JsonIgnore]
		public String AppletDirectory {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets configuration data specific to a particular applet
		/// </summary>
		/// <remarks>This property is used to store user preferences for a particular applet</remarks>
		[XmlElement("appletConfig"), JsonProperty("config")]
		public List<AppletConfiguration> AppletConfiguration {
			get;
			set;
		}

		/// <summary>
		/// Identifies the applet group orders
		/// </summary>
		/// <value>The applet group order.</value>
		[XmlElement("appletGroup")]
		public List<String> AppletGroupOrder {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a list of applets which are permitted for use and their 
		/// public key token used for validation
		/// </summary>
		/// <value>The applets.</value>
		[XmlElement("applet"), JsonProperty("applet")]
		public List<AppletName> Applets {
			get;
			set;
		}

        /// <summary>
        /// Auto-update applet
        /// </summary>
        [XmlElement("autoUpdate"), JsonProperty("autoUpdate")]
        public bool AutoUpdateApplets { get; set; }
    }

	/// <summary>
	/// Represents a configuration of an applet
	/// </summary>
	[JsonObject, XmlType(nameof(AppletConfiguration), Namespace = "http://openiz.org/mobile/configuration")]
	public class AppletConfiguration
	{

		/// <summary>
		/// Gets or sets the applet id
		/// </summary>
		[XmlAttribute("applet"), JsonProperty("applet")]
		public String AppletId {
			get;
			set;
		}

		/// <summary>
		/// Applet configuration entry
		/// </summary>
		[XmlElement("appSetting"), JsonProperty("appSetting")]
		public List<AppletConfigurationEntry> AppSettings {
			get;
			set;
		}

	}

	/// <summary>
	/// Applet configuration entry
	/// </summary>
	[JsonObject, XmlType(nameof(AppletConfigurationEntry), Namespace = "http://openiz.org/mobile/configuration")]
	public class AppletConfigurationEntry
	{
		/// <summary>
		/// The name of the property
		/// </summary>
		/// <value>The name.</value>
		[XmlAttribute("name"), JsonProperty("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[XmlAttribute("value"), JsonProperty("value")]
		public String Value {
			get;
			set;
		}
	}

}

