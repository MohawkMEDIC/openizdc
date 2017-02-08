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

        // Data dictionary
        private Dictionary<String, Object> m_dataDictionary = new Dictionary<string, object>();

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
        /// Data dictionary
        /// </summary>
        public IDictionary<String, Object> Data { get { return this.m_dataDictionary; } }

        /// <summary>
        /// Add cache commit
        /// </summary>
        public void AddCacheCommit(IdentifiedData data)
        {
            if (data.Key.HasValue && !this.m_cacheCommit.ContainsKey(data.Key.Value) && data.Key.HasValue)
                this.m_cacheCommit.Add(data.Key.Value, data);
        }

        /// <summary>
        /// Adds data in a safe way
        /// </summary>
        public void AddData(string key, object value)
        {
            lock (this.m_dataDictionary)
                if (!this.m_dataDictionary.ContainsKey(key))
                    this.m_dataDictionary.Add(key, value);
        }


    }
}
