using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data
{
	/// <summary>
	/// Represents data that is identified in some way
	/// </summary>
	public class IdentifiedData
	{

		/// <summary>
		/// Gets or sets the identifier of the identified data
		/// </summary>
		/// <value>The identifier.</value>
		[PrimaryKey, AutoIncrement, Column("id")]
		public int Id {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the universal identifier for the object
		/// </summary>
		[Column("uuid"), MaxLength(16), Indexed]
		public byte[] Uuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the creation time
		/// </summary>
		/// <value>The creation time.</value>
		[Column("creation_time")]
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
		/// Gets or sets the user that created the data
		/// </summary>
		/// <value>The created by identifier.</value>
		[Column("created_by")]
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
		/// Gets or sets the server version UUID, this is used to ensure that the version on a server
		/// equals the version here
		/// </summary>
		/// <value>The version UUID.</value>
		[Column("version_uuid"), MaxLength(16)]
		public byte[] VersionUuid
		{
			get;
			set;
		}


	}
}

