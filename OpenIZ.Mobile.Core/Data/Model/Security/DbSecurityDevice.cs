using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Security
{
	/// <summary>
	/// Represents a security device. This table should only have one row (the current device)
	/// </summary>
	[Table("security_device")]
	public class DbSecurityDevice : DbBaseData
	{
		
		/// <summary>
		/// Gets or sets the public identifier.
		/// </summary>
		/// <value>The public identifier.</value>
		[Column("public_id"), Unique]
		public String PublicId {
			get;
			set;
		}


	}
}

