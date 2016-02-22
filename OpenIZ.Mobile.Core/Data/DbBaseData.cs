using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data
{
	/// <summary>
	/// Represents data which has authorship information attached
	/// </summary>
	public abstract class DbBaseData : DbIdentified
	{
		/// <summary>
		/// Gets or sets the creation time
		/// </summary>
		/// <value>The creation time.</value>
		[Column("creation_time"), NotNull]
		public DateTime CreationTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the time that the record was obsoleted
		/// </summary>
		/// <value>The obsoletion time.</value>
		[Column("obsoletion_time")]
		public DateTime ObsoletionTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the updated time.
		/// </summary>
		/// <value>The updated time.</value>
		[Column("updated_time")]
		public DateTime UpdatedTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the user that created the data
		/// </summary>
		/// <value>The created by identifier.</value>
		[Column("created_by"), NotNull]
		public int CreatedById {
			get;
			set;
		}

		/// <summary>
		/// Obsoletion user
		/// </summary>
		/// <value>The obsoleted by identifier.</value>
		[Column("obsoleted_by")]
		public int ObsoletedById { 
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the updated by identifier.
		/// </summary>
		/// <value>The updated by identifier.</value>
		[Column("updated_by")]
		public int UpdatedById {
			get;
			set;
		}	}
}

