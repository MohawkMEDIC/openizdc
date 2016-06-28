using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents a place in the local database
	/// </summary>
	[Table("place")]
	public class DbPlace : DbIdentified
    {
        /// <summary>
        /// Identifies whether the place is mobile
        /// </summary>
        [Column("isMobile")]
        public bool IsMobile { get; set; }

        /// <summary>
        /// Identifies the known latitude of the place
        /// </summary>
        [Column("lat")]
        public float Lat { get; set; }

        /// <summary>
        /// Identifies the known longitude of the place
        /// </summary>
        [Column("lng")]
        public float Lng { get; set; }

    }
}

