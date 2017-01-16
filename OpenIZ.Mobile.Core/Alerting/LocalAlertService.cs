/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-11-14
 */
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenIZ.Mobile.Core.Alerting
{
	/// <summary>
	/// Represents a local alerting service
	/// </summary>
	public class LocalAlertService : IAlertRepositoryService
	{
		// Mapper
		private static ModelMapper s_mapper = new ModelMapper(typeof(LocalAlertService).GetTypeInfo().Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Alerting.ModelMap.xml"));

		// Connection string
		private string m_connectionString = ApplicationContext.Current.Configuration.GetConnectionString(
			ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().MessageQueueConnectionStringName).Value;

		// Tracer
		private Tracer m_tracer = Tracer.GetTracer(typeof(LocalAlertService));

		public event EventHandler<AlertEventArgs> Committed;

		public event EventHandler<AlertEventArgs> Received;

		/// <summary>
		/// Broadcast alert
		/// </summary>
		public void BroadcastAlert(AlertMessage msg)
		{
			try
			{
				this.m_tracer.TraceVerbose("Broadcasting alert {0}", msg);

				// Broadcast alert
				// TODO: Fix this, this is bad
				var args = new AlertEventArgs(msg);
				this.Received?.Invoke(this, args);
				if (args.Ignore)
					return;

				this.Save(msg);

				// Committed
				this.Committed?.BeginInvoke(this, args, null, null);
			}
			catch (Exception e)
			{
				this.m_tracer.TraceError("Error broadcasting alert: {0}", e);
			}
		}

		/// <summary>
		/// Get alerts matching
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public IEnumerable<AlertMessage> Find(Expression<Func<AlertMessage, bool>> predicate, int offset, int? count, out int totalCount)
		{
			try
			{
				var conn = SQLiteConnectionManager.Current.GetConnection(this.m_connectionString);
				using (conn.Lock())
				{
					var dbPredicate = s_mapper.MapModelExpression<AlertMessage, DbAlertMessage>(predicate);

					var results = conn.Table<DbAlertMessage>().Where(dbPredicate).Skip(offset).Take(count ?? 100).OrderByDescending(o => o.TimeStamp).ToList().Select(o => o.ToAlert());
					totalCount = results.Count();

					return results;
				}
			}
			catch (Exception e)
			{
				this.m_tracer.TraceError("Error searching alerts {0}: {1}", predicate, e);
				throw;
			}
		}

		/// <summary>
		/// Get an alert from the storage
		/// </summary>
		public AlertMessage Get(Guid id)
		{
			var idKey = id.ToByteArray();
			var conn = SQLiteConnectionManager.Current.GetConnection(this.m_connectionString);
			using (conn.Lock())
				return conn.Table<DbAlertMessage>().Where(o => o.Id == idKey).FirstOrDefault()?.ToAlert();
		}

		/// <summary>
		/// Inserts an alert message.
		/// </summary>
		/// <param name="message">The alert message to be inserted.</param>
		/// <returns>Returns the inserted alert.</returns>
		public AlertMessage Insert(AlertMessage message)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AlertMessage>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<AlertMessage>)));
			}

			AlertMessage alert = persistenceService.Insert(message);
			this.Received?.Invoke(this, new AlertEventArgs(alert));

			return alert;
		}

		/// <summary>
		/// Save the alert without notifying anyone
		/// </summary>
		public AlertMessage Save(AlertMessage alert)
		{
			var conn = SQLiteConnectionManager.Current.GetConnection(this.m_connectionString);
			using (conn.Lock())
			{
				try
				{
					// Transient messages don't get saved
					if (alert.Flags.HasFlag(AlertMessageFlags.Transient))
					{
						return alert;
					}


					var msg = new DbAlertMessage(alert);

					this.m_tracer.TraceVerbose("Saving alert {0}", msg);
					conn.BeginTransaction();

					if (!conn.TableMappings.Any(o => o.MappedType == typeof(DbAlertMessage)))
					{
						conn.CreateTable<DbAlertMessage>();
					}

					// Check for key and assign ID
					try
					{
						var existingAlert = this.Get(new Guid(msg.Id));

						if (existingAlert == null)
						{
							msg.Id = Guid.NewGuid().ToByteArray();
							msg.CreatedBy = AuthenticationContext.Current.Principal?.Identity?.Name;
							conn.Insert(msg);
						}
						else
						{
							conn.Update(msg);
						}

						conn.Commit();
					}
					catch (SQLite.Net.SQLiteException e)
					{
						this.m_tracer.TraceError("Error saving alert: {0}", e);
						throw;
					}
				}
				catch (Exception e)
				{
					this.m_tracer.TraceError("Error saving alert: {0}", e);
					conn.Rollback();
				}
			}

			return alert;
		}
	}
}