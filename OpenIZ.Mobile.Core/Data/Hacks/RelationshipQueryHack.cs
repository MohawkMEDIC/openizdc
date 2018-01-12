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
 * Date: 2018-1-12
 */
using OpenIZ.Core.Data.QueryBuilder;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Hacks
{
    /// <summary>
    /// Represents a query hack for participations / relationships where the guard is being queried 
    /// </summary>
    public class RelationshipQueryHack : IQueryBuilderHack
    {

        /// <summary>
        /// Hack query builder based on clause
        /// </summary>
        public bool HackQuery(QueryBuilder builder, SqlStatement sqlStatement, SqlStatement whereClause, PropertyInfo property, string queryPrefix, QueryPredicate predicate, object values, IEnumerable<TableMapping> scopedTables)
        {
            string columnName = String.Empty;
            Type scanType = null;

            // Filter values
            if (typeof(Concept).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()) && predicate.SubPath == "mnemonic")
            {
                if (predicate.Path == "participationRole" && property.DeclaringType == typeof(ActParticipation))
                {
                    columnName = "participationRole";
                    scanType = typeof(ActParticipationKey);
                }
                else if (predicate.Path == "relationshipType" && property.DeclaringType == typeof(EntityRelationship))
                {
                    columnName = "relationshipType";
                    scanType = typeof(EntityRelationshipTypeKeys);
                }
                else if (predicate.Path == "relationshipType" && property.DeclaringType == typeof(ActRelationship))
                {
                    columnName = "relationshipType";
                    scanType = typeof(ActRelationshipTypeKeys);
                }
                else
                    return false;

                // Now we scan
                List<Object> qValues = new List<object>();
                if (values is IEnumerable)
                    foreach (var i in values as IEnumerable)
                    {
                        var fieldInfo = scanType.GetRuntimeField(i.ToString());
                        if (fieldInfo == null) return false;
                        qValues.Add(fieldInfo.GetValue(null));
                    }
                else
                {
                    var fieldInfo = scanType.GetRuntimeField(values.ToString());
                    if (fieldInfo == null) return false;
                    qValues.Add(fieldInfo.GetValue(null));
                }

                // Now add to query
                whereClause.And($"{columnName} IN ({String.Join(",", qValues.Select(o => $"X'{BitConverter.ToString(((Guid)o).ToByteArray()).Replace("-","")}'").ToArray())})");
                return true;
            }
            else
                return false;
        }
    }
}
