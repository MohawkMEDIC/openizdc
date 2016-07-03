using System;
using OpenIZ.Mobile.Core.Diagnostics;
using SQLite;
using OpenIZ.Mobile.Core.Synchronization.Model;
using OpenIZ.Mobile.Core.Data.Model;

namespace OpenIZ.Mobile.Core.Configuration.Data.Migrations
{
	/// <summary>
	/// Initial queue catalog migration.
	/// </summary>
	public class InitialQueueCatalog : IDbMigration
	{
		#region IDbMigration implementation
		/// <summary>
		/// Install the migration package
		/// </summary>
		public bool Install ()
		{
			var tracer = Tracer.GetTracer (this.GetType ());

			// Database for the SQL Lite connection
			using (var db = new SQLiteConnection (ApplicationContext.Current?.Configuration.GetConnectionString (ApplicationContext.Current?.Configuration.GetSection<DataConfigurationSection> ().MessageQueueConnectionStringName).Value)) {

                // Migration log create and check
                db.CreateTable<DbMigrationLog>();
                if (db.Table<DbMigrationLog>().Count(o => o.MigrationId == this.Id) > 0)
                {
                    tracer.TraceInfo("Migration already installed");
                    return true;
                }
                else
                    db.Insert(new DbMigrationLog()
                    {
                        MigrationId = this.Id,
                        InstallationDate = DateTime.Now
                    });

                tracer.TraceInfo ("Installing queues...");
				db.CreateTable<InboundQueueEntry> ();
				db.CreateTable<OutboundQueueEntry> ();
				db.CreateTable<DeadLetterQueueEntry> ();
			}

			return true;
		}
		/// <summary>
		/// Gets the identifier of the migration
		/// </summary>
		/// <value>The identifier.</value>
		public string Id {
			get {
				return "000-init-openiz-algonquin-queue";
			}
		}
		/// <summary>
		/// A human readable description of the migration
		/// </summary>
		/// <value>The description.</value>
		public string Description {
			get {
				return "OpenIZ offline sync queue catalog";
			}
		}
		#endregion
	}
}

