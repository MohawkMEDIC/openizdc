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
 * Date: 2017-3-31
 */
using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OpenIZ.Core.Data.QueryBuilder;
using OpenIZ.Mobile.Core.Data.Model;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persister which is a act relationship 
    /// </summary>
    public class ActRelationshipPersistenceService : IdentifiedPersistenceService<ActRelationship, DbActRelationship>, ILocalAssociativePersistenceService
    {
       
        /// <summary>
        /// Create DbActParticipation from modelinstance
        /// </summary>
        public override object FromModelInstance(ActRelationship modelInstance, LocalDataContext context)
        {
            modelInstance.Key = modelInstance.Key ?? Guid.NewGuid();
            return new DbActRelationship()
            {
                SourceUuid = modelInstance.SourceEntityKey?.ToByteArray(),
                TargetUuid = modelInstance.TargetActKey?.ToByteArray(),
                RelationshipTypeUuid = modelInstance.RelationshipTypeKey?.ToByteArray(),
                Uuid = modelInstance.Key?.ToByteArray()
            };
        }

        /// <summary>
        /// Get from source
        /// </summary>
        public IEnumerable GetFromSource(LocalDataContext context, Guid id, decimal? versionSequenceId)
        {
            return this.Query(context, o => o.SourceEntityKey == id);
        }

        /// <summary>
        /// Insert the relationship
        /// </summary>
        protected override ActRelationship InsertInternal(LocalDataContext context, ActRelationship data)
        {
            // Ensure we haven't already persisted this
            if(data.TargetAct != null) data.TargetAct = data.TargetAct.EnsureExists(context);
            data.TargetActKey = data.TargetAct?.Key ?? data.TargetActKey;
            if (data.RelationshipType != null) data.RelationshipType = data.RelationshipType.EnsureExists(context);
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;

            byte[] target = data.TargetActKey.Value.ToByteArray(),
                source = data.SourceEntityKey.Value.ToByteArray(),
                typeKey = data.RelationshipTypeKey.Value.ToByteArray();

            SqlStatement sql = new SqlStatement<DbActRelationship>().SelectFrom()
                .Where<DbActRelationship>(o => o.SourceUuid == source)
                .Limit(1).Build();

            IEnumerable<DbActRelationship> dbrelationships = context.TryGetData($"EX:{sql.ToString()}") as IEnumerable<DbActRelationship>;
            if (dbrelationships == null)
            {
                dbrelationships = context.Connection.Query<DbActRelationship>(sql.SQL, sql.Arguments.ToArray()).ToList();
                context.AddData($"EX{sql.ToString()}", dbrelationships);
            }
            var existing = dbrelationships.FirstOrDefault(
                    o => o.RelationshipTypeUuid == typeKey &&
                    o.TargetUuid == target);

            if (existing == null)
            {
                var retVal = base.InsertInternal(context, data);
                (dbrelationships as List<DbActRelationship>).Add(new DbActRelationship()
                {
                    Uuid = retVal.Key.Value.ToByteArray(),
                    RelationshipTypeUuid = typeKey,
                    SourceUuid = source,
                    TargetUuid = target
                });
                return retVal;
            }
            else
            {
                data.Key = new Guid(existing.Uuid);
                return data;
            }
        }

        /// <summary>
        /// Update the specified object
        /// </summary>
        protected override ActRelationship UpdateInternal(LocalDataContext context, ActRelationship data)
        {
            if (data.TargetAct != null) data.TargetAct = data.TargetAct.EnsureExists(context);
            data.TargetActKey = data.TargetAct?.Key ?? data.TargetActKey;
            if (data.RelationshipType != null) data.RelationshipType = data.RelationshipType.EnsureExists(context);
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;

            return base.UpdateInternal(context, data);
        }
    }
}
