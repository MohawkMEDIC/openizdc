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
		/// Gets the specified name
		/// </summary>
		public String GetName(String language, bool returnNuetralIfNotFound = true)
		{
			var str = this.Names?.Find(o=>o.Language == language);
			if(str == null && returnNuetralIfNotFound)
				str = this.Names?.Find(o=>o.Language == null);
			return str?.Value;
		}

		/// <summary>
		/// Gets the specified name
		/// </summary>
		public String GetGroupName(String language, bool returnNuetralIfNotFound = true)
		{
			var str = this.GroupNames?.Find(o=>o.Language == language);
			if(str == null && returnNuetralIfNotFound)
				str = this.GroupNames?.Find(o=>o.Language == null);
			return str?.Value;
		}

		/// <summary>
		/// Gets or sets the icon resource
		/// </summary>
		/// <value>The icon.</value>
		[XmlElement("icon")]
		public String Icon {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the applet info
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("name")]
		public List<LocaleString> Names {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the applet info
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("groupName")]
		public List<LocaleString> GroupNames {
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
		public String Id {
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

		/// <summary>
		/// Gets the thumbprint of the key that signed the signature
		/// </summary>
		[XmlElement("publicKeyToken")]
		public String PublicKeyToken {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the applet's dependencies
		/// </summary>
		[XmlElement("dependency")]
		public List<AppletReference> Dependencies {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the applets required policies for a user to run
		/// </summary>
		[XmlElement("demand")]
		public List<String> Policies {
			get;
			set;
		}

		/// <summary>
		/// Return this applet reference
		/// </summary>
		public AppletReference AsReference()
		{
			return new AppletReference (this.Id, this.Version, this.PublicKeyToken);
		}
	}

}

