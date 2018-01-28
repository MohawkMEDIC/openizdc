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
    /// Represents a wrapper class which closes a connection when lock is released
    /// </summary>
    public class ReadonlySQLiteConnection : LockableSQLiteConnection
    {

        public ReadonlySQLiteConnection(ISQLitePlatform sqlitePlatform, String databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true, IBlobSerializer serializer = null, IDictionary<String, TableMapping> tableMappings = null, IDictionary<Type, String> extraTypeMappings = null, IContractResolver resolver = null) :
            base(sqlitePlatform, databasePath, openFlags, storeDateTimeAsTicks, serializer, tableMappings, extraTypeMappings, resolver)
        {
        }

        /// <summary>
        /// Wrapper lock
        /// </summary>
        private class SQLiteConnectionWrapperLock : IDisposable
        {
            // Connection
            private ReadonlySQLiteConnection m_connection;
            private static Dictionary<String, Object> m_lockBox = new Dictionary<string, object>();

            /// <summary>
            /// Wrapper with lock
            /// </summary>
            public SQLiteConnectionWrapperLock(ReadonlySQLiteConnection wrappedConnection)
            {
                this.m_connection = wrappedConnection;
                Object lockObject = null;
                if (!m_lockBox.TryGetValue(wrappedConnection.DatabasePath, out lockObject))
                {
                    lockObject = new object();
                    lock (m_lockBox)
                        if (!m_lockBox.ContainsKey(wrappedConnection.DatabasePath))
                            m_lockBox.Add(wrappedConnection.DatabasePath, lockObject);
                        else
                            lockObject = m_lockBox[wrappedConnection.DatabasePath];
                }

                Monitor.Enter(lockObject);
                SQLiteConnectionManager.Current.RegisterReadonlyConnection(this.m_connection);
                this.m_connection.m_lockCount++;

            }

            /// <summary>
            /// Dispose
            /// </summary>
            public void Dispose()
            {
                this.m_connection.m_lockCount--;

                if (this.m_connection.m_lockCount == 0)
                {
                    if (!this.m_connection.Persistent)
                    {
                        this.m_connection.Close();
                        this.m_connection.Dispose();
                    }
                    SQLiteConnectionManager.Current.UnregisterReadonlyConnection(this.m_connection);
                }

                var lockObject = m_lockBox[this.m_connection.DatabasePath];
                Monitor.Exit(lockObject);

            }
        }

        /// <summary>
        /// Fakes a lock on the database
        /// </summary>
        /// <returns></returns>
        public override IDisposable Lock()
        {
            return new SQLiteConnectionWrapperLock(this);
        }
    }


}
