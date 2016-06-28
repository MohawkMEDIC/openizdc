using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
    /// <summary>
    /// Represents the orm class for place service
    /// </summary>
    [Table("place_service")]
    public class DbPlaceService : DbEntityLink
    {

        /// <summary>
        /// Gets or sets the service schedule information
        /// </summary>
        [Column("serviceSchedule")]
        public byte[] ServiceSchedule { get; set; }

        /// <summary>
        /// Gets or sets the service concept
        /// </summary>
        [Column("serviceConcept"), MaxLength(16)]
        public byte[] ServiceConceptUuid { get; set; }

    }
}
