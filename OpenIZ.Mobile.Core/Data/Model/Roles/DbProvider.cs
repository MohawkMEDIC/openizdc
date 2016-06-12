using System;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Roles
{
	/// <summary>
	/// Represents a health care provider in the database
	/// </summary>
	[Table("provider")]
	public class DbProvider
	{

		/// <summary>
		/// Gets or sets the specialty.
		/// </summary>
		/// <value>The specialty.</value>
		[Column("specialty")]
		public int? Specialty {
			get;
			set;
		}

	}
}

