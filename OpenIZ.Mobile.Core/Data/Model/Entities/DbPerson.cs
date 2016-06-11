using System;
using SQLite;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents a person
	/// </summary>
	[Table("person")]
	public class DbPerson : DbEntityLink
	{

		/// <summary>
		/// Gets or sets the date of birth.
		/// </summary>
		/// <value>The date of birth.</value>
		[Column("dateOfBirth")]
		public DateTime? DateOfBirth {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the date of birth precision.
		/// </summary>
		/// <value>The date of birth precision.</value>
		[Column("dateOfBirthPrecision")]
		public DatePrecision? DateOfBirthPrecision {
			get;
			set;
		}


	}
}

