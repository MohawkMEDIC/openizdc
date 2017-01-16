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
 * Date: 2016-7-30
 */
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Services;
using SQLite.Net;
using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Connection
{
    /// <summary>
    /// SQLiteConnectionManager
    /// </summary>
    public class SQLiteConnectionManager : IDataConnectionManager
    {


        // connections
        private Dictionary<String, SQLiteConnectionWithLock> m_connections = new Dictionary<string, SQLiteConnectionWithLock>();

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SQLiteConnectionManager));
        // lock
        private static object s_lockObject = new object();
        // instance singleton
        private static SQLiteConnectionManager s_instance = null;

        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;

        /// <summary>
        /// Gets the current connection manager
        /// </summary>
        public static SQLiteConnectionManager Current
        {
            get
            {
                //if (s_instance == null)
                //    lock (s_lockObject)
                //        if (s_instance == null)
                //        {
                //            s_instance = new SQLiteConnectionManager();
                //            s_instance.Start();
                //        }
                return ApplicationContext.Current.GetService<SQLiteConnectionManager>();
            }
        }

        /// <summary>
        /// True if the daemon is running
        /// </summary>
        public bool IsRunning
        {
            get; private set;
        }

        ///// <summary>
        ///// Release the connection
        ///// </summary>
        ///// <param name="databasePath"></param>
        //public void ReleaseConnection(string databasePath)
        //{
        //    Object lockObject = null;
        //    if (!this.m_locks.TryGetValue(databasePath, out lockObject))
        //        return;
        //    else
        //        Monitor.Exit(lockObject);
        //}

        /// <summary>
        /// SQLLiteConnection manager
        /// </summary>
        public SQLiteConnectionManager()
        {
            s_instance = this;
            this.Start();
        }

        /// <summary>
        /// Get connection to the datafile
        /// </summary>
        public SQLiteConnectionWithLock GetConnection(String dataSource)
        {
            SQLiteConnectionWithLock conn = null;
            if (!this.IsRunning)
                throw new InvalidOperationException("Cannot get connection before daemon is started");

            try
            {
                if (!this.m_connections.TryGetValue(dataSource, out conn))
                    lock (s_lockObject)
                        if (!this.m_connections.TryGetValue(dataSource, out conn))
                        {

                            ISQLitePlatform platform = ApplicationContext.Current.GetService<ISQLitePlatform>();
                            conn = new SQLiteConnectionWithLock(platform, new SQLiteConnectionString(dataSource, true));
                            this.m_connections.Add(dataSource, conn);
                            this.m_tracer.TraceInfo("Connection to {0} established, {1} active connections", dataSource, this.m_connections.Count);
#if DEBUG_SQL
                conn.TraceListener = new TracerTraceListener();
#endif
                        }
                return conn;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error getting connection: {0}", e);
                if (conn != null)
                    Monitor.Exit(conn);
                throw;
            }
        }

        /// <summary>
        /// Start the connection manager
        /// </summary>
        public bool Start()
        {
            if (this.IsRunning) return true;
            this.Starting?.Invoke(this, EventArgs.Empty);
            this.Started?.Invoke(this, EventArgs.Empty);
            this.IsRunning = true;
            this.Started?.Invoke(this, EventArgs.Empty);
            return this.IsRunning;
        }

        /// <summary>
        /// Stop this service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            // disconnect all 
            foreach (var itm in this.m_connections)
            {
                this.m_tracer.TraceInfo("Closing connection {0}...", itm.Key);
                using (itm.Value.Lock())
                    itm.Value.Close();
            }
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }

    /// <summary>
    /// Tracer trace listener
    /// </summary>
    internal class TracerTraceListener : ITraceListener
    {
        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SQLiteConnectionWithLock));

        /// <summary>
        /// Trace info to console
        /// </summary>
        public void Receive(string message)
        {
            this.m_tracer.TraceVerbose(message);
        }
    }
}
