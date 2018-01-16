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
using OpenIZ.Mobile.Core.Data.Model.Concepts;
using OpenIZ.Mobile.Core.Data.Model.Extensibility;
using OpenIZ.Mobile.Core.Data.Model.Security;
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Acts
{
    /// <summary>
    /// Represents a table which can store act data
    /// </summary>
    [Table("act")]
    [AssociativeTable(typeof(DbSecurityPolicy), typeof(DbActSecurityPolicy))]
    public class DbAct : DbVersionedData
    {
        /// <summary>
        /// Gets or sets the template
        /// </summary>
        [Column("template"), MaxLength(16), ForeignKey(typeof(DbTemplateDefinition), nameof(DbTemplateDefinition.Uuid))]
        public byte[] TemplateUuid { get; set; }

        /// <summary>
        /// True if negated
        /// </summary>
        [Column("isNegated")]
        public bool IsNegated { get; set; }

        /// <summary>
        /// Identifies the time that the act occurred
        /// </summary>
        [Column("actTime")]
        public DateTimeOffset? ActTime { get; set; }

        /// <summary>
        /// Identifies the start time of the act
        /// </summary>
        [Column("startTime")]
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// Identifies the stop time of the act
        /// </summary>
        [Column("stopTime")]
        public DateTimeOffset? StopTime { get; set; }

        /// <summary>
        /// Identifies the class concept
        /// </summary>
        [Column("classConcept"), MaxLength(16), NotNull, ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
        public byte[] ClassConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the mood of the act
        /// </summary>
        [Column("moodConcept"), MaxLength(16), NotNull, ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
        public byte[] MoodConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the reason concept
        /// </summary>
        [Column("reasonConcept"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
        public byte[] ReasonConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the status concept
        /// </summary>
        [Column("statusConcept"), MaxLength(16), NotNull, ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
        public byte[] StatusConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the type concept
        /// </summary>
        [Column("typeConcept"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
        public byte[] TypeConceptUuid { get; set; }

    }
}
