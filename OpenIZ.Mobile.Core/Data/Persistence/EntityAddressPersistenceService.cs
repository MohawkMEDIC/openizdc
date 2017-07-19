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
 * Date: 2017-2-4
 */
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using OpenIZ.Mobile.Core.Exceptions;
using SQLite.Net;
using SQLite.Net.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persistence service for entity addresses
    /// </summary>
    public class EntityAddressPersistenceService : IdentifiedPersistenceService<EntityAddress, DbEntityAddress>, ILocalAssociativePersistenceService
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
        public override object FromModelInstance(EntityAddress modelInstance, LocalDataContext context)
        {
            foreach (var itm in modelInstance.Component)
                itm.Value = itm.Value.Trim();

            modelInstance.Key = modelInstance.Key ?? Guid.NewGuid();

            return new DbEntityAddress()
            {
                Uuid = modelInstance.Key?.ToByteArray(),
                SourceUuid = modelInstance.SourceEntityKey?.ToByteArray(),
                UseConceptUuid = modelInstance.AddressUseKey?.ToByteArray()
            };
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        protected override EntityAddress InsertInternal(LocalDataContext context, EntityAddress data)
        {

            // Ensure exists
            if (data.AddressUse != null) data.AddressUse = data.AddressUse?.EnsureExists(context);
            data.AddressUseKey = data.AddressUse?.Key ?? data.AddressUseKey;

            var retVal = base.InsertInternal(context, data);

            // Data component
            var addPx = ApplicationContext.Current.GetService<EntityAddressComponentPersistenceService>();

            if (data.Component != null)
                base.UpdateAssociatedItems<EntityAddressComponent, EntityAddress>(
                    new List<EntityAddressComponent>(),
                    data.Component,
                    data.Key,
                    context);

            return retVal;
        }

        /// <summary>
        /// Update the entity name
        /// </summary>
        protected override EntityAddress UpdateInternal(LocalDataContext context, EntityAddress data)
        {

            // Ensure exists
            if (data.AddressUse != null) data.AddressUse = data.AddressUse?.EnsureExists(context);
            data.AddressUseKey = data.AddressUse?.Key ?? data.AddressUseKey;

            var retVal = base.UpdateInternal(context, data);

            var sourceKey = data.Key.Value.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<EntityAddressComponent, EntityAddress>(
                    context.Connection.Table<DbEntityAddressComponent>().Where(o => o.AddressUuid == sourceKey).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityAddressComponent, EntityAddressComponent>(o)).ToList(),
                    data.Component,
                    data.Key,
                    context);

            return retVal;
        }

    }

    /// <summary>
    /// Entity address component persistence service
    /// </summary>
    public class EntityAddressComponentPersistenceService : IdentifiedPersistenceService<EntityAddressComponent, DbEntityAddressComponent, DbEntityAddressComponent.QueryResult>, ILocalAssociativePersistenceService
    {

        // Address value identifier lookup 
        private Dictionary<String, byte[]> m_addressValueIds = new Dictionary<string, byte[]>();

        /// <summary>
        /// To model instance
        /// </summary>
        public override EntityAddressComponent ToModelInstance(object dataInstance, LocalDataContext context)
        {
            if (dataInstance == null) return null;

            var addrComp = (dataInstance as DbEntityAddressComponent.QueryResult)?.GetInstanceOf<DbEntityAddressComponent>() ?? dataInstance as DbEntityAddressComponent;
            var addrValue = (dataInstance as DbEntityAddressComponent.QueryResult)?.GetInstanceOf<DbAddressValue>() ?? context.Connection.Table<DbAddressValue>().Where(o => o.Uuid == addrComp.ValueUuid).FirstOrDefault();
            return new EntityAddressComponent()
            {
                ComponentTypeKey = new Guid(addrComp.ComponentTypeUuid),
                Value = addrValue.Value,
                Key = addrComp.Key,
                SourceEntityKey = new Guid(addrComp.AddressUuid)
            };
        }

        /// <summary>
        /// From the model instance
        /// </summary>
        public override object FromModelInstance(EntityAddressComponent modelInstance, LocalDataContext context)
        {
            modelInstance.Key = modelInstance.Key ?? Guid.NewGuid();
            var retVal = new DbEntityAddressComponent()
            {
                AddressUuid = modelInstance.SourceEntityKey?.ToByteArray(),
                ComponentTypeUuid = modelInstance.ComponentTypeKey?.ToByteArray(),
                Uuid = modelInstance.Key?.ToByteArray() 
            };

            // Address component already exists?
            byte[] existingKey = null;
            if (String.IsNullOrEmpty(modelInstance.Value)) return retVal;
            if (!this.m_addressValueIds.TryGetValue(modelInstance.Value, out existingKey))
            {
                var existing = context.Connection.Table<DbAddressValue>().Where(o => o.Value == modelInstance.Value).Take(1).FirstOrDefault();
                if (existing != null && existing.Key != retVal.Key)
                    retVal.ValueUuid = existing.Uuid;
                else if (!String.IsNullOrEmpty(modelInstance.Value))
                {
                    retVal.ValueUuid = Guid.NewGuid().ToByteArray();
                    context.Connection.Insert(new DbAddressValue()
                    {
                        Uuid = retVal.ValueUuid,
                        Value = modelInstance.Value
                    });
                }
                lock (this.m_addressValueIds)
                    if (!this.m_addressValueIds.ContainsKey(modelInstance.Value))
                        this.m_addressValueIds.Add(modelInstance.Value, retVal.ValueUuid);
            }
            else
                retVal.ValueUuid = existingKey;

            return retVal;
        }

        /// <summary>
        /// Entity address component
        /// </summary>
        protected override EntityAddressComponent InsertInternal(LocalDataContext context, EntityAddressComponent data)
        {
            if (data.ComponentType != null) data.ComponentType = data.ComponentType?.EnsureExists(context) as Concept;
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.InsertInternal(context, data);
        }

        /// <summary>
        /// Update 
        /// </summary>
        protected override EntityAddressComponent UpdateInternal(LocalDataContext context, EntityAddressComponent data)
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
