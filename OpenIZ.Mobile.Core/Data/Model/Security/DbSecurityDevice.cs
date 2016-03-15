using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Security
{
	/// <summary>
	/// Represents a security device. This table should only have one row (the current device)
	/// </summary>
	[Table("security_device")]
	public class DbSecurityDevice : DbIdentified
	{

		/// <summary>
		/// Device secret
		/// </summary>
		/// <value>The device secret.</value>
		[Column("secret"), Unique]
		public String DeviceSecret {
			get;
			set;
		}

	}
}

