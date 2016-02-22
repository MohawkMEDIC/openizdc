using System;
using SQLite;
using System.Xml.Serialization;
using OpenIZ.Mobile.Core.Applets;
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

