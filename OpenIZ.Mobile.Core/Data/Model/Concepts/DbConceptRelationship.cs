using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Concepts
{
	/// <summary>
	/// Represents concept relationships
	/// </summary>
	[Table("concept_relationship")]
	public class DbConceptRelationship : DbIdentified
	{

		/// <summary>
		/// Gets or sets the source concept.
		/// </summary>
		[Column("source_concept"), Indexed, NotNull, MaxLength(16)]
		public byte[] SourceConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the target concept identifier.
		/// </summary>
		/// <value>The target concept identifier.</value>
		[Column("target_concept"), Indexed, NotNull, MaxLength(16)]
		public byte[] TargetConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the relationship type identifier.
		/// </summary>
		/// <value>The relationship type identifier.</value>
		[Column("relationship_type"), NotNull, MaxLength(16)]
		public byte[] RelationshipTypeUuid {
			get;
			set;
		}
	}
}

