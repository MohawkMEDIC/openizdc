using System;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using OpenIZ.Mobile.Core.Configuration;

namespace OpenIZ.Mobile.Core.Applets
{
	/// <summary>
	/// The applet manifest is responsible for storing data related to a JavaScript applet
	/// </summary>
	[XmlType(nameof(AppletManifest), Namespace = "http://openiz.org/mobile/applet")]
	[XmlRoot(nameof(AppletManifest), Namespace = "http://openiz.org/mobile/applet")]
	public class AppletManifest
	{

		/// <summary>
		/// Load the specified manifest name
		/// </summary>
		public static AppletManifest Load(Stream resourceStream)
		{
			XmlSerializer xsz = new XmlSerializer (typeof(AppletManifest));
			return xsz.Deserialize (resourceStream) as AppletManifest;
		}

		/// <summary>
		/// Create an unsigned package
		/// </summary>
		/// <returns>The package.</returns>
		public AppletPackage CreatePackage()
		{
			AppletPackage retVal = new AppletPackage () {
				Meta = this.Info.AsReference ()
			};
			using (MemoryStream ms = new MemoryStream ()) {
				XmlSerializer xsz = new XmlSerializer (typeof(AppletManifest));
				xsz.Serialize (ms, this);
				retVal.Manifest = ms.ToArray ();
			}
			return retVal;

		}

		/// <summary>
		/// Gets or sets the tile sizes the applet can have
		/// </summary>
		[XmlElement("tile")]
		public List<AppletTile> Tiles {
			get;
			set;
		}

		/// <summary>
		/// Applet information itself
		/// </summary>
		[XmlElement("info")]
		public AppletInfo Info {
			get;
			set;
		}

		/// <summary>
		/// Initial applet configuration
		/// </summary>
		/// <value>The configuration.</value>
		[XmlElement("configuration", Namespace = "http://openiz.org/mobile/configuration")]
		public AppletConfiguration Configuration {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the assets which are to be used in the applet
		/// </summary>
		[XmlElement("asset")]
		public List<AppletAsset> Assets {
			get;
			set;
		}
	}
}

