using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Synchronization.Model;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Synchronization
{
    /// <summary>
    /// Represents the synchronization log
    /// </summary>
    public class SynchronizationLog
    {
        // Get the tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SynchronizationLog));

        // Object sync
        private object m_syncObject = new object();

        // The log instance
        private static SynchronizationLog s_instance;

        /// <summary>
		/// Singleton
		/// </summary>
		/// <value>The current.</value>
		public static SynchronizationLog Current
        {
            get
            {
                if (s_instance == null)
                    s_instance = new SynchronizationLog();
                return s_instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Synchronization.SynchronizationQueue`1"/> class.
        /// </summary>
        private SynchronizationLog()
        {
        }


        /// <summary>
        /// Create a connection
        /// </summary>
        /// <returns>The connection.</returns>
        private SQLiteConnectionWithLock CreateConnection()
        {
            return SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current.Configuration.GetConnectionString(
                ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().MessageQueueConnectionStringName
            ).Value);
        }

        /// <summary>
        /// Get the last successful modification time of an object retrieved
        /// </summary>
        public DateTime? GetLastTime(Type modelType, String filter = null)
        {
            var conn = this.CreateConnection();
                using(conn.Lock())
            {
                var modelAqn = modelType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;
                var logEntry = conn.Table<SynchronizationLogEntry>().Where(o => o.ResourceType == modelAqn && o.Filter == filter).FirstOrDefault();
                return logEntry?.LastSync;
            }
        }

        /// <summary>
        /// Get the last successful etag
        /// </summary>
        public String GetLastEtag(Type modelType, String filter = null)
        {
            var conn = this.CreateConnection();
                using(conn.Lock())
            {
                var modelAqn = modelType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;
                var logEntry = conn.Table<SynchronizationLogEntry>().Where(o => o.ResourceType == modelAqn && o.Filter == filter).FirstOrDefault();
                return logEntry?.LastETag;
            }
        }

        /// <summary>
        /// Save the sync log entry
        /// </summary>
        public void Save(Type modelType, String filter, String eTag)
        {
            var conn = this.CreateConnection();
                using(conn.Lock())
            {
                var modelAqn = modelType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;
                var logEntry = conn.Table<SynchronizationLogEntry>().Where(o => o.ResourceType == modelAqn && o.Filter == filter).FirstOrDefault();
                if (logEntry == null)
                    conn.Insert(new SynchronizationLogEntry() { ResourceType = modelAqn, Filter = filter, LastETag = eTag, LastSync = DateTime.Now });
                else
                {
                    logEntry.LastSync = DateTime.Now;
                    logEntry.LastETag = eTag;
                    conn.Update(logEntry);
                }
            }
        }
    }
}
