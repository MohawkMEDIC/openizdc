using System;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Roles
{
	/// <summary>
	/// Represents a health care provider in the database
	/// </summary>
	[Table("provider")]
	public class DbProvider : DbIdentified
    {

		/// <summary>
		/// Gets or sets the specialty.
		/// </summary>
		/// <value>The specialty.</value>
		[Column("specialty"), MaxLength(16)]
		public byte[] Specialty {
			get;
			set;
		}

	}
}

