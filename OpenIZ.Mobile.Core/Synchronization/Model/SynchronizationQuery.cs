using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Synchronization.Model
{
    /// <summary>
    /// Synchroinzation query exec
    /// </summary>
    [Table("sync_queue")]
    public class SynchronizationQuery : SynchronizationLogEntry
    {


        /// <summary>
        /// Last successful record number
        /// </summary>
        [Column("last_recno")]
        public int LastSuccess { get; set; }

        /// <summary>
        /// Start time of the query
        /// </summary>
        [Column("start")]
        public DateTime StartTime { get; set; }

    }
}
