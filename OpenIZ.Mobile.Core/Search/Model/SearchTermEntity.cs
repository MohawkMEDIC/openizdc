using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Search.Model
{
    /// <summary>
    /// Identifies a link between a search term and an entity
    /// </summary>
    [Table("search_term_entity")]
    public class SearchTermEntity
    {
        /// <summary>
        /// Constructs a new instances of the search term entity link
        /// </summary>
        public SearchTermEntity()
        {
            this.Key = Guid.NewGuid().ToByteArray();
        }

        /// <summary>
        /// Key of the association
        /// </summary>
        [Column("key"), PrimaryKey, MaxLength(16)]
        public byte[] Key { get; set; }

        /// <summary>
        /// Entity identifier
        /// </summary>
        [Column("entity"), NotNull, Indexed, MaxLength(16)]
        public byte[] EntityId { get; set; }

        /// <summary>
        /// Gets or sets the term identifier
        /// </summary>
        [Column("term"), NotNull, Indexed, MaxLength(16)]
        public byte[] TermId { get; set; }

    }
}
