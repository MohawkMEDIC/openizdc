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
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Synchronization.Model
{
    /// <summary>
    /// Synchroinzation query exec
    /// </summary>
    [Table("sync_queue")]
    public class SynchronizationQuery : SynchronizationLogEntry
    {


        /// <summary>
        /// Last successful record number
        /// </summary>
        [Column("last_recno")]
        public int LastSuccess { get; set; }

        /// <summary>
        /// Start time of the query
        /// </summary>
        [Column("start")]
        public DateTime StartTime { get; set; }

    }
}
