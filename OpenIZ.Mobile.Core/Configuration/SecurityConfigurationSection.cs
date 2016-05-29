using System;
using SQLite;
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

	}

}

