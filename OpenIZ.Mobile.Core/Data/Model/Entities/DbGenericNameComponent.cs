using SQLite;
using System;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
    /// <summary>
    /// Generic name component
    /// </summary>
    public abstract class DbGenericNameComponent : DbIdentified
    {

        /// <summary>
        /// Gets or sets the type of the component
        /// </summary>
        [Column("type"), MaxLength(16)]
        public byte[] ComponentTypeUuid { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Column("value"), NotNull, Indexed]
        public String Value
        {
            get;
            set;
        }
    }
}