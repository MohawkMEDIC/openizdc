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
    /// Associates the audit actor to audit message
    /// </summary>
    [Table("audit_actor_assoc")]
    public class DbAuditActorAssociation
    {
        /// <summary>
        /// Id of the association
        /// </summary>
        [Column("id"), PrimaryKey]
        public byte[] Id { get; set; }

        /// <summary>
        /// Audit identifier
        /// </summary>
        [Column("audit_id"), NotNull, Indexed, ForeignKey(typeof(DbAuditData), nameof(DbAuditData.Id))]
        public byte[] SourceUuid { get; set; }

        /// <summary>
        /// Actor identifier
        /// </summary>
        [Column("actor_id"), NotNull, Indexed, ForeignKey(typeof(DbAuditActor), nameof(DbAuditActor.Id))]
        public byte[] TargetUuid { get; set; }

    }
}
