using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.DataType
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
		[Column("authority_uuid"), NotNull]
		public int AuthorityUuid {
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
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("entity_uuid"), Indexed, MaxLength(16)]
        public byte[] EntityUuid
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Act identifier storage.
	/// </summary>
	[Table("act_identifier")]
	public class DbActIdentifier : DbIdentifier
	{
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("act_uuid"), Indexed, MaxLength(16)]
        public byte[] ActUuid
        {
            get;
            set;
        }
    }
}

