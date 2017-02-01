using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Interfaces;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data
{
    /// <summary>
    /// Local data context
    /// </summary>
    public class LocalDataContext
    {
        /// <summary>
        /// Cache commit
        /// </summary>
        private Dictionary<Guid, IdentifiedData> m_cacheCommit = new Dictionary<Guid, IdentifiedData>();

        /// <summary>
        /// Local data context
        /// </summary>
        public LocalDataContext(SQLiteConnectionWithLock connection)
        {
            this.Connection = connection;
            this.m_cacheCommit = new Dictionary<Guid, IdentifiedData>();
        }

        /// <summary>
        /// Local data connection
        /// </summary>
        public SQLiteConnectionWithLock Connection { get; set; }

        /// <summary>
        /// Cache on commit
        /// </summary>
        public IEnumerable<IdentifiedData> CacheOnCommit
        {
            get
            {
                return this.m_cacheCommit.Values;
            }
        }

        /// <summary>
        /// Add cache commit
        /// </summary>
        public void AddCacheCommit(IdentifiedData data)
        {
            if (data.Key.HasValue && !this.m_cacheCommit.ContainsKey(data.Key.Value) && data.Key.HasValue)
                this.m_cacheCommit.Add(data.Key.Value, data);
        }

        
    }
}
