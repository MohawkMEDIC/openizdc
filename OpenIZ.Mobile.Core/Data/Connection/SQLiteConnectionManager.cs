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
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Resources;
using OpenIZ.Mobile.Core.Services;
using SQLite.Net;
using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.Diagnostics;

namespace OpenIZ.Mobile.Core.Data.Connection
{

    /// <summary>
    /// SQLiteConnectionManager
    /// </summary>
    public class SQLiteConnectionManager : IDataConnectionManager
    {

        // Connection pool
        private List<LockableSQLiteConnection> m_connectionPool = new List<LockableSQLiteConnection>();

        /// <summary>
        /// Un-register a readonly connection
        /// </summary>
        internal void UnregisterReadonlyConnection(ReadonlySQLiteConnection conn)
        {
            this.UnregisterConnection(conn);
        }

        /// <summary>
        /// Unregister connection
        /// </summary>
        private void UnregisterConnection(LockableSQLiteConnection conn)
        {
            List<LockableSQLiteConnection> connections = this.GetOrRegisterConnections(conn.DatabasePath);
            lock (s_lockObject)
            {
                Monitor.Exit(conn);
                connections.Remove(conn);

                // Add connection back onto the pool
                if (conn.Persistent)
                    this.m_connectionPool.Add(conn);

                this.m_tracer.TraceVerbose("-- {0} ({1})", conn.DatabasePath, connections.Count);

            }
        }

        /// <summary>
        /// Un-register a readonly connection
        /// </summary>
        internal void RegisterReadonlyConnection(ReadonlySQLiteConnection conn)
        {
            List<LockableSQLiteConnection> connections = this.GetOrRegisterConnections(conn.DatabasePath);

            // Are there other connections that this thread owns?
            if (!connections.Any(o => Monitor.IsEntered(o))) // then we must adhere to traffic jams
            {
                var mre = this.GetOrRegisterResetEvent(conn.DatabasePath);
                mre.WaitOne();
                this.RegisterConnection(conn);

            }
        }

        /// <summary>
        /// Register connection
        /// </summary>
        private void RegisterConnection(LockableSQLiteConnection conn)
        {
            List<LockableSQLiteConnection> connections = this.GetOrRegisterConnections(conn.DatabasePath);
            lock (s_lockObject)
            {
                connections.Add(conn);
                this.m_connectionPool.Remove(conn); // Just in-case
                // Lock this connection so I know if I can bypass later
                Monitor.Enter(conn);

                this.m_tracer.TraceVerbose("++ {0} ({1})", conn.DatabasePath, connections.Count);
            }
        }

        /// <summary>
        /// Gets or registers a connection pool
        /// </summary>
        private List<LockableSQLiteConnection> GetOrRegisterConnections(String databasePath)
        {
            List<LockableSQLiteConnection> retVal = null;
            if (!this.m_readonlyConnections.TryGetValue(databasePath, out retVal))
            {
                retVal = new List<LockableSQLiteConnection>();
                lock (s_lockObject)
                    if (!this.m_readonlyConnections.ContainsKey(databasePath))
                        this.m_readonlyConnections.Add(databasePath, retVal);
                    else
                        retVal = this.m_readonlyConnections[databasePath];
            }
            return retVal;
        }

        /// <summary>
        /// Un-register a readonly connection
        /// </summary>
        internal void UnregisterWriteConnection(WriteableSQLiteConnection conn)
        {
            var mre = this.GetOrRegisterResetEvent(conn.DatabasePath);
            mre.Set();
            this.UnregisterConnection(conn);
        }

        /// <summary>
        /// Un-register a readonly connection
        /// </summary>
        internal void RegisterWriteConnection(WriteableSQLiteConnection conn)
        {
            var mre = this.GetOrRegisterResetEvent(conn.DatabasePath);
            var connections = this.GetOrRegisterConnections(conn.DatabasePath);
            mre.Reset();
            // Wait for readonly connections to go to 0
            while (connections.Count(o=>!Monitor.IsEntered(o)) > 0)
                Task.Delay(100).Wait();

            if(connections.Count == 0)
                this.RegisterConnection(conn);

        }

        /// <summary>
        /// Gets or sets the reset event for the particular database
        /// </summary>
        private ManualResetEvent GetOrRegisterResetEvent(string databasePath)
        {
            ManualResetEvent retVal = null;
            if (!this.m_connections.TryGetValue(databasePath, out retVal))
            {
                retVal = new ManualResetEvent(true);
                lock (s_lockObject)
                    if (!this.m_connections.ContainsKey(databasePath))
                        this.m_connections.Add(databasePath, retVal);
                    else
                        retVal = this.m_connections[databasePath];
            }
            return retVal;
        }

        // connections
        private Dictionary<String, ManualResetEvent> m_connections = new Dictionary<string, ManualResetEvent>();

        // Readonly connections
        private Dictionary<String, List<LockableSQLiteConnection>> m_readonlyConnections = new Dictionary<string, List<LockableSQLiteConnection>>();

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
        /// Get a readonly connection
        /// </summary>
        public LockableSQLiteConnection GetReadonlyConnection(String dataSource)
        {
            //return this.GetConnection(dataSource);

            if (!this.IsRunning)
                throw new InvalidOperationException("Cannot get connection before daemon is started");

            // Are there any connections that are open by this source and thread?
            try
            {
                var retVal = this.GetOrCreatePooledConnection(dataSource, true);
                this.m_tracer.TraceInfo("Readonly connection to {0} established, {1} active connections", dataSource, this.m_connections.Count + this.m_readonlyConnections.Count);
                
#if DEBUG_SQL
                conn.TraceListener = new TracerTraceListener();
#endif
                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error getting connection: {0}", e);
                throw;
            }

        }

        /// <summary>
        /// Get or create a pooled connection
        /// </summary>
        private LockableSQLiteConnection GetOrCreatePooledConnection(string dataSource, bool isReadonly)
        {
            // First is there a connection already?
            var connections = this.GetOrRegisterConnections(dataSource);
            var conn = connections.FirstOrDefault(o => Monitor.IsEntered(o));
            if (conn != null) return conn;

            // Next is there a pooled connection that we can take off?
            if (isReadonly)
            {
                var mre = this.GetOrRegisterResetEvent(dataSource);
                mre.WaitOne();
            }

            ISQLitePlatform platform = ApplicationContext.Current.GetService<ISQLitePlatform>();
            
            lock(s_lockObject)
            {
                LockableSQLiteConnection retVal = null;
                if (isReadonly)
                    retVal = this.m_connectionPool.OfType<ReadonlySQLiteConnection>().FirstOrDefault(o => o.DatabasePath == dataSource);
                else // Writeable connection can only have one in the pool so if it isn't there make sure it isn't in the current 
                { 
                    retVal = this.m_connectionPool.OfType<WriteableSQLiteConnection>().FirstOrDefault(o => o.DatabasePath == dataSource);
                }

                // Remove return value
                if (retVal != null)
                    this.m_connectionPool.Remove(retVal);
                else if (isReadonly)
                    retVal = new ReadonlySQLiteConnection(platform, dataSource, SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex) { Persistent = true };
                else
                {
                    // There aren't any open connections so we create one!
                    retVal = connections.OfType<WriteableSQLiteConnection>().FirstOrDefault(o => o.DatabasePath == dataSource);
                    if (retVal == null)
                        retVal = new WriteableSQLiteConnection(platform, dataSource, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create) { Persistent = true };
                    else // There is an active connection we should use it
                        retVal.Persistent = true;
                }
                return retVal;
            }
        }

        /// <summary>
        /// Get connection to the datafile
        /// </summary>
        public LockableSQLiteConnection GetConnection(String dataSource)
        {
            if (!this.IsRunning)
                throw new InvalidOperationException("Cannot get connection before daemon is started");
            
            try
            {
                ISQLitePlatform platform = ApplicationContext.Current.GetService<ISQLitePlatform>();
                var retVal = this.GetOrCreatePooledConnection(dataSource, false);
                this.m_tracer.TraceInfo("Write connection to {0} established, {1} active connections", dataSource, this.m_connections.Count + this.m_readonlyConnections.Count);
#if DEBUG_SQL
                conn.TraceListener = new TracerTraceListener();
#endif
                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error getting connection: {0}", e);
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

            // Wait for all write connections to finish up
            foreach (var mre in this.m_connections)
                mre.Value.WaitOne();

            // Close all readonly connections
            foreach (var itm in this.m_readonlyConnections)
            {
                this.m_tracer.TraceInfo("Waiting for connections to {0} to finish up...", itm.Key);
                while (itm.Value.Count > 0)
                    Task.Delay(100).Wait();
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Compact all items 
        /// </summary>
        public void Compact()
        {
            // Let other threads know they can't open a r/o connection for each db
            try
            {

                for (var i = 0; i < this.m_connections.Count; i++)
                {
                    var itm = this.m_connections.ElementAt(i);
                    using (var conn = this.GetConnection(itm.Key))
                    using (conn.Lock())
                    {
                        ApplicationContext.Current.SetProgress(Strings.locale_compacting, (i * 3 + 0) / (this.m_connections.Count * 3.0f));
                        conn.Execute("VACUUM");
                        ApplicationContext.Current.SetProgress(Strings.locale_compacting, (i * 3 + 1) / (this.m_connections.Count * 3.0f));
                        conn.Execute("REINDEX");
                        ApplicationContext.Current.SetProgress(Strings.locale_compacting, (i * 3 + 2) / (this.m_connections.Count * 3.0f));
                        conn.Execute("ANALYZE");
                    }
                }
            }
            finally
            {
            }
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
