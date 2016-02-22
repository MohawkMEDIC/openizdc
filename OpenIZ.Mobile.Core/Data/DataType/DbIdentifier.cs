using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.DataType
{
	/// <summary>
	/// Represents an identifier
	/// </summary>
	public abstract class DbIdentifier : DbIdentified
	{

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		[Column("value"), Indexed, NotNull]
		public String Value {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type identifier.
		/// </summary>
		/// <value>The type identifier.</value>
		[Column("type")]
		public int TypeId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the authority identifier.
		/// </summary>
		/// <value>The authority identifier.</value>
		[Column("authority"), NotNull]
		public int AuthorityId {
			get;
			set;
		}
	}

	/// <summary>
	/// Entity identifier storage.
	/// </summary>
	[Table("entity_identifier")]
	public class DbEntityIdentifier : DbIdentifier
	{
	}

	/// <summary>
	/// Act identifier storage.
	/// </summary>
	[Table("act_identifier")]
	public class DbActIdentifier : DbIdentifier
	{
	}
}

