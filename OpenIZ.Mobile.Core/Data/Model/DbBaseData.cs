using System;
using SQLite;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model.Security;

namespace OpenIZ.Mobile.Core.Data.Model
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
		public DateTime? CreationTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the time that the record was obsoleted
		/// </summary>
		/// <value>The obsoletion time.</value>
		[Column("obsoletion_time")]
		public DateTime? ObsoletionTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the updated time.
		/// </summary>
		/// <value>The updated time.</value>
		[Column("updated_time")]
		public DateTime? UpdatedTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the user that created the data
		/// </summary>
		/// <value>The created by identifier.</value>
		[Column("created_by"), NotNull, MaxLength(16)]
		public byte[] CreatedByUuid {
			get;
			set;
		}

		/// <summary>
		/// Obsoletion user
		/// </summary>
		/// <value>The obsoleted by identifier.</value>
		[Column("obsoleted_by"), MaxLength(16)]
		public byte[] ObsoletedByUuid { 
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the updated by identifier.
		/// </summary>
		/// <value>The updated by identifier.</value>
		[Column("updated_by"), MaxLength(16)]
		public byte[] UpdatedByUuid {
			get;
			set;
		}	

		/// <summary>
		/// Sets the created by key.
		/// </summary>
		/// <value>The created by key.</value>
		[Ignore]
		internal Guid CreatedByKey {
			get { return this.CreatedByUuid.ToGuid() ?? Guid.Empty; }
			set { this.CreatedByUuid = value.ToByteArray (); }
		}
			
		/// <summary>
		/// Gets or sets the updated by key.
		/// </summary>
		/// <value>The updated by key.</value>
		[Ignore]
		internal Guid? UpdatedByKey {
			get { return this.UpdatedByUuid?.ToGuid(); }
			set { this.UpdatedByUuid = value?.ToByteArray (); }
		}

		/// <summary>
		/// Gets or sets the obsoleted by key.
		/// </summary>
		/// <value>The obsoleted by key.</value>
		[Ignore]
		internal Guid? ObsoletedByKey {
			get { return this.CreatedByUuid?.ToGuid(); }
			set { this.ObsoletedByUuid = value?.ToByteArray (); }
		}
	}
}

