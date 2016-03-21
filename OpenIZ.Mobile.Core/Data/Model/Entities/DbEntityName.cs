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
		/// Gets or sets the use concept.
		/// </summary>
		/// <value>The use concept.</value>
		[Column("use_concept_uuid"), MaxLength(16)]
		public byte[] UseConcept {
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
		[Column("name_id"), MaxLength(16)]
		public byte[] NameId {
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
		[Column("phon_alg_uuid"), NotNull]
		public byte[] PhoneticAlgorithmUuid {
			get;
			set;
		}
	}

}

