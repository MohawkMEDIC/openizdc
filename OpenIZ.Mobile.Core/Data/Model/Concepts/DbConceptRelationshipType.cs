using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Concepts
{
	/// <summary>
	/// Concept relationship type.
	/// </summary>
	[Table("concept_relationship_type")]
	public class DbConceptRelationshipType: DbIdentified
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

	}
}

