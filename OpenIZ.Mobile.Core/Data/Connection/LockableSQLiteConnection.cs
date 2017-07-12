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
 * Date: 2017-6-28
 */
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
            base(sqlitePlatform, databasePath, openFlags, storeDateTimeAsTicks, serializer, tableMappings, extraTypeMappings, resolver, ApplicationContext.Current.GetCurrentContextSecurityKey())
        {

        }

        /// <summary>
        /// Locks the connection file
        /// </summary>
        public abstract IDisposable Lock();
    }
}
