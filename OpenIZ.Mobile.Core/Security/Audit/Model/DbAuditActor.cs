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
    /// Audit actors
    /// </summary>
    [Table("audit_actor")]
    public class DbAuditActor
    {
        /// <summary>
        /// Identifier for the actor instance
        /// </summary>
        [Column("id"), PrimaryKey]
        public byte[] Id { get; set; }

        /// <summary>
        /// User identifier
        /// </summary>
        [Column("user_id"), MaxLength(16), Indexed]
        public String UserIdentifier { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [Column("user_name"), Unique, Collation("NOCASE")]
        public String UserName { get; set; }

        /// <summary>
        /// True if user is requestor
        /// </summary>
        [Column("is_requestor")]
        public bool UserIsRequestor { get; set; }

        /// <summary>
        /// Role code identifier
        /// </summary>
        [Column("role_code_id"), ForeignKey(typeof(DbAuditCode), nameof(DbAuditCode.Id))]
        public byte[] RoleCodeId { get; set; }

    }
}
