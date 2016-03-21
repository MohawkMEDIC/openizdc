using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents an entity in the database
	/// </summary>
	[Table("entity")]
	public class DbEntity : DbVersionedData
	{

		/// <summary>
		/// Gets or sets the class concept identifier.
		/// </summary>
		/// <value>The class concept identifier.</value>
		[Column("class_concept_uuid"), MaxLength(16), NotNull]
		public byte[] ClassConceptId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the determiner concept identifier.
		/// </summary>
		/// <value>The determiner concept identifier.</value>
		[Column("determiner_concept_uuid"), MaxLength(16)]
		public byte[] DeterminerConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the status concept identifier.
		/// </summary>
		/// <value>The status concept identifier.</value>
		[Column("status_concept_uuid"), MaxLength(16)]
		public byte[] StatusConceptId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type concept identifier.
		/// </summary>
		/// <value>The type concept identifier.</value>
		[Column("type_concept_uuid"), MaxLength(16)]
		public byte[] TypeConceptUuid {
			get;
			set;
		}


	}
}

