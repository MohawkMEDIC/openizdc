using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Entities
{
	/// <summary>
	/// Represents an entity which is used to represent an application
	/// </summary>
	[Table("application")]
	public class DbApplicationEntity
	{
		/// <summary>
		/// Gets or sets the security application.
		/// </summary>
		/// <value>The security application.</value>
		[Column("security_application_id")]
		public int SecurityApplication {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the software.
		/// </summary>
		/// <value>The name of the software.</value>
		[Column("name"), NotNull]
		public String SoftwareName {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the version.
		/// </summary>
		/// <value>The name of the version.</value>
		[Column("version")]
		public String VersionName {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the vendor.
		/// </summary>
		/// <value>The name of the vendor.</value>
		[Column("vendor")]
		public String VendorName {
			get;
			set;
		}
	}
}

