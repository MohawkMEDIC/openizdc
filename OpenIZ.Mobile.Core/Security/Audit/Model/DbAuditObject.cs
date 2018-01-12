/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
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
    /// Represents a target object
    /// </summary>
    [Table("audit_object")]
    public class DbAuditObject
    {
        /// <summary>
        /// Identifier of the object
        /// </summary>
        [Column("id"), PrimaryKey]
        public byte[] Id { get; set; }

        /// <summary>
        /// Gets or sets the audit identifier
        /// </summary>
        [Column("audit_id"), ForeignKey(typeof(DbAuditData), nameof(DbAuditData.Id))]
        public byte[] AuditId { get; set; }

        /// <summary>
        /// The identifier of the object
        /// </summary>
        [Column("obj_id")]
        public string ObjectId { get; set; }

        /// <summary>
        /// The object type identifier
        /// </summary>
        [Column("obj_typ")]
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the role
        /// </summary>
        [Column("rol")]
        public int Role { get; set; }

        /// <summary>
        /// The lifecycle
        /// </summary>
        [Column("lifecycle")]
        public int LifecycleType { get; set; }

        /// <summary>
        /// Identifier type code
        /// </summary>
        [Column("id_type")]
        public int IDTypeCode { get; set; }

        /// <summary>
        /// The query associated
        /// </summary>
        [Column("query")]
        public String QueryData { get; set; }

        /// <summary>
        /// The name data associated 
        /// </summary>
        [Column("name")]
        public String NameData { get; set; }
    }
}
