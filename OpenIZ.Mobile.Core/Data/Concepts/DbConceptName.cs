using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Concepts
{
	/// <summary>
	/// Represents a concept name
	/// </summary>
	[Table("concept_name")]
	public class DbConceptName
	{
	
		/// <summary>
		/// Gets or sets the concept identifier.
		/// </summary>
		/// <value>The concept identifier.</value>
		[Column("concept_id"), Indexed, NotNull]
		public int ConceptId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the language.
		/// </summary>
		/// <value>The language.</value>
		[Column("lang")]
		public String Language {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Column("name"), Indexed, NotNull]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the phonetic code.
		/// </summary>
		/// <value>The phonetic code.</value>
		[Column("phon_code"), Indexed]
		public String PhoneticCode {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the phonetic algorithm identifier.
		/// </summary>
		/// <value>The phonetic algorithm identifier.</value>
		[Column("phon_alg")]
		public int PhoneticAlgorithmId {
			get;
			set;
		}
	}
}

