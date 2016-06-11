using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.DataType
{
    /// <summary>
    /// Represents an assigning authority
    /// </summary>
    [Table("assigning_authority")]
    public class DbAssigningAuthority : DbIdentified
    {

        /// <summary>
        /// Gets or sets the name of the aa
        /// </summary>
        [Column("name"), Indexed]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the short HL7 code of the AA
        /// </summary>
        [Column("domainName"), Indexed, MaxLength(32)]
        public String DomainName { get; set; }

        /// <summary>
        /// Gets or sets the OID of the AA
        /// </summary>
        [Column("oid")]
        public String Oid { get; set; }

        /// <summary>
        /// Gets or sets the description of the AA
        /// </summary>
        [Column("description")]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of AA
        /// </summary>
        [Column("url")]
        public String Url { get; set; }


    }
}
