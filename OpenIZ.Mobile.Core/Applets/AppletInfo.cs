using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Applets
{
	/// <summary>
	/// Represents applet information
	/// </summary>
	[XmlType(nameof(AppletInfo), Namespace = "http://openiz.org/mobile/applet")]
	public class AppletInfo
	{

		/// <summary>
		/// Gets or sets the name of the applet info
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the author of the applet
		/// </summary>
		[XmlElement("author")]
		public String Author {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the identifier of the applet
		/// </summary>
		[XmlElement("id")]
		public Guid Id {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the version of the applet
		/// </summary>
		[XmlElement("version")]
		public String Version {
			get;
			set;
		}
	}

}

