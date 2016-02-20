using System;
using SQLite;
using System.Xml.Serialization;
using OpenIZ.Mobile.Core.Applets;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Configuration
{


	/// <summary>
	/// Configuration table object
	/// </summary>
	[XmlRoot(nameof(OpenIZConfiguration), Namespace = "http://openiz.org/mobile/configuration")]
	[XmlType(nameof(OpenIZConfiguration), Namespace = "http://openiz.org/mobile/configuration")]
	public class OpenIZConfiguration
	{
		
		/// <summary>
		/// Application database file
		/// </summary>
		[XmlElement("dbFile")]
		public String AppDataFile {
			get;
			set;
		}

		/// <summary>
		/// The directory where applets are stored
		/// </summary>
		[XmlElement("appletDir")]
		public String AppletDir {
			get;
			set;
		}

		/// <summary>
		/// Gets the thumbprint the device should use for authentication
		/// </summary>
		[XmlElement("deviceKey")]
		public String DeviceKeyThumbprint
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the real/domain to which the application is currently joined
		/// </summary>
		[XmlElement("realm")]
		public String Realm {
			get;
			set;
		}

		/// <summary>
		/// Access control service URL
		/// </summary>
		[XmlElement("acsUrl")]
		public String AccessControlServiceUrl {
			get;
			set;
		}

		/// <summary>
		/// Access control service URL
		/// </summary>
		[XmlElement("dataExchangeUrl")]
		public String DataExchangeUrl {
			get;
			set;
		}

		/// <summary>
		/// The list of applets configured and/or detected
		/// </summary>
		public List<AppletManifest> Applets {
			get;
			set;
		}
	}
}

