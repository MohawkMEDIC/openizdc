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
using OpenIZ.Core.Interfaces;
using SQLite.Net.Interop;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Data.Connection;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity name persistence service
    /// </summary>
    public class EntityNamePersistenceService : IdentifiedPersistenceService<EntityName, DbEntityName>, ILocalAssociativePersistenceService
    {

        private readonly Dictionary<Guid, String> m_nameUseMap = new Dictionary<Guid, String>() {
            { Guid.Parse("71D1C07C-6EE6-4240-8A95-19F96583512E"), "Alphabetic" },
            { Guid.Parse("95E6843A-26FF-4046-B6F4-EB440D4B85F7"), "Anonymous" },
            { Guid.Parse("4A7BF199-F33B-42F9-8B99-32433EA67BD7"), "Artist" },
            { Guid.Parse("A87A6D21-2CA6-4AEA-88F3-6135CCEB58D1"), "Assigned" },
            { Guid.Parse("09000479-4672-44F8-BB4A-72FB25F7356A"), "Ideographic" },
            { Guid.Parse("A3FB2A05-5EBE-47AE-AFD0-4C1B22336090"), "Indigenous" },
            { Guid.Parse("EFFE122D-8D30-491D-805D-ADDCB4466C35"), "Legal" },
            { Guid.Parse("48075D19-7B29-4CA5-9C73-0CBD31248446"), "License" },
            { Guid.Parse("0674C1C8-963A-4658-AFF9-8CDCD308FA68"), "MaidenName" },
            { Guid.Parse("1EC9583A-B019-4BAA-B856-B99CAF368656"), "OfficialRecord" },
            { Guid.Parse("2B085D38-3308-4664-9F89-48D8EF4DABA7"), "Phonetic" },
            { Guid.Parse("C31564EF-CA8D-4528-85A8-88245FCEF344"), "Pseudonym" },
            { Guid.Parse("15207687-5290-4672-A7DF-2880A23DCBB5"), "Religious" },
            { Guid.Parse("87964BFF-E442-481D-9749-69B2A84A1FBE"), "Search" },
            { Guid.Parse("E5794E3B-3025-436F-9417-5886FEEAD55A"), "Soundex" },
            { Guid.Parse("B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF"), "Syllabic" }
            };

        /// <summary>
        /// Represent as a model instance
        /// </summary>
        public override EntityName ToModelInstance(object dataInstance, LocalDataContext context)
        {
            var dbEntName = dataInstance as DbEntityName;
            var compPersister = new EntityNameComponentPersistenceService();

            return new EntityName()
            {
                Key = new Guid(dbEntName.Uuid),
                NameUseKey = dbEntName.UseConceptUuid == null ? null : (Guid?)new Guid(dbEntName.UseConceptUuid),
                SourceEntityKey = new Guid(dbEntName.SourceUuid),
                NameUse = new Concept()
                {
                    Key = new Guid(dbEntName.UseConceptUuid),
                    Mnemonic = this.m_nameUseMap[new Guid(dbEntName.UseConceptUuid)]
                },
                LoadState = OpenIZ.Core.Model.LoadState.FullLoad,
                Component = compPersister.GetFromSource(context, new Guid(dbEntName.Uuid), null).OfType<EntityNameComponent>().ToList()
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
        /// Override model instance
        /// </summary>
        public override object FromModelInstance(EntityName modelInstance, LocalDataContext context)
        {
            foreach (var itm in modelInstance.Component)
                itm.Value = itm.Value.Trim();

            modelInstance.Key = modelInstance.Key ?? Guid.NewGuid();
            return new DbEntityName()
            {
                Uuid = modelInstance.Key?.ToByteArray(),
                SourceUuid = modelInstance.SourceEntityKey?.ToByteArray(),
                UseConceptUuid = modelInstance.NameUseKey?.ToByteArray()
            };
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        protected override EntityName InsertInternal(LocalDataContext context, EntityName data)
        {

            // Ensure exists
            if (data.NameUse != null) data.NameUse = data.NameUse?.EnsureExists(context);
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

        // Existing 
        private Dictionary<String, byte[]> m_existingNames = new Dictionary<string, byte[]>();

        /// <summary>
        /// To model instance
        /// </summary>
        public override EntityNameComponent ToModelInstance(object dataInstance, LocalDataContext context)
        {
            if (dataInstance == null) return null;

            var entName = (dataInstance as DbEntityNameComponent.QueryResult)?.GetInstanceOf<DbEntityNameComponent>() ?? dataInstance as DbEntityNameComponent;
            var nameValue = (dataInstance as DbEntityNameComponent.QueryResult)?.GetInstanceOf<DbPhoneticValue>() ?? context.Connection.Table<DbPhoneticValue>().Where(o => o.Uuid == entName.ValueUuid).FirstOrDefault();
            return new EntityNameComponent()
            {
                ComponentTypeKey = new Guid(entName.ComponentTypeUuid ?? Guid.Empty.ToByteArray()),
                Value = nameValue.Value,
                Key = entName.Key,
                PhoneticAlgorithmKey = new Guid(nameValue.PhoneticAlgorithmUuid ?? Guid.Empty.ToByteArray()),
                PhoneticCode = nameValue.PhoneticCode,
                SourceEntityKey = new Guid(entName.NameUuid)
            };
        }

        /// <summary>
        /// From the model instance
        /// </summary>
        public override object FromModelInstance(EntityNameComponent modelInstance, LocalDataContext context)
        {
            modelInstance.Key = modelInstance.Key ?? Guid.NewGuid();
            var retVal = new DbEntityNameComponent()
            {
                NameUuid = modelInstance.SourceEntityKey?.ToByteArray(),
                ComponentTypeUuid = modelInstance.ComponentTypeKey?.ToByteArray(),
                Uuid = modelInstance.Key?.ToByteArray()
            };

            // Address component already exists?
            byte[] existingKey = null;
            if (String.IsNullOrEmpty(modelInstance.Value)) return retVal;
            if (!this.m_existingNames.TryGetValue(modelInstance.Value, out existingKey))
            {
                var existing = context.Connection.Table<DbPhoneticValue>().Where(o => o.Value == modelInstance.Value).FirstOrDefault();
                if (existing != null && existing.Key != retVal.Key)
                    retVal.ValueUuid = existing.Uuid;
                else if (!String.IsNullOrEmpty(modelInstance.Value))
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

                lock (this.m_existingNames)
                    if (!this.m_existingNames.ContainsKey(modelInstance.Value))
                        this.m_existingNames.Add(modelInstance.Value, retVal.ValueUuid);
            }
            else
                retVal.ValueUuid = existingKey;
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
