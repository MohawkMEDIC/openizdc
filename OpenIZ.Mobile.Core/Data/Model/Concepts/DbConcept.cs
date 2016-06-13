using System;
using System.Linq;
using SQLite;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Data.Model.Concepts
{
	/// <summary>
	/// Physical data layer implemntation of concept
	/// </summary>
	[Table("concept")]
	public class DbConcept : DbVersionedData
	{

		/// <summary>
		/// Gets or sets whether the object is a system concept or not
		/// </summary>
		[Column("isReadonly")]
		public bool IsSystemConcept {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the object mnemonic
		/// </summary>
		/// <value>The mnemonic.</value>
		[Column("mnemonic"), Unique, NotNull]
		public string Mnemonic {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the status concept id
		/// </summary>
		[Column("statusConcept"), Indexed, NotNull, MaxLength(16)]
		public byte[] StatusUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the concept classification
		/// </summary>
		[Column("class"), NotNull, MaxLength(16)]
		public byte[] ClassUuid {
			get;
			set;
		}

    }
}

