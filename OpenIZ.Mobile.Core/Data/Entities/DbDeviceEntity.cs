﻿using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Entities
{
	/// <summary>
	/// Represents the entity representation of an object
	/// </summary>
	[Table("device")]
	public class DbDeviceEntity
	{

		/// <summary>
		/// Gets or sets the security device identifier.
		/// </summary>
		/// <value>The security device identifier.</value>
		[Column("security_device")]
		public int SecurityDeviceId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the manufacturer model.
		/// </summary>
		/// <value>The name of the manufacturer model.</value>
		[Column("manufacturer")]
		public string ManufacturerModelName {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the operating system.
		/// </summary>
		/// <value>The name of the operating system.</value>
		[Column("operating_system")]
		public String OperatingSystemName {
			get;
			set;
		}
	}
}
