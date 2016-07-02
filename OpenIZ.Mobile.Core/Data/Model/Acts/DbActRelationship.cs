using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Acts
{
    /// <summary>
    /// Identifies relationships between acts
    /// </summary>
    [Table("act_relationship")]
    public class DbActRelationship : DbIdentified
    {

        /// <summary>
        /// Gets or sets the source act of the relationship
        /// </summary>
        [Column("act_uuid"), MaxLength(16), NotNull, Indexed]
        public byte[] ActUuid { get; set; }

        /// <summary>
        /// Gets or sets the target entity
        /// </summary>
        [Column("target"), MaxLength(16), NotNull, Indexed]
        public byte[] TargetUuid { get; set; }

        /// <summary>
        /// Gets or sets the link type concept
        /// </summary>
        [Column("relationshipType"), MaxLength(16), NotNull]
        public byte[] RelationshipTypeUuid { get; set; }

    }
}
