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
 * Date: 2016-6-28
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model;
using SQLite.Net;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity derived persistence services
    /// </summary>
    public class EntityDerivedPersistenceService<TModel, TData, TQueryResult> : IdentifiedPersistenceService<TModel, TData, TQueryResult>
        where TModel : Entity, new()
        where TData : DbIdentified, new()
        where TQueryResult : DbIdentified
    {

        // Entity persister
        protected EntityPersistenceService m_entityPersister = new EntityPersistenceService();

        /// <summary>
        /// Insert the specified TModel into the database
        /// </summary>
        protected override TModel InsertInternal(LocalDataContext context, TModel data)
        {
            var inserted = this.m_entityPersister.Insert(context, data);
            data.Key = inserted.Key;
            return base.InsertInternal(context, data);
        }

        /// <summary>
        /// Update the specified TModel
        /// </summary>
        protected override TModel UpdateInternal(LocalDataContext context, TModel data)
        {
            this.m_entityPersister.Update(context, data);
            return base.UpdateInternal(context, data);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        protected override TModel ObsoleteInternal(LocalDataContext context, TModel data)
        {
            var retVal = this.m_entityPersister.Obsolete(context, data);
            return data;
        }

    }
}