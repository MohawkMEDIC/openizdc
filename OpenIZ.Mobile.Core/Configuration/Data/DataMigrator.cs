using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Diagnostics;

namespace OpenIZ.Mobile.Core.Configuration.Data
{
	/// <summary>
	/// Represents a data migrator which is responsible for performing data migrations
	/// </summary>
	public class DataMigrator
	{

		// Tracer
		private Tracer m_tracer;

		// Configuration
		private OpenIZConfiguration m_configuration;

		// Migrations
		private List<IDbMigration> m_migrations;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Configuration.Data.DataMigrator"/> class.
		/// </summary>
		/// <param name="configuration">Configuration.</param>
		public DataMigrator (OpenIZConfiguration configuration)
		{
			this.m_tracer = Tracer.CreateTracer (this.GetType (), configuration);
			this.m_configuration = configuration;
			this.m_migrations = new List<IDbMigration> ();

			this.m_tracer.TraceInfo ("Scanning for data migrations...");

			// Scan for migrations 
			foreach (var dbm in typeof(DataMigrator).GetTypeInfo().Assembly.ExportedTypes) {
				try {
					IDbMigration migration = Activator.CreateInstance (dbm) as IDbMigration;
					if (migration != null) {
						migration.Configuration = configuration;
						this.m_tracer.TraceVerbose ("Found data migrator {0}...", migration.Id);
						this.m_migrations.Add (migration);
					}
				} catch {
				}
			}
		}

		/// <summary>
		/// Assert that all data migrations have occurred
		/// </summary>
		public void Ensure ()
		{

			this.m_tracer.TraceInfo ("Ensuring database is up to date");
			// Migration order
			foreach (var m in this.GetProposal()) {
				this.m_tracer.TraceVerbose ("Will Install {0}", m.Id);
				if (!m.Install ())
					throw new DataMigrationException (m);
				else
					this.m_configuration.GetSection<DataConfigurationSection> ().MigrationLog.Entry.Add (new DataMigrationLog.DataMigrationEntry (m));
			}

		}

		/// <summary>
		/// Get the list of data migrations that need to occur for the application to be in the most recent state
		/// </summary>
		/// <returns>The proposal.</returns>
		public List<IDbMigration> GetProposal ()
		{
			List<IDbMigration> retVal = new List<IDbMigration> ();

			this.m_tracer.TraceInfo ("Generating data migration proposal...");
			foreach (var itm in this.m_migrations.OrderBy(o=>o.Id)) {
				var migrationLog = this.m_configuration.GetSection<DataConfigurationSection> ().MigrationLog.Entry.Find (o => o.Id == itm.Id);
				this.m_tracer.TraceVerbose ("Migration {0} ... {1}", itm.Id, migrationLog == null ? "Install" : "Skip - Installed on " + migrationLog.Date.ToString ());
				if (migrationLog == null)
					retVal.Add (itm);
			}
			return retVal;
		}
	}
}

