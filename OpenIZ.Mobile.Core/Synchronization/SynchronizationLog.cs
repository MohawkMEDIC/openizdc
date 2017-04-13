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
 * User: justi
 * Date: 2016-7-30
 */
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
            using (conn.Lock())
            {
                var modelAqn = modelType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;
                var logEntry = conn.Table<SynchronizationLogEntry>().Where(o => o.ResourceType == modelAqn && o.Filter == filter).FirstOrDefault();
                return logEntry?.LastSync.ToLocalTime();
            }
        }

        /// <summary>
        /// Get the last successful etag
        /// </summary>
        public String GetLastEtag(Type modelType, String filter = null)
        {
            var conn = this.CreateConnection();
            using (conn.Lock())
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
            using (conn.Lock())
            {
                var modelAqn = modelType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;
                var logEntry = conn.Table<SynchronizationLogEntry>().Where(o => o.ResourceType == modelAqn && o.Filter == filter).FirstOrDefault();
                if (logEntry == null)
                    conn.Insert(new SynchronizationLogEntry() { ResourceType = modelAqn, Filter = filter, LastETag = eTag, LastSync = DateTime.Now });
                else
                {
                    logEntry.LastSync = DateTime.Now;
                    if (!String.IsNullOrEmpty(eTag))
                        logEntry.LastETag = eTag;
                    conn.Update(logEntry);
                }
            }
        }

        /// <summary>
        /// Get all synchronizations
        /// </summary>
        public List<SynchronizationLogEntry> GetAll()
        {
            var conn = this.CreateConnection();
            using (conn.Lock())
            {
                return conn.Table<SynchronizationLogEntry>().ToList();
            }
        }

        /// <summary>
        /// Save the query state so that it can come back if the connection is lost
        /// </summary>
        public void SaveQuery(Type modelType, String filter, Guid queryId, int offset)
        {

            var conn = this.CreateConnection();
            using (conn.Lock())
            {
                try
                {
                    var qid = queryId.ToByteArray();
                    var currentQuery = conn.Table<SynchronizationQuery>().Where(o=>o.Uuid == qid).FirstOrDefault();
                    if (currentQuery == null)
                    {
                        var modelAqn = modelType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

                        conn.Insert(new SynchronizationQuery()
                        {
                            Uuid = queryId.ToByteArray(),
                            Filter = filter,
                            LastSuccess = offset,
                            StartTime = DateTime.Now,
                            ResourceType = modelAqn
                        });
                    }
                    else
                    {
                        currentQuery.LastSuccess = offset;
                        conn.Update(currentQuery);
                    }
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error saving query data {0} : {1}", queryId, e);
                    throw;
                }
            }

        }

        /// <summary>
        /// Complete query 
        /// </summary>
        public void CompleteQuery(Guid queryId)
        {
            var conn = this.CreateConnection();
            using (conn.Lock())
            {
                try
                {
                    var qid = queryId.ToByteArray();
                    conn.Table<SynchronizationQuery>().Delete(o=>o.Uuid == qid);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error deleting query data {0} : {1}", queryId, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Find query data
        /// </summary>
        public SynchronizationQuery FindQueryData(Type modelType, String filter)
        {
            var conn = this.CreateConnection();
            using (conn.Lock())
            {
                try
                {
                    var modelAqn = modelType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;
                    return conn.Table<SynchronizationQuery>().Where(o=>o.ResourceType == modelAqn && o.Filter == filter).FirstOrDefault();
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error fetching query data {0} : {1}", modelType, e);
                    throw;
                }
            }
        }
    }
}
