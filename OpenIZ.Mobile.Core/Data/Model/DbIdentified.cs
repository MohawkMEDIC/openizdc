using System;
using SQLite;
using OpenIZ.Core.Model;

namespace OpenIZ.Mobile.Core.Data.Model
{
	/// <summary>
	/// Represents data that is identified in some way
	/// </summary>
	public abstract class DbIdentified
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Data.Model.Model.DbIdentified"/> class.
		/// </summary>
		public DbIdentified ()
		{
			this.Uuid = Guid.NewGuid ().ToByteArray ();
		}

		/// <summary>
		/// Gets or sets the universal identifier for the object
		/// </summary>
		[PrimaryKey, Column("uuid"), MaxLength(16), Indexed, NotNull]
		public byte[] Uuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the key (GUID) on the persistence item
		/// </summary>
		/// <value>The key.</value>
		[Ignore]
		public Guid Key
		{
			get { return this.Uuid.ToGuid() ?? Guid.Empty; }
			set { this.Uuid = value.ToByteArray (); }
		}

	}
}

