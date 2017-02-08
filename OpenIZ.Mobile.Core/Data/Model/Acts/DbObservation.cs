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
 * Date: 2016-7-24
 */
using OpenIZ.Core.Data.QueryBuilder.Attributes;
using OpenIZ.Mobile.Core.Data.Model.Concepts;
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
    /// Stores data related to an observation act
    /// </summary>
    [Table("observation")]
    public class DbObservation : DbActSubTable
    {

        /// <summary>
        /// Gets or sets the interpretation concept
        /// </summary>
        [Column("interpretationConcept"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
        public byte[] InterpretationConceptUuid { get; set; }

        /// <summary>
        /// Identifies the value type
        /// </summary>
        [Column("valueType"), MaxLength(2), NotNull]
        public String ValueType { get; set; }

        /// <summary>
        /// A query result of type observation
        /// </summary>
        public class QueryResult : DbAct
        {

            [Column("interpretationConcept"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
            public byte[] InterpretationConceptUuid { get; set; }

            [Column("valueType"), MaxLength(2), NotNull]
            public String ValueType { get; set; }

        }

    }

    /// <summary>
    /// Represents additional data related to a quantified observation
    /// </summary>
    [Table("quantity_observation")]
    public class DbQuantityObservation : DbObservationSubTable
    {

        /// <summary>
        /// Represents the unit of measure
        /// </summary>
        [Column("unitOfMeasure"), MaxLength(16), NotNull, ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
        public byte[] UnitOfMeasureUuid { get; set; }

        /// <summary>
        /// Gets or sets the value of the measure
        /// </summary>
        [Column("value")]
        public Decimal Value { get; set; }

        /// <summary>
        /// DbObservation query result
        /// </summary>
        public class QueryResult : DbObservation.QueryResult
        {

            /// <summary>
            /// Represents the unit of measure
            /// </summary>
            [Column("unitOfMeasure"), MaxLength(16), NotNull, ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
            public byte[] UnitOfMeasureUuid { get; set; }

            /// <summary>
            /// Gets or sets the value of the measure
            /// </summary>
            [Column("value")]
            public Decimal Value { get; set; }

        }
    }

    /// <summary>
    /// Identifies the observation as a text obseration
    /// </summary>
    [Table("text_observation")]
    public class DbTextObservation : DbObservationSubTable
    {
        /// <summary>
        /// Gets the value of the observation as a string
        /// </summary>
        [Column("value")]
        public String Value { get; set; }

        public class QueryResult : DbObservation.QueryResult
        {
            /// <summary>
            /// Gets the value of the observation as a string
            /// </summary>
            [Column("value")]
            public String Value { get; set; }

        }
    }

    /// <summary>
    /// Identifies data related to a coded observation
    /// </summary>
    [Table("coded_observation")]
    public class DbCodedObservation : DbObservationSubTable
    {

        /// <summary>
        /// Gets or sets the concept representing the value of this
        /// </summary>
        [Column("value"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))] 
        public byte[] Value { get; set; }

        /// <summary>
        /// Observation query result
        /// </summary>
        public class QueryResult : DbObservation.QueryResult
        {

            /// <summary>
            /// Gets or sets the concept representing the value of this
            /// </summary>
            [Column("value"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
            public byte[] Value { get; set; }

        }
    }
}
