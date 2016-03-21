using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents one or more entity addresses linked to an Entity
	/// </summary>
	[Table("entity_address")]
	public class DbEntityAddress : DbEntityLink
	{
		
		/// <summary>
		/// Gets or sets the use concept identifier.
		/// </summary>
		/// <value>The use concept identifier.</value>
		[Column("use_concept_uuid"), MaxLength(16)]
		public byte[] UseConceptUuid {
			get;
			set;
		}

	}

	/// <summary>
	/// Represents an identified address component
	/// </summary>
	[Table("entity_address_comp")]
	public class DbEntityAddressComponent : DbIdentified
	{

		/// <summary>
		/// Gets or sets the address identifier.
		/// </summary>
		/// <value>The address identifier.</value>
		[Column("address_uuid"), MaxLength(16)]
		public byte[] AddressUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type identifier.
		/// </summary>
		/// <value>The type identifier.</value>
		[Column("type_uuid"), MaxLength(16), NotNull]
		public byte[] TypeUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value of the identifier
		/// </summary>
		/// <value>The value.</value>
		[Column("value")]
		public String Value {
			get;
			set;
		}

	}

}

