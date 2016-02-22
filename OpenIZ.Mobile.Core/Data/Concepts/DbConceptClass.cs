using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Concepts
{
	/// <summary>
	/// Represents storage entity for concept class
	/// </summary>
	[Table("concept_class")]
	public class DbConceptClass : DbIdentified
	{

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Column("name"), Indexed]
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the mnemonic.
		/// </summary>
		/// <value>The mnemonic.</value>
		[Column("mnemonic"), Unique, NotNull]
		public String Mnemonic {
			get;
			set;
		}
	}
}

