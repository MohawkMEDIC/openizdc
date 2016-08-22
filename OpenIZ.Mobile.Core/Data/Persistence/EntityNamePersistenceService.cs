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
 * Date: 2016-6-14
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using OpenIZ.Mobile.Core.Data.Model.Concepts;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity name persistence service
    /// </summary>
    public class EntityNamePersistenceService : IdentifiedPersistenceService<EntityName, DbEntityName>
    {

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override EntityName Insert(SQLiteConnectionWithLock context, EntityName data)
        {

            // Ensure exists
            data.NameUse?.EnsureExists(context);
            data.NameUseKey = data.NameUse?.Key ?? data.NameUseKey;
            var retVal = base.Insert(context, data);

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<EntityNameComponent, EntityName>(
                    new List<EntityNameComponent>(),
                    data.Component, 
                    data.Key, 
                    context);

            return retVal;
        }

        /// <summary>
        /// Update the entity name
        /// </summary>
        public override EntityName Update(SQLiteConnectionWithLock context, EntityName data)
        {
            // Ensure exists
            data.NameUse?.EnsureExists(context);
            data.NameUseKey = data.NameUse?.Key ?? data.NameUseKey;

            var retVal = base.Update(context, data);

            var sourceKey = data.Key.Value.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<EntityNameComponent, EntityName>(
                    context.Table<DbEntityNameComponent>().Where(o => o.NameUuid == sourceKey).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityNameComponent, EntityNameComponent>(o)).ToList(),
                    data.Component,
                    data.Key,
                    context);

            return retVal;
        }

    }

}
