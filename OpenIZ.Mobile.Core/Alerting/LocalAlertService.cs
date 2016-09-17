using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Map;
using System.Reflection;
using OpenIZ.Core.Alert.Alerting;

namespace OpenIZ.Mobile.Core.Alerting
{
    /// <summary>
    /// Represents a local alerting service
    /// </summary>
    public class LocalAlertService : IAlertService
    {
        // Mapper
        private static ModelMapper s_mapper = new ModelMapper(typeof(LocalAlertService).GetTypeInfo().Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Alerting.ModelMap.xml"));

        // Connection string
        private string m_connectionString = ApplicationContext.Current.Configuration.GetConnectionString(
            ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().MessageQueueConnectionStringName).Value;

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalAlertService));

        public event EventHandler<AlertEventArgs> Received;
        public event EventHandler<AlertEventArgs> Committed;


        /// <summary>
        /// Get an alert from the storage
        /// </summary>
        public AlertMessage GetAlert(Guid id)
        {
            var idKey = id.ToByteArray();
            var conn = SQLiteConnectionManager.Current.GetConnection(this.m_connectionString);
            using (conn.Lock())
                return conn.Table<DbAlertMessage>().Where(o => o.Id == idKey).FirstOrDefault()?.ToAlert();
        }

        /// <summary>
        /// Get alerts matching
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<AlertMessage> FindAlerts(Expression<Func<AlertMessage, bool>> predicate, int offset, int? count)
        {
            try
            {
                var conn = SQLiteConnectionManager.Current.GetConnection(this.m_connectionString);
                using (conn.Lock())
                {
                    var dbPredicate = s_mapper.MapModelExpression<AlertMessage, DbAlertMessage>(predicate);
                    return conn.Table<DbAlertMessage>().Where(dbPredicate).Skip(offset).Take(count ?? 100).OrderByDescending(o => o.TimeStamp).ToList().Select(o=>o.ToAlert()).ToList();
                }
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error searching alerts {0}: {1}", predicate, e);
                throw;
            }
        }

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

                this.SaveAlert(msg);

                // Committed
                this.Committed?.BeginInvoke(this, args, null, null);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error broadcasting alert: {0}", e);
            }
        }

        /// <summary>
        /// Save the alert without notifying anyone
        /// </summary>
        public void SaveAlert(AlertMessage alert)
        {
            var conn = SQLiteConnectionManager.Current.GetConnection(this.m_connectionString);
            using (conn.Lock())
                try
                {
                    if (alert.Flags.HasFlag(AlertMessageFlags.Transient)) return; // Transient messages don't get saved

                    var msg = new DbAlertMessage(alert);

                    this.m_tracer.TraceVerbose("Saving alert {0}", msg);
                    conn.BeginTransaction();

                    if(!conn.TableMappings.Any(o=>o.MappedType == typeof(DbAlertMessage)))
                        conn.CreateTable<DbAlertMessage>();

                    // Check for key and assign ID
                    try
                    {
						conn.Update(msg);
                    }
                    catch(SQLite.Net.SQLiteException)
                    {
						msg.Id = Guid.NewGuid().ToByteArray();
						msg.CreatedBy = ApplicationContext.Current.Principal?.Identity?.Name;
						conn.Insert(msg);
					}

                    conn.Commit();

                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error saving alert: {0}", e);
                    conn.Rollback();
                }
        }
    }
}
