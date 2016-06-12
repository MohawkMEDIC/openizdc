using System;
using OpenIZ.Core.Model.Entities;
using SQLite;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Mobile.Core.Data.Model.Roles
{
	/// <summary>
	/// Represents a patient in the SQLite store
	/// </summary>
	[Table("patient")]
	public class DbPatient : DbIdentified
	{

		/// <summary>
		/// Gets or sets the gender concept
		/// </summary>
		/// <value>The gender concept.</value>
		[Column("genderConcept"), NotNull, MaxLength(16)]
		public byte[] GenderConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the deceased date.
		/// </summary>
		/// <value>The deceased date.</value>
		[Column("deceasedDate")]
		public DateTime? DeceasedDate {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the deceased date precision.
		/// </summary>
		/// <value>The deceased date precision.</value>
		[Column("deceasedDatePrevision"), MaxLength(1)]
		public string DeceasedDatePrecision {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the multiple birth order.
		/// </summary>
		/// <value>The multiple birth order.</value>
		[Column("birth_order")]
		public int? MultipleBirthOrder {
			get;
			set;
		}

	}
}

