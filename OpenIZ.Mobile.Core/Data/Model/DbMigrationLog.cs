using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model
{
    /// <summary>
    /// Keeps track of migrations
    /// </summary>
    [Table("migrations")]
    public class DbMigrationLog
    {

        /// <summary>
        /// Migration log
        /// </summary>
        [PrimaryKey, Column("id")]
        public String MigrationId { get; set; }

        /// <summary>
        /// Installation date
        /// </summary>
        [Column("date")]
        public DateTime InstallationDate { get; set; }
    }
}
