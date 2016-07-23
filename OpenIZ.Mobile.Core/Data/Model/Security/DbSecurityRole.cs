using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Security
{
	/// <summary>
	/// Represents a security role
	/// </summary>
	[Table("security_role")]
	public class DbSecurityRole : DbIdentified
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Column("name"), NotNull, Unique]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[Column("description")]
		public String Description {
			get;
			set;
		}

	}
}

