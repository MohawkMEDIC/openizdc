using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Extensibility
{
	/// <summary>
	/// Extension types
	/// </summary>
	[Table("extension_type")]
	public class DbExtensionType: DbIdentified
	{

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Column("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the extension handler.
		/// </summary>
		/// <value>The extension handler.</value>
		[Column("extensionHandler")]
		public String ExtensionHandler {
			get;
			set;
		}
	}
}

