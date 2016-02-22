using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data
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

	}
}

