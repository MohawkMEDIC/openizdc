using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Extensibility
{
    /// <summary>
    /// Represents a database template definition
    /// </summary>
    [Table("template")]
    public class DbTemplateDefinition : DbBaseData
    {
        /// <summary>
        /// Gets the OID of the template
        /// </summary>
        [Column("oid")]
        public String Oid { get; set; }

        /// <summary>
        /// Gets the name of the template
        /// </summary>
        [Column("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets the mnemonic
        /// </summary>
        [Column("mnemonic")]
        public String Mnemonic { get; set; }

    }
}
