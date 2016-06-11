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
		[Column("classConcept"), MaxLength(16), NotNull]
		public byte[] ClassConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the determiner concept identifier.
		/// </summary>
		/// <value>The determiner concept identifier.</value>
		[Column("determinerConcept"), MaxLength(16)]
		public byte[] DeterminerConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the status concept identifier.
		/// </summary>
		/// <value>The status concept identifier.</value>
		[Column("statusConcept"), MaxLength(16)]
		public byte[] StatusConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type concept identifier.
		/// </summary>
		/// <value>The type concept identifier.</value>
		[Column("typeConcept"), MaxLength(16)]
		public byte[] TypeConceptUuid {
			get;
			set;
		}


	}
}

