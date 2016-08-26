using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Search.Model
{
    /// <summary>
    /// Represents a search term
    /// </summary>
    [Table("search_term")]
    public class SearchTerm
    {
        /// <summary>
        /// The search term ctor
        /// </summary>
        public SearchTerm()
        {
            this.Key = Guid.NewGuid().ToByteArray();
        }

        /// <summary>
        /// Gets or sets the primary key of the term
        /// </summary>
        [Column("key"), PrimaryKey, MaxLength(16)]
        public byte[] Key { get; set; }

        /// <summary>
        /// Gets or sets the term
        /// </summary>
        [Column("term"), Indexed(Unique = true)]
        public String Term { get; set; }
    }
}
