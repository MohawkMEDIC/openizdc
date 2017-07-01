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
    /// Locked SQLite connection
    /// </summary>
    public class LockedSQLiteConnection : SQLiteConnection
    {

        /// <summary>
        /// Creates a locked SQLite connection
        /// </summary>
        public LockedSQLiteConnection(
            ISQLitePlatform sqlitePlatform, string databasePath, bool storeDateTimeAsTicks = true, IBlobSerializer serializer = null, IDictionary<string, TableMapping> tableMappings = null, IDictionary<Type, string> extraTypeMappings = null, IContractResolver resolver = null
            ) : base(sqlitePlatform, databasePath, storeDateTimeAsTicks , serializer , tableMappings,extraTypeMappings,resolver)
        {

        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            SQLiteConnectionManager.Current.ReleaseConnection(this.DatabasePath);
            base.Dispose(disposing);
        }
    }
}
