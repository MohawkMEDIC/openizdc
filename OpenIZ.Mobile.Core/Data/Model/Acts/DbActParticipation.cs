using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Acts
{
    /// <summary>
    /// Represents a link between an act and an entity
    /// </summary>
    [Table("act_participation")]
    public class DbActParticipation : DbIdentified
    {
        /// <summary>
        /// Gets or sets the act identifier
        /// </summary>
        [Column("act_uuid"), MaxLength(16), Indexed, NotNull]
        public byte[] ActUuid { get; set; }

        /// <summary>
        /// Gets or sets the act identifier
        /// </summary>
        [Column("entity_uuid"), MaxLength(16), Indexed, NotNull]
        public byte[] EntityUuid { get; set; }

        /// <summary>
        /// Gets or sets the role that the player plays in the act
        /// </summary>
        [Column("participationRole"), MaxLength(16)]
        public byte[] ParticipationRoleUuid { get; set; }

    }
}
