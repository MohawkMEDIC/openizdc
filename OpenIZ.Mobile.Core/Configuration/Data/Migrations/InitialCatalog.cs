using System;
using SQLite;
using OpenIZ.Mobile.Core.Data.Concepts;
using OpenIZ.Mobile.Core.Diagnostics;

namespace OpenIZ.Mobile.Core.Configuration.Data.Migrations
{
	/// <summary>
	/// This class is responsible for setting up an initial catalog of items in the SQL Lite database
	/// </summary>
	public class InitialCatalog : IDbMigration
	{
		
		#region IDbMigration implementation

		/// <summary>
		/// Install the initial catalog
		/// </summary>
		public bool Install ()
		{

			if (this.Configuration == null)
				throw new InvalidOperationException ("Configuration not set");

			var tracer = Tracer.CreateTracer (this.GetType (), this.Configuration);

			// Database for the SQL Lite connection
			using (var db = new SQLiteConnection (this.Configuration.GetConnectionString(this.Configuration.GetSection<DataConfigurationSection>().MainDataSourceConnectionStringName).Value)) {

				db.TableChanged += (s, e) => tracer.TraceInfo ("Updating {0}", e.Table.TableName);
				// Create tables
				tracer.TraceVerbose("Installing Concepts...");
				db.CreateTable<Concept> (CreateFlags.None);

			}
			return true;
		}


		/// <summary>
		/// Configuration identifier
		/// </summary>
		/// <value>The identifier.</value>
		public string Id {
			get {
				return "000-init-openiz-algonquin";
			}
		}

		/// <summary>
		/// A human readable description of the migration
		/// </summary>
		/// <value>The description.</value>
		public string Description {
			get {
				return "OpenIZ Mobile Algonquin (0.1.0.0) data model";
			}
		}

		/// <summary>
		/// Gets or sets the configuration
		/// </summary>
		public OpenIZConfiguration Configuration { get; set; }


		#endregion
	}
}

