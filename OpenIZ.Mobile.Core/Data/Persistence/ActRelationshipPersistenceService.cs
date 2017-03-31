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

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persister which is a act relationship 
    /// </summary>
    public class ActRelationshipPersistenceService : IdentifiedPersistenceService<ActRelationship, DbActRelationship>, ILocalAssociativePersistenceService
    {

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

            var existing = context.Connection.Table<DbActRelationship>().Where(o => o.TargetUuid == target && o.SourceUuid == source && o.RelationshipTypeUuid == typeKey).FirstOrDefault();
            if (existing == null)
                return base.InsertInternal(context, data);
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
