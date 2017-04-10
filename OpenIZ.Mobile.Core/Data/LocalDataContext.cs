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

        /// <summary>
        /// Try get cache item
        /// </summary>
        public IdentifiedData TryGetCacheItem(Guid key)
        {
            IdentifiedData retVal = null;
            this.m_cacheCommit.TryGetValue(key, out retVal);
            return retVal;
        }
    }
}
