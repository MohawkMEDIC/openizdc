using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents a telecommunications address
	/// </summary>
	[Table("entity_telecom")]
	public class DbTelecomAddress : DbEntityLink
	{

		/// <summary>
		/// Gets or sets the telecom use.
		/// </summary>
		/// <value>The telecom use.</value>
		[Column("use"), MaxLength(16)]
		public byte[] TelecomUseUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[Column("value"), NotNull, Indexed]
		public String Value {
			get;
			set;
		}

	}
}

