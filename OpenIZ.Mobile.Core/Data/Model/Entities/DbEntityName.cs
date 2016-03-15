using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents an entity name related to an entity
	/// </summary>
	[Table("entity_name")]
	public class DbEntityName : DbEntityLink
	{

		/// <summary>
		/// Gets or sets the entity identifier.
		/// </summary>
		/// <value>The entity identifier.</value>
		[Column("source_id"), NotNull]
		public int EntityId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the use concept.
		/// </summary>
		/// <value>The use concept.</value>
		[Column("use_concept")]
		public int UseConcept {
			get;
			set;
		}
	}

	/// <summary>
	/// Represents a component of a name
	/// </summary>
	[Table("entity_name_comp")]
	public class DbEntityNameComponent : DbIdentified
	{

		/// <summary>
		/// Gets or sets the name identifier.
		/// </summary>
		/// <value>The name identifier.</value>
		[Column("name_id")]
		public int NameId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[Column("value"), NotNull, Indexed]
		public String Value {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the phonetic code.
		/// </summary>
		/// <value>The phonetic code.</value>
		[Column("phon_code"), Indexed, NotNull]
		public String PhoneticCode {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the phonetic algorithm identifier.
		/// </summary>
		/// <value>The phonetic algorithm identifier.</value>
		[Column("phon_alg"), NotNull]
		public int PhoneticAlgorithmId {
			get;
			set;
		}
	}

}

