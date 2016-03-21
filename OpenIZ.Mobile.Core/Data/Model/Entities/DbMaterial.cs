using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents a material in the database
	/// </summary>
	[Table("material")]
	public class DbMaterial : DbEntityLink
	{

		/// <summary>
		/// Gets or sets the quantity of an entity within its container.
		/// </summary>
		/// <value>The quantity.</value>
		[Column("quantity")]
		public decimal Quantity {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the form concept.
		/// </summary>
		/// <value>The form concept.</value>
		[Column("form_concept_uuid"), Indexed, NotNull, MaxLength(16)]
		public byte[] FormConcept {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the quantity concept.
		/// </summary>
		/// <value>The quantity concept.</value>
		[Column("quantity_concept_uuid"), MaxLength(16)]
		public byte[] QuantityConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the expiry date.
		/// </summary>
		/// <value>The expiry date.</value>
		[Column("expiry")]
		public DateTime ExpiryDate {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is administrative.
		/// </summary>
		/// <value><c>true</c> if this instance is administrative; otherwise, <c>false</c>.</value>
		[Column("isAdministrative")]
		public bool IsAdministrative {
			get;
			set;
		}
	}

	/// <summary>
	/// Manufactured material.
	/// </summary>
	[Table("manufactured_material")]
	public class DbManufacturedMaterial : DbMaterial
	{

		/// <summary>
		/// Gets or sets the lot number.
		/// </summary>
		/// <value>The lot number.</value>
		[Column("lot"), NotNull, Indexed]
		public String LotNumber {
			get;
			set;
		}
	}

}

