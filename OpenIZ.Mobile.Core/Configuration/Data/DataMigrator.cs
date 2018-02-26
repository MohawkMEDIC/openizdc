/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2017-9-1
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Resources;

namespace OpenIZ.Mobile.Core.Configuration.Data
{
	/// <summary>
	/// Represents a data migrator which is responsible for performing data migrations
	/// </summary>
	public class DataMigrator
	{

		// Tracer
		private Tracer m_tracer;

		// Migrations
		private List<IDbMigration> m_migrations;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Configuration.Data.DataMigrator"/> class.
		/// </summary>
		/// <param name="configuration">Configuration.</param>
		public DataMigrator ()
		{
			this.m_tracer = Tracer.GetTracer(this.GetType ());
			this.m_migrations = new List<IDbMigration> ();

			this.m_tracer.TraceInfo ("Scanning for data migrations...");

			// Scan for migrations 
			foreach (var dbm in typeof(DataMigrator).GetTypeInfo().Assembly.DefinedTypes) {
				try {
					if(dbm.AsType() == typeof(DataMigrator) ||
                        !typeof(IDbMigration).GetTypeInfo().IsAssignableFrom(dbm))
						continue;
					
					IDbMigration migration = Activator.CreateInstance (dbm.AsType()) as IDbMigration;
					if (migration != null) {
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
                ApplicationContext.Current.SetProgress(Strings.locale_setting_migration, 0);
				this.m_tracer.TraceVerbose ("Will Install {0}", m.Id);
				if (!m.Install ())
					throw new DataMigrationException (m);
				else
					ApplicationContext.Current?.Configuration.GetSection<DataConfigurationSection> ().MigrationLog.Entry.Add (new DataMigrationLog.DataMigrationEntry (m));
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
				var migrationLog = ApplicationContext.Current?.Configuration.GetSection<DataConfigurationSection> ().MigrationLog.Entry.Find (o => o.Id == itm.Id);
				this.m_tracer.TraceVerbose ("Migration {0} ... {1}", itm.Id, migrationLog == null ? "Install" : "Skip - Installed on " + migrationLog.Date.ToString ());
				if (migrationLog == null)
					retVal.Add (itm);
			}
			return retVal;
		}
	}
}

