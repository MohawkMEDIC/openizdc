using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenIZ.Mobile.Core.Applets
{
	/// <summary>
	/// Applet reference
	/// </summary>
	[JsonObject, XmlType(nameof(AppletReference), Namespace = "http://openiz.org/mobile/applet")]
	public class AppletReference
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Applets.AppletReference"/> class.
		/// </summary>
		public AppletReference ()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Applets.AppletReference"/> class.
		/// </summary>
		public AppletReference (String id, String version, String publicKeyToken)
		{
			this.Id = id;
			this.Version = version;
			this.PublicKeyToken = publicKeyToken;
		}

		/// <summary>
		/// The identifier of the applet
		/// </summary>
		[XmlAttribute("id"), JsonProperty("id")]
		public String Id {
			get;
			set;
		}

		/// <summary>
		/// The version of the applet
		/// </summary>
		[XmlAttribute("version"), JsonProperty("version")]
		public String Version {
			get;
			set;
		}

		/// <summary>
		/// The signature of the applet (not used for verification, rather lookup)
		/// </summary>
		/// <value>The signature.</value>
		[XmlAttribute("publicKeyToken"), JsonIgnore]
		public String PublicKeyToken {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the signature which can be used to validate the file
		/// </summary>
		[XmlElement("signature")]
		public byte[] Signature {
			get;
			set;
		}

		/// <summary>
		/// A hash which validates the file has not changed on the disk. Different from signature which 
		/// indicates the manifest has not changed since installed.
		/// </summary>
		/// <value><c>true</c> if this instance hash; otherwise, <c>false</c>.</value>
		[XmlElement("hash")]
		public byte[] Hash {
			get;
			set;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("Id={0}, Version={1}, PublicKeyToken={2}", Id, Version, PublicKeyToken);
		}
	}


}

