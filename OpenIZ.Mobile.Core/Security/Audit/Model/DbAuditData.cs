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
using OpenIZ.Core.Data.QueryBuilder.Attributes;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Security.Audit.Model
{
    /// <summary>
    /// Audit data
    /// </summary>
    [Table("audit")]
    [AssociativeTable(typeof(DbAuditActor), typeof(DbAuditActorAssociation))]
    public class DbAuditData
    {
        /// <summary>
        /// The identifier assigned to the audit number
        /// </summary>
        [Column("id"), PrimaryKey]
        public byte[] Id { get; set; }

        /// <summary>
        /// Outcome of the event
        /// </summary>
        [Column("outcome")]
        public int Outcome { get; set; }

        /// <summary>
        /// The action performed
        /// </summary>
        [Column("action")]
        public int ActionCode { get; set; }

        /// <summary>
        /// The type of action performed
        /// </summary>
        [Column("type")]
        public int EventIdentifier { get; set; }

        /// <summary>
        /// The time of the event
        /// </summary>
        [Column("eventTime")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The time the data was created
        /// </summary>
        [Column("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// The event type identifier
        /// </summary>
        [Column("class"), ForeignKey(typeof(DbAuditCode), nameof(DbAuditCode.Id)), AlwaysJoin]
        public byte[] EventTypeCode { get; set; }

        /// <summary>
        /// Query result type
        /// </summary>
        public class QueryResult : DbAuditData
        {

            /// <summary>
            /// Code
            /// </summary>
            [Column("code"), NotNull]
            public string Code { get; set; }

            /// <summary>
            /// Code system
            /// </summary>
            [Column("code_system")]
            public String CodeSystem { get; set; }

        }
    }
}
