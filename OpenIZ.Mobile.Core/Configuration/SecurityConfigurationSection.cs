using System;
using SQLite;
using System.Xml.Serialization;
using OpenIZ.Mobile.Core.Applets;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Configuration.Data;
using System.IO;

namespace OpenIZ.Mobile.Core.Configuration
{

	/// <summary>
	/// Security configuration section
	/// </summary>
	[XmlType(nameof(SecurityConfigurationSection), Namespace = "http://openiz.org/mobile/configuration")]
	public class SecurityConfigurationSection : IConfigurationSection
	{


		/// <summary>
		/// Gets the real/domain to which the application is currently joined
		/// </summary>
		[XmlElement("domain")]
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
	}

}

