using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Concepts
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
		[Column("source_concept"), Indexed, NotNull]
		public int SourceConceptId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the target concept identifier.
		/// </summary>
		/// <value>The target concept identifier.</value>
		[Column("target_concept"), Indexed, NotNull]
		public int TargetConceptId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the relationship type identifier.
		/// </summary>
		/// <value>The relationship type identifier.</value>
		[Column("relationship_type"), NotNull]
		public int RelationshipTypeId {
			get;
			set;
		}
	}
}

