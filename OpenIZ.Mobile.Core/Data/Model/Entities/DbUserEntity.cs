using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
    /// <summary>
    /// User entity ORM
    /// </summary>
    [Table("user")]
    public class DbUserEntity : DbIdentified
    {

        /// <summary>
        /// Gets or sets the security user which is associated with this entity
        /// </summary>
        [Column("securityUser"), MaxLength(16), Indexed, NotNull]
        public byte[] SecurityUserUuid { get; set; }

    }
}
