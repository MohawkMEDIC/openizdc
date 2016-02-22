﻿using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.DataType
{
	/// <summary>
	/// Identifier type table.
	/// </summary>
	[Table("identifier_type")]
	public class DbIdentifierType : DbIdentified
	{
		
		/// <summary>
		/// Gets or sets the type concept identifier.
		/// </summary>
		/// <value>The type concept identifier.</value>
		[Column("type"), NotNull]
		public int TypeConceptId {
			get;
			set;
		}


	}
}

