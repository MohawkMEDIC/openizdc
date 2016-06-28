using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Security
{
	/// <summary>
	/// Security application data. Should only be one entry here as well
	/// </summary>
	[Table("security_application")]
	public class DbSecurityApplication : DbBaseData
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

