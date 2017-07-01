using SQLite.Net;
using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Connection
{
    /// <summary>
    /// Lockable sqlite connection
    /// </summary>
    public abstract class LockableSQLiteConnection : SQLiteConnection
    {

        // Lock count
        protected int m_lockCount = 0;

        /// <summary>
        /// When true the connection stays open
        /// </summary>
        public bool Persistent { get; set; }

        /// <summary>
        /// Constructor for locable sqlite connection
        /// </summary>
        public LockableSQLiteConnection(ISQLitePlatform sqlitePlatform, String databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true, IBlobSerializer serializer = null, IDictionary<String, TableMapping> tableMappings = null, IDictionary<Type, String> extraTypeMappings = null, IContractResolver resolver = null) :
            base(sqlitePlatform, databasePath, openFlags, storeDateTimeAsTicks, serializer, tableMappings, extraTypeMappings, resolver)
        {

        }

        /// <summary>
        /// Locks the connection file
        /// </summary>
        public abstract IDisposable Lock();
    }
}
