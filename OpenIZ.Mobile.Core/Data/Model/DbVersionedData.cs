using System;
using SQLite;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model.Security;

namespace OpenIZ.Mobile.Core.Data.Model
{
	/// <summary>
	/// Versioned data
	/// </summary>
	public abstract class DbVersionedData : DbBaseData
	{
		/// <summary>
		/// Gets or sets the server version UUID, this is used to ensure that the version on a server
		/// equals the version here
		/// </summary>
		/// <value>The version UUID.</value>
		[Column("version_uuid"), MaxLength(16), NotNull]
		public byte[] VersionUuid
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the version key.
		/// </summary>
		/// <value>The version key.</value>
		[Ignore]
		public Guid VersionKey
		{
			get { return this.VersionUuid == null ? Guid.Empty : new Guid (this.VersionUuid); }
			set { this.VersionUuid = value.ToByteArray (); }
		}

        /// <summary>
        /// Replace previous version uuid
        /// </summary>
        [Column("replace_version_uuid"), MaxLength(16)]
        public byte[] PreviousVersionUuid { get; set; }


        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        /// <value>The version key.</value>
        [Ignore]
        public Guid PreviousVersionKey
        {
            get { return this.PreviousVersionUuid == null ? Guid.Empty : new Guid(this.PreviousVersionUuid); }
            set { this.PreviousVersionUuid = value.ToByteArray(); }
        }
    }
}

