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
    /// Associates the audit actor to audit message
    /// </summary>
    [Table("audit_actor_assoc")]
    [AssociativeTable(typeof(DbAuditData), typeof(DbAuditActor))]
    public class DbAuditActorAssociation
    {
        /// <summary>
        /// Id of the association
        /// </summary>
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Audit identifier
        /// </summary>
        [Column("audit_id"), NotNull, ForeignKey(typeof(DbAuditData), nameof(DbAuditData.Id))]
        public int AuditId { get; set; }

        /// <summary>
        /// Actor identifier
        /// </summary>
        [Column("actor_id"), NotNull, ForeignKey(typeof(DbAuditActor), nameof(DbAuditActor.Id))]
        public int ActorId { get; set; }

    }
}
