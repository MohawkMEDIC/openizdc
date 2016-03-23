using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Applets
{
	/// <summary>
	/// Represents an applet asset
	/// </summary>
	[XmlType(nameof(AppletAsset), Namespace = "http://openiz.org/mobile/applet")]
	public class AppletAsset
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Applets.AppletAsset"/> class.
		/// </summary>
		public AppletAsset ()
		{
			this.Reference = new List<string>();
		}

		/// <summary>
		/// Gets or sets the name of the asset
		/// </summary>
		[XmlAttribute("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Language
		/// </summary>
		[XmlAttribute("lang")]
		public String Language {
			get;
			set;
		}

		/// <summary>
		/// Mime type
		/// </summary>
		[XmlAttribute("mimeType")]
		public String MimeType {
			get;
			set;
		}

		/// <summary>
		/// References
		/// </summary>
		/// <value>The reference.</value>
		[XmlElement("reference")]
		public List<String> Reference {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the content of the asset
		/// </summary>
		[XmlElement("contentText", Type = typeof(String))]
		[XmlElement("contentBin", Type = typeof(byte[]))]
		[XmlElement("contentXml", Type = typeof(XElement))]
		public Object Content {
			get;
			set;
		}


	}

}

