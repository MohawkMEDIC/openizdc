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
using OpenIZ.Mobile.Core.Data.Model.Entities;
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
    /// Represents a link between an act and an entity
    /// </summary>
    [Table("act_participation")]
    public class DbActParticipation : DbIdentified
    {
        /// <summary>
        /// Gets or sets the act identifier
        /// </summary>
        [Column("act_uuid"), MaxLength(16), Indexed, NotNull, ForeignKey(typeof(DbAct), nameof(DbAct.Uuid))]
        public byte[] ActUuid { get; set; }

        /// <summary>
        /// Gets or sets the act identifier
        /// </summary>
        [Column("entity_uuid"), MaxLength(16), Indexed, NotNull, ForeignKey(typeof(DbEntity), nameof(DbEntity.Uuid))]
        public byte[] EntityUuid { get; set; }

        /// <summary>
        /// Gets or sets the role that the player plays in the act
        /// </summary>
        [Column("participationRole"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
        public byte[] ParticipationRoleUuid { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        [Column("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets the sequence identifier
        /// </summary>
        [Column("sequence")]
        public int Sequence { get; set; }

    }
}
