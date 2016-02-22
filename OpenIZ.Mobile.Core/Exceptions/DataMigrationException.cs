using System;
using OpenIZ.Mobile.Core.Configuration.Data;

namespace OpenIZ.Mobile.Core.Exceptions
{
	/// <summary>
	/// Data migration exception
	/// </summary>
	public class DataMigrationException : Exception
	{

		/// <summary>
		/// Gets or sets the migration id
		/// </summary>
		/// <value>The migration identifier.</value>
		public String MigrationId {
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Exceptions.DataMigrationException"/> class.
		/// </summary>
		public DataMigrationException (IDbMigration offender) : this(offender, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Exceptions.DataMigrationException"/> class.
		/// </summary>
		public DataMigrationException (IDbMigration offender, Exception inner) : base(String.Format("Migration of {0} failed", offender.Id), inner)
		{
			this.MigrationId = offender.Id;
		}

	}
}

