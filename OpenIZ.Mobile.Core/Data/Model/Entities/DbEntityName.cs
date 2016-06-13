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
		[Column("use"), MaxLength(16)]
		public byte[] UseConceptUuid {
			get;
			set;
		}
	}

	/// <summary>
	/// Represents a component of a name
	/// </summary>
	[Table("entity_name_comp")]
	public class DbEntityNameComponent : DbGenericNameComponent
	{

		/// <summary>
		/// Gets or sets the name identifier.
		/// </summary>
		/// <value>The name identifier.</value>
		[Column("name_uuid"), MaxLength(16), NotNull]
		public byte[] NameUuid {
			get;
			set;
		}
        
		/// <summary>
		/// Gets or sets the phonetic code.
		/// </summary>
		/// <value>The phonetic code.</value>
		[Column("phoneticCode"), Indexed]
		public String PhoneticCode {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the phonetic algorithm identifier.
		/// </summary>
		/// <value>The phonetic algorithm identifier.</value>
		[Column("phoneticAlgorithm")]
		public byte[] PhoneticAlgorithmUuid {
			get;
			set;
		}
	}

}

