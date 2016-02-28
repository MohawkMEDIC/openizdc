using System;

namespace OpenIZ.Mobile.Core.Configuration.Data
{
	/// <summary>
	/// Identifies a database migration script in code
	/// </summary>
	public interface IDbMigration
	{


		/// <summary>
		/// Gets the identifier of the migration
		/// </summary>
		String Id {
			get;
		}


		/// <summary>
		/// A human readable description of the migration
		/// </summary>
		/// <value>The description.</value>
		String Description { get; }

		/// <summary>
		/// Install the migration package
		/// </summary>
		bool Install();

	}

}

