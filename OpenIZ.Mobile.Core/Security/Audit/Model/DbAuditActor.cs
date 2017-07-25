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
    /// Audit actors
    /// </summary>
    [Table("audit_actor")]
    [AssociativeTable(typeof(DbAuditData), typeof(DbAuditActorAssociation))]

    public class DbAuditActor
    {
        /// <summary>
        /// Identifier for the actor instance
        /// </summary>
        [Column("id"), PrimaryKey]
        public byte[] Id { get; set; }

        /// <summary>
        /// User identifier
        /// </summary>
        [Column("user_id"), MaxLength(16)]
        public String UserIdentifier { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [Column("user_name"), Unique, Collation("NOCASE")]
        public String UserName { get; set; }

        /// <summary>
        /// True if user is requestor
        /// </summary>
        [Column("is_requestor")]
        public bool UserIsRequestor { get; set; }

        /// <summary>
        /// Role code identifier
        /// </summary>
        [Column("role_code_id")]
        public byte[] ActorRoleCode { get; set; }

        /// <summary>
        /// Query result
        /// </summary>
        public class QueryResult : DbAuditActor
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
