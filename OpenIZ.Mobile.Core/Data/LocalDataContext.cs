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
 * Date: 2017-3-31
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Mobile.Core.Data.Connection;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Interop;
using OpenIZ.Core.Data.QueryBuilder;

namespace OpenIZ.Mobile.Core.Data
{
    /// <summary>
    /// Local data context
    /// </summary>
    public class LocalDataContext : IDisposable
    {

        /// <summary>
        /// Partial load mode
        /// </summary>
        public LocalDataContext()
        {
            this.DelayLoadMode = LoadState.PartialLoad;
        }

        // Prepared
        private Dictionary<String, IDbStatement> m_prepared = new Dictionary<string, IDbStatement>();

        // Cached query
        private Dictionary<String, IEnumerable<Object>> m_cachedQuery = new Dictionary<string, IEnumerable<object>>();
        
        /// <summary>
        /// Cache commit
        /// </summary>
        private Dictionary<Guid, IdentifiedData> m_cacheCommit = new Dictionary<Guid, IdentifiedData>();

        // Data dictionary
        private Dictionary<String, Object> m_dataDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Associations to be be forcably loaded
        /// </summary>
        public String[] LoadAssociations { get; set; }

        /// <summary>
        /// Local data context
        /// </summary>
        public LocalDataContext(SQLiteConnection connection)
        {
            this.Connection = connection;
            this.m_cacheCommit = new Dictionary<Guid, IdentifiedData>();
        }

        /// <summary>
        /// Lock connection
        /// </summary>
        public IDisposable LockConnection()
        {
            return (this.Connection as SQLiteConnectionWithLock)?.Lock()
                ??
                (this.Connection as SQLiteConnectionManager.SQLiteConnectionWrapper)?.Lock();
        }

        /// <summary>
        /// Local data connection
        /// </summary>
        public SQLiteConnection Connection { get; set; }

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
        /// The data loading mode
        /// </summary>
        public OpenIZ.Core.Model.LoadState DelayLoadMode { get; set; }

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

        /// <summary>
        /// Try get cache item
        /// </summary>
        public IdentifiedData TryGetCacheItem(Guid key)
        {
            IdentifiedData retVal = null;
            this.m_cacheCommit.TryGetValue(key, out retVal);
            return retVal;
        }

        /// <summary>
        /// Get or create prepared statement
        /// </summary>
        internal IDbStatement GetOrCreatePrepared(string cmdText)
        {
            IDbStatement prepared = null;
            if(!this.m_prepared.TryGetValue(cmdText, out prepared))
            {
                prepared = this.Connection.Prepare(cmdText);
                lock (this.m_prepared)
                    if (!this.m_prepared.ContainsKey(cmdText))
                        this.m_prepared.Add(cmdText, prepared);
            }
            return prepared;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            foreach (var stmt in this.m_prepared.Values)
                stmt.Finalize();
        }

        /// <summary>
        /// Query
        /// </summary>
        public String GetQueryLiteral(SqlStatement query)
        {
            return query.ToString();
        }

      
        /// <summary>
        /// Add a cached set of query results
        /// </summary>
        public void AddQuery(SqlStatement domainQuery, IEnumerable<object> results)
        {
            var key = this.GetQueryLiteral(domainQuery);
            lock (this.m_cachedQuery)
                if (!this.m_cachedQuery.ContainsKey(key))
                    this.m_cachedQuery.Add(key, results);
        }

        /// <summary>
        /// Cache a query 
        /// </summary>
        public IEnumerable<Object> CacheQuery(SqlStatement domainQuery)
        {
            var key = this.GetQueryLiteral(domainQuery);
            IEnumerable<Object> retVal = null;
            this.m_cachedQuery.TryGetValue(key, out retVal);
            return retVal;
        }

        /// <summary>
        /// Try to get data from data array
        /// </summary>
        public Object TryGetData(String key)
        {
            Object data = null;
            this.Data.TryGetValue(key, out data);
            return data;
        }
    }
}
