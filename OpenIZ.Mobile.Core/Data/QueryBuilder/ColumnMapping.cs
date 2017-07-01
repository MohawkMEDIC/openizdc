﻿/*
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
 * Date: 2017-3-31
 */
using OpenIZ.Core.Data.QueryBuilder.Attributes;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenIZ.Core.Data.QueryBuilder
{
    /// <summary>
    /// Column mapping
    /// </summary>
    public class ColumnMapping
    {

        // Column mapping
        private static Dictionary<PropertyInfo, ColumnMapping> s_columnCache = new Dictionary<PropertyInfo, ColumnMapping>();

        /// <summary>
        /// Gets the source property
        /// </summary>
        public PropertyInfo SourceProperty { get; private set; }

        /// <summary>
        /// Gets the name of the column
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// True if column is primary key
        /// </summary>
        public bool IsPrimaryKey { get; private set; }


        /// <summary>
        /// Gets the foreign key 
        /// </summary>
        public ForeignKeyAttribute ForeignKey { get; private set; }

        /// <summary>
        /// The table mapping which this column belongs to
        /// </summary>
        public TableMapping Table { get; private set; }
        /// <summary>
        /// True if always join condition
        /// </summary>
        public bool IsAlwaysJoin { get; private set; }

        /// <summary>
        /// Create a column mapping
        /// </summary>
        private ColumnMapping(PropertyInfo pi, TableMapping table)
        {
            var att = pi.GetCustomAttribute<ColumnAttribute>();
            this.Name = att?.Name ?? pi.Name;
            this.IsPrimaryKey = pi.GetCustomAttribute<PrimaryKeyAttribute>() != null;
            this.SourceProperty = pi;
            this.ForeignKey = pi.GetCustomAttribute<ForeignKeyAttribute>();
            this.Table = table;
            this.IsAlwaysJoin = pi.GetCustomAttribute<AlwaysJoinAttribute>() != null;
        }

        /// <summary>
        /// Get property mapping
        /// </summary>
        public static ColumnMapping Get(PropertyInfo pi, TableMapping ownerTable)
        {

            ColumnMapping retVal = null;
            if(!s_columnCache.TryGetValue(pi, out retVal)) 
                lock(s_columnCache)
                {
                    retVal = new ColumnMapping(pi, ownerTable);
                    if (!s_columnCache.ContainsKey(pi))
                        s_columnCache.Add(pi, retVal);
                }
            return retVal;
        }
    }
}