using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Acts
{
    /// <summary>
    /// Represents storage class for a substance administration
    /// </summary>
    [Table("substance_administration")]
    public class DbSubstanceAdministration : DbIdentified
    {
        /// <summary>
        /// Gets or sets the route of administration
        /// </summary>
        [Column("routeConcept"), MaxLength(16)]
        public byte[] RouteConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the dose unit
        /// </summary>
        [Column("doseUnit"), MaxLength(16)]
        public byte[] DoseUnitConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the dose quantity
        /// </summary>
        [Column("doseQuantity")]
        public Decimal DoseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the sequence number
        /// </summary>
        [Column("sequenceId")]
        public int SequenceId { get; set; }

    }
}
