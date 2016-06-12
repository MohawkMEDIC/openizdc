using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
    /// <summary>
    /// Represents a relationship between two entities
    /// </summary>
    [Table("entity_relationship")] 
    public class DbEntityRelationship : DbEntityLink
    {

        /// <summary>
        /// Gets or sets the target entity
        /// </summary>
        [Column("target"), MaxLength(16), NotNull, Indexed(Name = "entity_relationship_target_type", Unique = true)]
        public byte[] TargetUuid { get; set; }


        /// <summary>
        /// Gets or sets the link type concept
        /// </summary>
        [Column("relationshipType"), MaxLength(16), NotNull, Indexed(Name = "entity_relationship_target_type", Unique = true)]
        public byte[] RelationshipTypeUuid { get; set; }
    }
}
