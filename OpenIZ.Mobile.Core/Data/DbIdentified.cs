﻿using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data
{
	/// <summary>
	/// Represents data that is identified in some way
	/// </summary>
	public abstract class DbIdentified
	{

		/// <summary>
		/// Gets or sets the identifier of the identified data
		/// </summary>
		/// <value>The identifier.</value>
		[PrimaryKey, AutoIncrement, Column("id"), NotNull]
		public int Id {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the universal identifier for the object
		/// </summary>
		[Column("uuid"), MaxLength(16), Indexed, NotNull]
		public byte[] Uuid {
			get;
			set;
		}

	}
}
