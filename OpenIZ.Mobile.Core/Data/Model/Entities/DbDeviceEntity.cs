using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents the entity representation of an object
	/// </summary>
	[Table("device")]
	public class DbDeviceEntity : DbIdentified
    {

		/// <summary>
		/// Gets or sets the security device identifier.
		/// </summary>
		/// <value>The security device identifier.</value>
		[Column("securityDevice"), MaxLength(16)]
		public byte[] SecurityDeviceUuid {
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
		[Column("operatingSystem")]
		public String OperatingSystemName {
			get;
			set;
		}
	}
}

