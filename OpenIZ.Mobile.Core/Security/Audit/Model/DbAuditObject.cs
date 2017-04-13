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
    /// Represents a target object
    /// </summary>
    [Table("audit_object")]
    public class DbAuditObject
    {
        /// <summary>
        /// Identifier of the object
        /// </summary>
        [Column("id"), PrimaryKey]
        public byte[] Id { get; set; }

        /// <summary>
        /// Gets or sets the audit identifier
        /// </summary>
        [Column("audit_id"), ForeignKey(typeof(DbAuditData), nameof(DbAuditData.Id))]
        public byte[] AuditId { get; set; }

        /// <summary>
        /// The identifier of the object
        /// </summary>
        [Column("obj_id"), NotNull, MaxLength(16)]
        public string ObjectId { get; set; }

        /// <summary>
        /// The object type identifier
        /// </summary>
        [Column("obj_typ")]
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the role
        /// </summary>
        [Column("rol")]
        public int Role { get; set; }

        /// <summary>
        /// The lifecycle
        /// </summary>
        [Column("lifecycle")]
        public int Lifecycle { get; set; }

        /// <summary>
        /// Identifier type code
        /// </summary>
        [Column("id_type")]
        public int IDTypeCode { get; set; }

        /// <summary>
        /// The query associated
        /// </summary>
        [Column("query")]
        public String QueryData { get; set; }

        /// <summary>
        /// The name data associated 
        /// </summary>
        [Column("name")]
        public String Name { get; set; }
    }
}
