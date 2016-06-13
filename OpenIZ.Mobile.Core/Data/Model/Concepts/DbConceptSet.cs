using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Concepts
{
	/// <summary>
	/// Concept set
	/// </summary>
	[Table("concept_set")]
	public class DbConceptSet : DbIdentified
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Column("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the mnemonic.
		/// </summary>
		/// <value>The mnemonic.</value>
		[Column("mnemonic"), Indexed, NotNull]
		public String Mnemonic {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the oid of the concept set
        /// </summary>
        [Column("oid"), Indexed]
        public String Oid { get; set; }

        /// <summary>
        /// Gets or sets the url of the concept set
        /// </summary>
        [Column("url")]
        public String Url { get; set; }

    }

	/// <summary>
	/// Concept set concept association.
	/// </summary>
	[Table("concept_concept_set")]
	public class DbConceptSetConceptAssociation : DbIdentified
	{

		/// <summary>
		/// Gets or sets the concept identifier.
		/// </summary>
		/// <value>The concept identifier.</value>
		[Column("concept_uuid"), NotNull, Indexed(Name = "concept_concept_set_concept_set", Unique = true), MaxLength(16)]
		public byte[] ConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the concept set identifier.
		/// </summary>
		/// <value>The concept set identifier.</value>
		[Column("concept_set_uuid"), Indexed(Name = "concept_concept_set_concept_set", Unique = true), MaxLength(16)]
		public int ConceptSetUuid {
			get;
			set;
		}
	}
}

