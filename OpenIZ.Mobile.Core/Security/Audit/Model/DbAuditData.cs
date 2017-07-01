using OpenIZ.Core.Data.QueryBuilder.Attributes;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Security.Audit.Model
{
    /// <summary>
    /// Audit data
    /// </summary>
    [Table("audit")]
    [AssociativeTable(typeof(DbAuditActor), typeof(DbAuditActorAssociation))]
    public class DbAuditData
    {
        /// <summary>
        /// The identifier assigned to the audit number
        /// </summary>
        [Column("id"), PrimaryKey]
        public byte[] Id { get; set; }

        /// <summary>
        /// Outcome of the event
        /// </summary>
        [Column("outcome")]
        public int Outcome { get; set; }

        /// <summary>
        /// The action performed
        /// </summary>
        [Column("action")]
        public int ActionCode { get; set; }

        /// <summary>
        /// The type of action performed
        /// </summary>
        [Column("type")]
        public int EventIdentifier { get; set; }

        /// <summary>
        /// The time of the event
        /// </summary>
        [Column("eventTime")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The time the data was created
        /// </summary>
        [Column("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// The event type identifier
        /// </summary>
        [Column("class"), ForeignKey(typeof(DbAuditCode), nameof(DbAuditCode.Id)), AlwaysJoin]
        public byte[] EventTypeCode { get; set; }

        /// <summary>
        /// Query result type
        /// </summary>
        public class QueryResult : DbAuditData
        {

            /// <summary>
            /// Code
            /// </summary>
            [Column("code"), Indexed, NotNull]
            public string Code { get; set; }

            /// <summary>
            /// Code system
            /// </summary>
            [Column("code_system"), Indexed]
            public String CodeSystem { get; set; }

        }
    }
}
