/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-6-14
 */
using System;
using OpenIZ.Mobile.Core.Diagnostics;
using SQLite.Net;
using OpenIZ.Mobile.Core.Synchronization.Model;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Alerting;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Resources;
using OpenIZ.Core.Alert.Alerting;

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
            var db = SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current?.Configuration.GetConnectionString(ApplicationContext.Current?.Configuration.GetSection<DataConfigurationSection>().MessageQueueConnectionStringName).Value);
                using(db.Lock())
            {

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

                tracer.TraceInfo("Installing queues...");
                db.CreateTable<InboundQueueEntry>();
                db.CreateTable<OutboundQueueEntry>();
                db.CreateTable<DeadLetterQueueEntry>();
                db.CreateTable<SynchronizationLogEntry>();
                db.CreateTable<DbAlertMessage>();
                new LocalAlertService()?.SaveAlert(new AlertMessage()
                {
                    Body = Strings.locale_welcomeMessageBody,
                    CreatedBy = ApplicationContext.Current.Principal?.Identity?.Name,
                    From = "OpenIZ Team",
                    Flags = AlertMessageFlags.None,
                    Subject = Strings.locale_welcomeMessageSubject,
                    TimeStamp = DateTime.Now
                });
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

