using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Search.Model
{
    /// <summary>
    /// Search entity master list of types
    /// </summary>
    [Table("entity")]
    public class SearchEntityType
    {

        /// <summary>
        /// Search entity type ctor
        /// </summary>
        public SearchEntityType()
        {
            this.Key = Guid.NewGuid().ToByteArray();
        }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [Column("key"), PrimaryKey, MaxLength(16)]
        public byte[] Key { get; set; }

        /// <summary>
        /// Gets or sets the version in the index
        /// </summary>
        [Column("version"), MaxLength(16), NotNull]
        public byte[] VersionKey { get; set; }

        /// <summary>
        /// Represents the type of model
        /// </summary>
        [Column("type"), NotNull, Indexed]
        public String SearchType { get; set; }


    }
}
