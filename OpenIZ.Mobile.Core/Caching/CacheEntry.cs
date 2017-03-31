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
 * User: fyfej
 * Date: 2016-11-14
 */
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Caching
{
    /// <summary>
    /// Represents a cache entry
    /// </summary>
    public class CacheEntry
    {

        // Last read time
        private long m_lastReadTime;

        /// <summary>
        /// Creates a new cache entry
        /// </summary>
        public CacheEntry(DateTime loadTime, IdentifiedData data)
        {
            this.m_lastReadTime = this.LastUpdateTime = loadTime.Ticks;
            this.Data = data;
        }

        /// <summary>
        /// The time that the item was loaded
        /// </summary>
        public long LastUpdateTime { get; set; }

        /// <summary>
        /// Last read time
        /// </summary>
        public long LastReadTime { get { return this.m_lastReadTime; } set { this.m_lastReadTime = value; } }

        /// <summary>
        /// The data that was loaded
        /// </summary>
        public IdentifiedData Data { get; set; }

        /// <summary>
        /// Touches the cache entry
        /// </summary>
        internal void Touch()
        {
            Interlocked.Exchange(ref m_lastReadTime, DateTime.Now.Ticks);
        }

        /// <summary>
        /// Update the cache entry
        /// </summary>
        internal void Update(IdentifiedData data)
        {
            this.Data = data; // TODO: This should be a copy maybe?
            this.Touch();
        }

        public override string ToString()
        {
            return String.Format("CacheEntry {0} (@{1})", this.Data, this.Data.GetHashCode());
        }
    }
}
