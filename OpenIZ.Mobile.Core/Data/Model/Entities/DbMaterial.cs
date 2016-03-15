using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Entities
{
	/// <summary>
	/// Represents a material in the database
	/// </summary>
	[Table("material")]
	public class DbMaterial : DbEntity
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
		[Column("form_concept"), Indexed, NotNull)]
		public int FormConcept {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the quantity concept.
		/// </summary>
		/// <value>The quantity concept.</value>
		[Column("quantity")]
		public int QuantityConcept {
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
		[Column("lot"), NotNull, Indexed)]
		public String LotNumber {
			get;
			set;
		}
	}

}

