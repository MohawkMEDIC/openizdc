/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-30
 */
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Synchronization.Model
{
    /// <summary>
    /// Represents a synchronization log entry
    /// </summary>
    [Table("tx_log")]
    public class SynchronizationLogEntry
    {
        /// <summary>
        /// Synchronization log
        /// </summary>
        public SynchronizationLogEntry()
        {
            this.Uuid = Guid.NewGuid().ToByteArray();
        }

        /// <summary>
        /// The unique identifier for the summary
        /// </summary>
        [Column("uuid"), MaxLength(16), PrimaryKey]
        public byte[] Uuid { get; set; }

        /// <summary>
        /// Gets or sets the resource type which was synchronized
        /// </summary>
        [Column("resource"), NotNull, Indexed]
        public String ResourceType { get; set; }

        /// <summary>
        /// The filter (if any)
        /// </summary>
        [Column("filter")]
        public String Filter { get; set; }

        /// <summary>
        /// Gets or sets the key of the resource last synchronized of this type
        /// </summary>
        [Column("etag")]
        public String LastETag { get; set; }

        /// <summary>
        /// Represents the last synchronization performed
        /// </summary>
        [Column("time")]
        public DateTime LastSync { get; set; }
    }
}
