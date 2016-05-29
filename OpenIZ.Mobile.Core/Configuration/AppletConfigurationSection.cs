using System;
using SQLite;
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

