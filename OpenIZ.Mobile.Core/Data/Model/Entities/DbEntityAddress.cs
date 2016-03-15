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
		/// Gets or sets the entity identifier.
		/// </summary>
		/// <value>The entity identifier.</value>
		[Column("source_id")]
		public int EntityId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the use concept identifier.
		/// </summary>
		/// <value>The use concept identifier.</value>
		[Column("use_concept")]
		public int UseConceptId {
			get;
			set;
		}

	}

	/// <summary>
	/// Represents an identified address component
	/// </summary>
	[Table("entity_address_comp")]
	public class EntityAddressComponent : DbIdentified
	{

		/// <summary>
		/// Gets or sets the address identifier.
		/// </summary>
		/// <value>The address identifier.</value>
		[Column("address_id")]
		public int AddressId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type identifier.
		/// </summary>
		/// <value>The type identifier.</value>
		[Column("type_id"), NotNull]
		public int TypeId {
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

