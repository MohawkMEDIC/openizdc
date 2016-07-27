using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Synchronization.Model
{
    /// <summary>
    /// Represents a synchronization log entry
    /// </summary>
    [Table("tx_log")]
    public class SynchronizationLogEntry
    {
        /// <summary>
        /// Synchronization log
        /// </summary>
        public SynchronizationLogEntry()
        {
            this.Uuid = Guid.NewGuid().ToByteArray();
        }

        /// <summary>
        /// The unique identifier for the summary
        /// </summary>
        [Column("uuid"), MaxLength(16), PrimaryKey]
        public byte[] Uuid { get; set; }

        /// <summary>
        /// Gets or sets the resource type which was synchronized
        /// </summary>
        [Column("resource"), NotNull, Indexed]
        public String ResourceType { get; set; }

        /// <summary>
        /// The filter (if any)
        /// </summary>
        [Column("filter")]
        public String Filter { get; set; }

        /// <summary>
        /// Gets or sets the key of the resource last synchronized of this type
        /// </summary>
        [Column("etag")]
        public String LastETag { get; set; }

        /// <summary>
        /// Represents the last synchronization performed
        /// </summary>
        [Column("time")]
        public DateTime LastSync { get; set; }
    }
}
