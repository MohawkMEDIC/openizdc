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
 * Date: 2016-10-11
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using System.Collections;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity relationship persistence service
    /// </summary>
    public class EntityRelationshipPersistenceService : IdentifiedPersistenceService<EntityRelationship, DbEntityRelationship>, ILocalAssociativePersistenceService
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
        protected override EntityRelationship InsertInternal(LocalDataContext context, EntityRelationship data)
        {
            
            // Ensure we haven't already persisted this
            if(data.TargetEntity != null) data.TargetEntity = data.TargetEntity.EnsureExists(context);
            data.TargetEntityKey = data.TargetEntity?.Key ?? data.TargetEntityKey;
            if(data.RelationshipType != null) data.RelationshipType = data.RelationshipType.EnsureExists(context);
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;

            byte[] target = data.TargetEntityKey.Value.ToByteArray(),
                source = data.SourceEntityKey.Value.ToByteArray(),
                typeKey = data.RelationshipTypeKey.Value.ToByteArray();

            var existing = context.Connection.Table<DbEntityRelationship>().Where(o => o.TargetUuid == target && o.SourceUuid == source && o.RelationshipTypeUuid == typeKey).FirstOrDefault();
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
        protected override EntityRelationship UpdateInternal(LocalDataContext context, EntityRelationship data)
        {
            // Ensure we haven't already persisted this
            if (data.TargetEntity != null) data.TargetEntity = data.TargetEntity.EnsureExists(context);
            data.TargetEntityKey = data.TargetEntity?.Key ?? data.TargetEntityKey;
            if (data.RelationshipType != null) data.RelationshipType = data.RelationshipType.EnsureExists(context);
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;

            return base.UpdateInternal(context, data);
        }
    }
}
