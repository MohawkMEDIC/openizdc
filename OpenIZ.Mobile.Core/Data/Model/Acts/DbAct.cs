﻿/*
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
 * Date: 2016-7-2
 */
using SQLite;
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
    public class DbAct : DbVersionedData
    {
        [Column("isNegated")]
        public bool IsNegated { get; set; }

        /// <summary>
        /// Identifies the time that the act occurred
        /// </summary>
        [Column("actTime")]
        public DateTime? ActTime { get; set; }

        /// <summary>
        /// Identifies the start time of the act
        /// </summary>
        [Column("startTime")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Identifies the stop time of the act
        /// </summary>
        [Column("stopTime")]
        public DateTime? StopTime { get; set; }

        /// <summary>
        /// Identifies the class concept
        /// </summary>
        [Column("classConcept"), MaxLength(16), NotNull]
        public byte[] ClassConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the mood of the act
        /// </summary>
        [Column("moodConcept"), MaxLength(16), NotNull]
        public byte[] MoodConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the reason concept
        /// </summary>
        [Column("reasonConcept"), MaxLength(16)]
        public byte[] ReasonConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the status concept
        /// </summary>
        [Column("statusConcept"), MaxLength(16), NotNull]
        public byte[] StatusConceptUuid { get; set; }

        /// <summary>
        /// Gets or sets the type concept
        /// </summary>
        [Column("typeConcept"), MaxLength(16)]
        public byte[] TypeConceptUuid { get; set; }

    }
}
