using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Acts
{
    /// <summary>
    /// Represents storage class for a patient encounter
    /// </summary>
    [Table("encounter")]
    public class DbPatientEncounter : DbIdentified
    {

        /// <summary>
        /// Identifies the manner in which the patient was discharged
        /// </summary>
        [Column("dischargeDisposition"), MaxLength(16)]
        public byte[] DischargeDispositionUuid { get; set; }
    }
}
