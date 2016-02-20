using System;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Applets
{
	/// <summary>
	/// Represents applet tile sizes
	/// </summary>
	[Flags]
	[XmlType(nameof(AppletTileSize), Namespace = "http://openiz.org/mobile/applet")]
	public enum AppletTileSize
	{
		[XmlEnum("sm")]
		Small = 0x01,
		[XmlEnum("lg")]
		Large = 0x02
	}
}

