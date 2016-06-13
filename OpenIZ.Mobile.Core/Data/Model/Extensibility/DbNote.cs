using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Extensibility
{
	/// <summary>
	/// Represents note storage
	/// </summary>
	public abstract class DbNote : DbIdentified
	{

        /// <summary>
		/// Gets or sets the author identifier.
		/// </summary>
		/// <value>The author identifier.</value>
		[Column("author"), MaxLength(16), NotNull]
		public byte[] AuthorUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		[Column("text")]
		public String Text {
			get;
			set;
		}
	}

	/// <summary>
	/// Entity note.
	/// </summary>
	[Table("entity_note")]
	public class DbEntityNote : DbNote
	{
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("entity_uuid"), Indexed, NotNull, MaxLength(16)]
        public byte[] EntityUuid
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Act note.
    /// </summary>
    [Table("act_note")]
	public class DbActNote : DbNote
	{
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("act_uuid"), Indexed, NotNull, MaxLength(16)]
        public byte[] ActUuid
        {
            get;
            set;
        }

    }
}

