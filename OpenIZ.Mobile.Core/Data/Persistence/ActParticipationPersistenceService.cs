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
 * Date: 2016-10-28
 */
using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Act participation persistence service
    /// </summary>
    public class ActParticipationPersistenceService : IdentifiedPersistenceService<ActParticipation, DbActParticipation>
    {

        /// <summary>
        /// Insert the relationship
        /// </summary>
        protected override ActParticipation InsertInternal(LocalDataContext context, ActParticipation data)
        {
            // Ensure we haven't already persisted this
            if(data.PlayerEntity != null) data.PlayerEntity = data.PlayerEntity.EnsureExists(context);
            data.PlayerEntityKey = data.PlayerEntity?.Key ?? data.PlayerEntityKey;
            if(data.ParticipationRole != null) data.ParticipationRole = data.ParticipationRole.EnsureExists(context);
            data.ParticipationRoleKey = data.ParticipationRole?.Key ?? data.ParticipationRoleKey;
            if (data.Act != null) data.Act = data.Act.EnsureExists(context);
            data.ActKey = data.Act?.Key ?? data.ActKey;

            byte[] target = data.PlayerEntityKey.Value.ToByteArray(),
                source = data.SourceEntityKey.Value.ToByteArray(),
                typeKey = data.ParticipationRoleKey.Value.ToByteArray();

            var existing = context.Connection.Table<DbActParticipation>().Where(o => o.EntityUuid == target && o.ActUuid == source && o.ParticipationRoleUuid == typeKey).FirstOrDefault();
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
        protected override ActParticipation UpdateInternal(LocalDataContext context, ActParticipation data)
        {
            if (data.PlayerEntity != null) data.PlayerEntity = data.PlayerEntity.EnsureExists(context);
            data.PlayerEntityKey = data.PlayerEntity?.Key ?? data.PlayerEntityKey;
            if (data.ParticipationRole != null) data.ParticipationRole = data.ParticipationRole.EnsureExists(context);
            data.ParticipationRoleKey = data.ParticipationRole?.Key ?? data.ParticipationRoleKey;
            if (data.Act != null) data.Act = data.Act.EnsureExists(context);
            data.ActKey = data.Act?.Key ?? data.ActKey;

            return base.UpdateInternal(context, data);
        }
    }
}
