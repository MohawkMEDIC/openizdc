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
using System.Collections;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Services;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity name persistence service
    /// </summary>
    public class EntityNamePersistenceService : IdentifiedPersistenceService<EntityName, DbEntityName>, ILocalAssociativePersistenceService
    {

        /// <summary>
        /// Get from source
        /// </summary>
        public IEnumerable GetFromSource(LocalDataContext context, Guid id, decimal? versionSequenceId)
        {
            return this.Query(context, o => o.SourceEntityKey == id);
        }

        /// <summary>
        /// Override model instance
        /// </summary>
        public override object FromModelInstance(EntityName modelInstance, LocalDataContext context)
        {
            foreach (var itm in modelInstance.Component)
                itm.Value = itm.Value.Trim();
            return base.FromModelInstance(modelInstance, context);
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        protected override EntityName InsertInternal(LocalDataContext context, EntityName data)
        {

            // Ensure exists
            if(data.NameUse != null) data.NameUse = data.NameUse?.EnsureExists(context);
            data.NameUseKey = data.NameUse?.Key ?? data.NameUseKey;
            var retVal = base.InsertInternal(context, data);

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
        protected override EntityName UpdateInternal(LocalDataContext context, EntityName data)
        {
            // Ensure exists
            if (data.NameUse != null) data.NameUse = data.NameUse?.EnsureExists(context);
            data.NameUseKey = data.NameUse?.Key ?? data.NameUseKey;

            var retVal = base.UpdateInternal(context, data);

            var sourceKey = data.Key.Value.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<EntityNameComponent, EntityName>(
                    context.Connection.Table<DbEntityNameComponent>().Where(o => o.NameUuid == sourceKey).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityNameComponent, EntityNameComponent>(o)).ToList(),
                    data.Component,
                    data.Key,
                    context);

            return retVal;
        }

    }

    /// <summary>
    /// Entity address component persistence service
    /// </summary>
    public class EntityNameComponentPersistenceService : IdentifiedPersistenceService<EntityNameComponent, DbEntityNameComponent, DbEntityNameComponent.QueryResult>, ILocalAssociativePersistenceService
    {

        /// <summary>
        /// To model instance
        /// </summary>
        public override EntityNameComponent ToModelInstance(object dataInstance, LocalDataContext context, bool loadFast)
        {
            if (dataInstance == null) return null;

            var entName = (dataInstance as DbEntityNameComponent.QueryResult)?.GetInstanceOf<DbEntityNameComponent>() ?? dataInstance as DbEntityNameComponent;
            var nameValue = (dataInstance as DbEntityNameComponent.QueryResult)?.GetInstanceOf<DbPhoneticValue>() ?? context.Connection.Table<DbPhoneticValue>().Where(o => o.Uuid == entName.ValueUuid).FirstOrDefault();
            return new EntityNameComponent()
            {
                ComponentTypeKey = new Guid(entName.ComponentTypeUuid ?? Guid.Empty.ToByteArray() ),
                Value = nameValue.Value,
                Key = entName.Key,
                PhoneticAlgorithmKey = new Guid(nameValue.PhoneticAlgorithmUuid ?? Guid.Empty.ToByteArray() ),
                PhoneticCode = nameValue.PhoneticCode,
                SourceEntityKey = new Guid(entName.NameUuid)
            };
        }

        /// <summary>
        /// From the model instance
        /// </summary>
        public override object FromModelInstance(EntityNameComponent modelInstance, LocalDataContext context)
        {
            var retVal = base.FromModelInstance(modelInstance, context) as DbEntityNameComponent;

            // Address component already exists?
            var existing = context.Connection.Table<DbPhoneticValue>().Where(o => o.Value == modelInstance.Value).FirstOrDefault();
            if (existing != null && existing.Key != retVal.Key)
                retVal.ValueUuid = existing.Uuid;
            else
            {
                var phoneticCoder = ApplicationContext.Current.GetService<IPhoneticAlgorithmHandler>();
                retVal.ValueUuid = Guid.NewGuid().ToByteArray();
                context.Connection.Insert(new DbPhoneticValue()
                {
                    Uuid = retVal.ValueUuid,
                    Value = modelInstance.Value,
                    PhoneticAlgorithmUuid = (phoneticCoder?.AlgorithmId ?? PhoneticAlgorithmKeys.None).ToByteArray(),
                    PhoneticCode = phoneticCoder?.GenerateCode(modelInstance.Value)
                });
            }
            return retVal;
        }

        /// <summary>
        /// Entity address component
        /// </summary>
        protected override EntityNameComponent InsertInternal(LocalDataContext context, EntityNameComponent data)
        {
            if (data.ComponentType != null) data.ComponentType = data.ComponentType?.EnsureExists(context) as Concept;
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.InsertInternal(context, data);
        }

        /// <summary>
        /// Update 
        /// </summary>
        protected override EntityNameComponent UpdateInternal(LocalDataContext context, EntityNameComponent data)
        {
            if (data.ComponentType != null) data.ComponentType = data.ComponentType?.EnsureExists(context) as Concept;

            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.UpdateInternal(context, data);
        }

        /// <summary>
        /// Get components from source
        /// </summary>
        public IEnumerable GetFromSource(LocalDataContext context, Guid id, decimal? versionSequenceId)
        {
            int tr = 0;
            return this.QueryInternal(context, o => o.SourceEntityKey == id, 0, -1, out tr, Guid.Empty, false);
        }

    }


}
