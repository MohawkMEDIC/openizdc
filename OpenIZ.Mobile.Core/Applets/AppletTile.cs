using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Applets
{
	/// <summary>
	/// Applet tile
	/// </summary>
	[XmlType(nameof(AppletTile), Namespace = "http://openiz.org/mobile/applet")]
	public class AppletTile
	{

		/// <summary>
		/// The applet tile size
		/// </summary>
		[XmlAttribute("size")]
		public AppletTileSize Size {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the icon file reference
		/// </summary>
		[XmlElement("icon")]
		public String Icon {
			get;
			set;
		}

		/// <summary>
		/// Applet tile name
		/// </summary>
		[XmlElement("text")]
		public String Text { 
			get;
			set;
		}
	}

}

