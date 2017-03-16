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
 * Date: 2016-6-14
 */
using OpenIZ.Core.Data.QueryBuilder.Attributes;
using OpenIZ.Mobile.Core.Data.Model.Concepts;
using SQLite.Net;
using SQLite.Net.Attributes;
using System;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
    /// <summary>
    /// Generic name component
    /// </summary>
    public abstract class DbGenericNameComponent : DbIdentified
    {

        /// <summary>
        /// Gets or sets the type of the component
        /// </summary>
        [Column("type"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
        public byte[] ComponentTypeUuid { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Column("value_id"), MaxLength(16), NotNull, Indexed]
        public virtual byte[] ValueUuid
        {
            get;
            set;
        }
    }
}