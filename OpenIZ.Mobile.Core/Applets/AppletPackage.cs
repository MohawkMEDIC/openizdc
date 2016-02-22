using System;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Applets
{
	/// <summary>
	/// Applet package used for installations only
	/// </summary>
	[XmlType(nameof(AppletPackage), Namespace = "http://openiz.org/mobile/applet")]
	public class AppletPackage
	{

		/// <summary>
		/// Applet reference metadata
		/// </summary>
		[XmlElement("meta")]
		public AppletReference Meta {
			get;
			set;
		}

		/// <summary>
		/// Gets or ses the manifest to be installed
		/// </summary>
		/// <value>The manifest.</value>
		[XmlElement("manifest")]
		public byte[] Manifest {
			get;
			set;
		}
	}
}

