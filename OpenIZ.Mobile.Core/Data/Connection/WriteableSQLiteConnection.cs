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
    public class WriteableSQLiteConnection : LockableSQLiteConnection
    {

        public WriteableSQLiteConnection(ISQLitePlatform sqlitePlatform, String databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true, IBlobSerializer serializer = null, IDictionary<String, TableMapping> tableMappings = null, IDictionary<Type, String> extraTypeMappings = null, IContractResolver resolver = null) :
            base(sqlitePlatform, databasePath, openFlags, storeDateTimeAsTicks, serializer, tableMappings, extraTypeMappings, resolver)
        {
        }

        /// <summary>
        /// Wrapper lock
        /// </summary>
        private class SQLiteConnectionWrapperLock : IDisposable
        {
            // Connection
            private WriteableSQLiteConnection m_connection;

            // Lockbox
            private static Dictionary<String, Object> m_lockBox = new Dictionary<string, object>();

            /// <summary>
            /// Wrapper with lock
            /// </summary>
            public SQLiteConnectionWrapperLock(WriteableSQLiteConnection wrappedConnection)
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
                SQLiteConnectionManager.Current.RegisterWriteConnection(this.m_connection);
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
                    SQLiteConnectionManager.Current.UnregisterWriteConnection(this.m_connection);
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
