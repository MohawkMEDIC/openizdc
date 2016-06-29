using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Acts
{
    /// <summary>
    /// Represents a table which can store act data
    /// </summary>
    [Table("act")]
    public class DbAct : DbVersionedData
    {
        [Column("isNegated")]
        public bool IsNegated { get; set; }

        /// <summary>
        /// Identifies the time that the act occurred
        /// </summary>
        [Column("actTime")]
        public DateTime? ActTime { get; set; }

        /// <summary>
        /// Identifies the start time of the act
        /// </summary>
        [Column("startTime")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Identifies the stop time of the act
        /// </summary>
        [Column("stopTime")]
        public DateTime? StopTime { get; set; }

        /// <summary>
        /// Identifies the class concept
        /// </summary>
        [Column("classConcept"), MaxLength(16), NotNull]
        public byte[] ClassConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the mood of the act
        /// </summary>
        [Column("moodConcept"), MaxLength(16), NotNull]
        public byte[] MoodConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the reason concept
        /// </summary>
        [Column("reasonConcept"), MaxLength(16)]
        public byte[] ReasonConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the status concept
        /// </summary>
        [Column("statusConcept"), MaxLength(16), NotNull]
        public byte[] StatusConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the type concept
        /// </summary>
        [Column("typeConcept"), MaxLength(16)]
        public byte[] TypeConceptUuid { get; set; }

    }
}
