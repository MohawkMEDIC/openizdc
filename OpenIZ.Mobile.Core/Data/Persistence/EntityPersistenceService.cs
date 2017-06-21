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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Data.Model.DataType;
using OpenIZ.Mobile.Core.Data.Model.Extensibility;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model.Concepts;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Roles;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Exceptions;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity persistence service
    /// </summary>
    public class EntityPersistenceService : VersionedDataPersistenceService<Entity, DbEntity>
    {


        private const String Entity = "E29FCFAD-EC1D-4C60-A055-039A494248AE";
        private const String ManufacturedMaterial = "FAFEC286-89D5-420B-9085-054ACA9D1EEF";
        private const String Animal = "61FCBF42-B5E0-4FB5-9392-108A5C6DBEC7";
        private const String Place = "21AB7873-8EF3-4D78-9C19-4582B3C40631";
        private const String Device = "1373FF04-A6EF-420A-B1D0-4A07465FE8E8";
        private const String Organization = "7C08BD55-4D42-49CD-92F8-6388D6C4183F";
        private const String Food = "E5A09CC2-5AE5-40C2-8E32-687DBA06715D";
        private const String Material = "D39073BE-0F8F-440E-B8C8-7034CC138A95";
        private const String Person = "9DE2A846-DDF2-4EBC-902E-84508C5089EA";
        private const String CityOrTown = "79DD4F75-68E8-4722-A7F5-8BC2E08F5CD6";
        private const String ChemicalSubstance = "2E9FA332-9391-48C6-9FC8-920A750B25D3";
        private const String State = "8CF4B0B0-84E5-4122-85FE-6AFA8240C218";
        private const String Container = "B76FF324-B174-40B7-A6AC-D1FDF8E23967";
        private const String LivingSubject = "8BA5E5C9-693B-49D4-973C-D7010F3A23EE";
        private const String Patient = "BACD9C6F-3FA9-481E-9636-37457962804D";
        private const String ServiceDeliveryLocation = "FF34DFA7-C6D3-4F8B-BC9F-14BCDC13BA6C";
        private const String Provider = "6B04FED8-C164-469C-910B-F824C2BDA4F0";
        private const String CountyOrParish = "D9489D56-DDAC-4596-B5C6-8F41D73D8DC5";
        private const String Country = "48B2FFB3-07DB-47BA-AD73-FC8FB8502471";
        private const String NonLivingSubject = "9025E5C9-693B-49D4-973C-D7010F3A23EE";

        /// <summary>
        /// To model instance
        /// </summary>
        public virtual TEntityType ToModelInstance<TEntityType>(DbEntity dbInstance, LocalDataContext context) where TEntityType : Entity, new()
        {
            var retVal = m_mapper.MapDomainInstance<DbEntity, TEntityType>(dbInstance, useCache: !context.Connection.IsInTransaction);

            // Has this been updated? If so, minimal information about the previous version is available
            if (dbInstance.UpdatedTime != null)
            {
                retVal.CreationTime = (DateTimeOffset)dbInstance.UpdatedTime;
                retVal.CreatedByKey = dbInstance.UpdatedByKey;
                retVal.PreviousVersion = new Entity()
                {
                    ClassConcept = retVal.ClassConcept,
                    DeterminerConcept = retVal.DeterminerConcept,
                    Key = dbInstance.Key,
                    VersionKey = dbInstance.PreviousVersionKey,
                    CreationTime = (DateTimeOffset)dbInstance.CreationTime,
                    CreatedByKey = dbInstance.CreatedByKey
                };
            }


            retVal.LoadAssociations(context,
                // Exclude
                nameof(OpenIZ.Core.Model.Entities.Entity.Extensions),
                nameof(OpenIZ.Core.Model.Entities.Entity.Notes),
                nameof(OpenIZ.Core.Model.Entities.Entity.Participations),
                nameof(OpenIZ.Core.Model.Entities.Entity.Telecoms),
                nameof(OpenIZ.Core.Model.Entities.UserEntity.SecurityUser)
                );

            //if (!loadFast)
            //{
            //    foreach (var itm in retVal.Relationships.Where(o => !o.InversionIndicator && o.TargetEntity == null))
            //        itm.TargetEntity = this.CacheConvert(context.Get<DbEntity>(itm.TargetEntityKey.Value.ToByteArray()), context, true);
            //    retVal.Relationships.RemoveAll(o => o.InversionIndicator);
            //    retVal.Relationships.AddRange(
            //        context.Table<DbEntityRelationship>().Where(o => o.TargetUuid == dbInstance.Uuid).ToList().Select(o => new EntityRelationship(new Guid(o.RelationshipTypeUuid), new Guid(o.TargetUuid))
            //        {
            //            SourceEntityKey = new Guid(o.EntityUuid),
            //            InversionIndicator = true
            //        })
            //    );
            //    retVal.Participations = new List<ActParticipation>(context.Table<DbActParticipation>().Where(o => o.EntityUuid == dbInstance.Uuid).ToList().Select(o => new ActParticipation(new Guid(o.ParticipationRoleUuid), retVal)
            //    {
            //        ActKey = new Guid(o.ActUuid),
            //        Key = o.Key
            //    }));
            //}


            return retVal;
        }

        /// <summary>
        /// Create an appropriate entity based on the class code
        /// </summary>
        public override Entity ToModelInstance(object dataInstance, LocalDataContext context)
        {
            // Alright first, which type am I mapping to?
            var dbEntity = dataInstance as DbEntity;
            if (dbEntity != null)
                switch (new Guid(dbEntity.ClassConceptUuid).ToString().ToUpper())
                {
                    case Device:
                        return new DeviceEntityPersistenceService().ToModelInstance(dataInstance, context);
                    case NonLivingSubject:
                        return new ApplicationEntityPersistenceService().ToModelInstance(dataInstance, context);
                    case Person:
                        return new PersonPersistenceService().ToModelInstance(dataInstance, context);
                    case Patient:
                        return new PatientPersistenceService().ToModelInstance(dataInstance, context);
                    case Provider:
                        return new ProviderPersistenceService().ToModelInstance(dataInstance, context);
                    case Place:
                    case CityOrTown:
                    case Country:
                    case CountyOrParish:
                    case State:
                    case ServiceDeliveryLocation:
                        return new PlacePersistenceService().ToModelInstance(dataInstance, context);
                    case Organization:
                        return new OrganizationPersistenceService().ToModelInstance(dataInstance, context);
                    case Material:
                        return new MaterialPersistenceService().ToModelInstance(dataInstance, context);
                    case ManufacturedMaterial:
                        return new ManufacturedMaterialPersistenceService().ToModelInstance(dataInstance, context);
                    default:
                        return this.ToModelInstance<Entity>(dbEntity, context);

                }
            else if (dataInstance is DbDeviceEntity)
                return new DeviceEntityPersistenceService().ToModelInstance(dataInstance, context);
            else if (dataInstance is DbApplicationEntity)
                return new ApplicationEntityPersistenceService().ToModelInstance(dataInstance, context);
            else if (dataInstance is DbPerson)
                return new PersonPersistenceService().ToModelInstance(dataInstance, context);
            else if (dataInstance is DbPatient)
                return new PatientPersistenceService().ToModelInstance(dataInstance, context);
            else if (dataInstance is DbProvider)
                return new ProviderPersistenceService().ToModelInstance(dataInstance, context);
            else if (dataInstance is DbPlace)
                return new PlacePersistenceService().ToModelInstance(dataInstance, context);
            else if (dataInstance is DbOrganization)
                return new OrganizationPersistenceService().ToModelInstance(dataInstance, context);
            else if (dataInstance is DbMaterial)
                return new MaterialPersistenceService().ToModelInstance(dataInstance, context);
            else if (dataInstance is DbManufacturedMaterial)
                return new ManufacturedMaterialPersistenceService().ToModelInstance(dataInstance, context);
            else
                return null;

        }

        /// <summary>
        /// Convert entity into a dbentity
        /// </summary>
        public override object FromModelInstance(Entity modelInstance, LocalDataContext context)
        {
            modelInstance.Key = modelInstance.Key ?? Guid.NewGuid();
            return new DbEntity()
            {
                ClassConceptUuid = modelInstance.ClassConceptKey?.ToByteArray(),
                CreatedByUuid = modelInstance.CreatedByKey?.ToByteArray(),
                CreationTime = modelInstance.CreationTime,
                DeterminerConceptUuid = modelInstance.DeterminerConceptKey?.ToByteArray(),
                ObsoletedByUuid = modelInstance.ObsoletedByKey?.ToByteArray(),
                ObsoletionTime = modelInstance.ObsoletionTime,
                PreviousVersionUuid = modelInstance.PreviousVersionKey?.ToByteArray(),
                StatusConceptUuid = modelInstance.StatusConceptKey?.ToByteArray(),
                TemplateUuid = modelInstance.TemplateKey?.ToByteArray(),
                TypeConceptUuid = modelInstance.TypeConceptKey?.ToByteArray(),
                Uuid = modelInstance.Key?.ToByteArray() ,
                VersionSequenceId = (int)modelInstance.VersionSequence.GetValueOrDefault(),
                VersionUuid = modelInstance.VersionKey?.ToByteArray()
            };
        }

        /// <summary>
        /// Conversion based on type
        /// </summary>
        protected override Entity CacheConvert(DbIdentified dataInstance, LocalDataContext context)
        {
            return this.DoCacheConvert(dataInstance, context);
        }

        /// <summary>
        /// Perform the cache convert
        /// </summary>
        internal Entity DoCacheConvert(DbIdentified dataInstance, LocalDataContext context)
        {
            if (dataInstance == null)
                return null;
            // Alright first, which type am I mapping to?
            var dbEntity = dataInstance as DbEntity;
            Entity retVal = null;
            IDataCachingService cache = ApplicationContext.Current.GetService<IDataCachingService>();

            if(dbEntity != null)
                switch (new Guid(dbEntity.ClassConceptUuid).ToString().ToUpper())
                {
                    case Device:
                        retVal = cache?.GetCacheItem<DeviceEntity>(dbEntity.Key);
                        break;
                    case NonLivingSubject:
                        retVal = cache?.GetCacheItem<ApplicationEntity>(dbEntity.Key);
                        break;
                    case Person:
                        retVal = cache?.GetCacheItem<UserEntity>(dbEntity.Key);
                        if(retVal == null)
                            retVal = cache?.GetCacheItem<Person>(dbEntity.Key);
                        break;
                    case Patient:
                        retVal = cache?.GetCacheItem<Patient>(dbEntity.Key);
                        break;
                    case Provider:
                        retVal = cache?.GetCacheItem<Provider>(dbEntity.Key);

                        break;
                    case Place:
                    case CityOrTown:
                    case Country:
                    case CountyOrParish:
                    case State:
                    case ServiceDeliveryLocation:
                        retVal = cache?.GetCacheItem<Place>(dbEntity.Key);

                        break;
                    case Organization:
                        retVal = cache?.GetCacheItem<Organization>(dbEntity.Key);

                        break;
                    case Material:
                        retVal = cache?.GetCacheItem<Material>(dbEntity.Key);

                        break;
                    case ManufacturedMaterial:
                        retVal = cache?.GetCacheItem<ManufacturedMaterial>(dbEntity.Key);

                        break;
                    default:
                        retVal = cache?.GetCacheItem<Entity>(dbEntity.Key);
                        break;
                }
            else if (dataInstance is DbDeviceEntity)
                retVal = cache?.GetCacheItem<DeviceEntity>(dataInstance.Key);
            else if (dataInstance is DbApplicationEntity)
                retVal = cache?.GetCacheItem<ApplicationEntity>(dataInstance.Key);
            else if (dataInstance is DbPerson)
                retVal = cache?.GetCacheItem<UserEntity>(dataInstance.Key);
            else if (dataInstance is DbPatient)
                retVal = cache?.GetCacheItem<Patient>(dataInstance.Key);
            else if (dataInstance is DbProvider)
                retVal = cache?.GetCacheItem<Provider>(dataInstance.Key);
            else if (dataInstance is DbPlace)
                retVal = cache?.GetCacheItem<Place>(dataInstance.Key);
            else if (dataInstance is DbOrganization)
                retVal = cache?.GetCacheItem<Organization>(dataInstance.Key);
            else if (dataInstance is DbMaterial)
                retVal = cache?.GetCacheItem<Material>(dataInstance.Key);
            else if (dataInstance is DbManufacturedMaterial)
                retVal = cache?.GetCacheItem<ManufacturedMaterial>(dataInstance.Key);

            // Return cache value
            if (retVal != null)
            {
                if (retVal.LoadState < context.DelayLoadMode)
                    retVal.LoadAssociations(context,
                        // Exclude
                        nameof(OpenIZ.Core.Model.Entities.Entity.Extensions),
                        nameof(OpenIZ.Core.Model.Entities.Entity.Notes),
                        nameof(OpenIZ.Core.Model.Entities.Entity.Participations),
                        nameof(OpenIZ.Core.Model.Entities.Entity.Telecoms),
                        nameof(OpenIZ.Core.Model.Entities.UserEntity.SecurityUser)
                        );
                return retVal;
            }
            else
                return base.CacheConvert(dataInstance, context);
        }

        /// <summary>
        /// Insert the specified entity into the data context
        /// </summary>
        internal Entity InsertCoreProperties(LocalDataContext context, Entity data)
        {

            // Ensure FK exists
            if(data.ClassConcept != null) data.ClassConcept = data.ClassConcept.EnsureExists(context);
            if (data.DeterminerConcept != null) data.DeterminerConcept = data.DeterminerConcept.EnsureExists(context);
            if (data.StatusConcept != null) data.StatusConcept = data.StatusConcept.EnsureExists(context);
            if (data.TypeConcept != null) data.TypeConcept = data.TypeConcept.EnsureExists(context);
            if (data.Template != null) data.Template = data.Template.EnsureExists(context);

            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.DeterminerConceptKey = data.DeterminerConcept?.Key ?? data.DeterminerConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;
            data.TemplateKey = data.Template?.Key ?? data.TemplateKey;
            data.StatusConceptKey = data.StatusConceptKey.GetValueOrDefault() == Guid.Empty ? StatusKeys.New : data.StatusConceptKey;

            var retVal = base.InsertInternal(context, data);

            // Identifiers
            if (data.Identifiers != null)
            {
				// Validate unique values for IDs
				var uniqueIds = data.Identifiers.Where(o => o.AuthorityKey.HasValue).Where(o => ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>().Get(o.AuthorityKey.Value)?.IsUnique == true);
				byte[] entId = data.Key.Value.ToByteArray();

				foreach (var itm in uniqueIds)
				{
					byte[] authId = itm.Authority.Key.Value.ToByteArray();
					if (context.Connection.Table<DbEntityIdentifier>().Count(o => o.SourceUuid != entId && o.AuthorityUuid == authId && o.Value == itm.Value) > 0)
						throw new DuplicateKeyException(itm.Value);
				}

				base.UpdateAssociatedItems<EntityIdentifier, Entity>(
                    new List<EntityIdentifier>(),
                    data.Identifiers,
                    retVal.Key,
                    context);

            }

            // Relationships
            if (data.Relationships != null)
            {
                data.Relationships.RemoveAll(o => o.IsEmpty());
                base.UpdateAssociatedItems<EntityRelationship, Entity>(
                    new List<EntityRelationship>(),
                    data.Relationships.Where(o => o.SourceEntityKey == data.Key || o.TargetEntityKey == data.Key || !o.TargetEntityKey.HasValue ).Distinct(new EntityRelationshipPersistenceService.Comparer()).ToList(),
                    retVal.Key,
                    context);
            }

            // Telecoms
            if (data.Telecoms != null)
                base.UpdateAssociatedItems<EntityTelecomAddress, Entity>(
                    new List<EntityTelecomAddress>(),
                    data.Telecoms,
                    retVal.Key,
                    context);

            // Extensions
            if (data.Extensions != null)
                base.UpdateAssociatedItems<EntityExtension, Entity>(
                    new List<EntityExtension>(),
                    data.Extensions,
                    retVal.Key,
                    context);

            // Names
            if (data.Names != null)
                base.UpdateAssociatedItems<EntityName, Entity>(
                    new List<EntityName>(),
                    data.Names,
                    retVal.Key,
                    context);

            // Addresses
            if (data.Addresses != null)
                base.UpdateAssociatedItems<EntityAddress, Entity>(
                    new List<EntityAddress>(),
                    data.Addresses,
                    retVal.Key,
                    context);

            // Notes
            if (data.Notes != null)
                base.UpdateAssociatedItems<EntityNote, Entity>(
                    new List<EntityNote>(),
                    data.Notes,
                    retVal.Key,
                    context);

            // Tags
            if (data.Tags != null)
                base.UpdateAssociatedItems<EntityTag, Entity>(
                    new List<EntityTag>(),
                    data.Tags,
                    retVal.Key,
                    context);

            // Participations = The source is not the patient so we don't touch
            //if (data.Participations != null)
            //    foreach (var itm in data.Participations)
            //    {
            //        itm.PlayerEntityKey = retVal.Key;
            //        itm.EnsureExists(context);
            //    }
            return retVal;
        }

        /// <summary>
        /// Update the specified entity
        /// </summary>
        internal Entity UpdateCoreProperties(LocalDataContext context, Entity data)
        {
            // Esnure exists
            if (data.ClassConcept != null) data.ClassConcept = data.ClassConcept.EnsureExists(context);
            if (data.DeterminerConcept != null) data.DeterminerConcept = data.DeterminerConcept.EnsureExists(context);
            if (data.StatusConcept != null) data.StatusConcept = data.StatusConcept.EnsureExists(context);
            if (data.TypeConcept != null) data.TypeConcept = data.TypeConcept.EnsureExists(context);
            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.DeterminerConceptKey = data.DeterminerConcept?.Key ?? data.DeterminerConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;


            var retVal = base.UpdateInternal(context, data);

            byte[] entityUuid = retVal.Key.Value.ToByteArray();


            // Set appropriate versioning 
            retVal.PreviousVersion = new Entity()
            {
                ClassConcept = retVal.ClassConcept,
                Key = retVal.Key,
                VersionKey = retVal.PreviousVersionKey,
                CreationTime = (DateTimeOffset)retVal.CreationTime,
                CreatedByKey = retVal.CreatedByKey
            };
            retVal.CreationTime = DateTimeOffset.Now;
            retVal.CreatedByKey = data.CreatedByKey == Guid.Empty || data.CreatedByKey == null ? base.CurrentUserUuid(context) : data.CreatedByKey;


            // Identifiers
            if (data.Identifiers != null)
            {
                // Validate unique values for IDs
                var uniqueIds = data.Identifiers.Where(o => o.AuthorityKey.HasValue).Where(o => ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>().Get(o.AuthorityKey.Value)?.IsUnique == true);
                byte[] entId = data.Key.Value.ToByteArray();

                foreach (var itm in uniqueIds)
                {
                    byte[] authId = itm.Key.Value.ToByteArray();
                    if (context.Connection.Table<DbEntityIdentifier>().Count(o => o.SourceUuid != entId && o.AuthorityUuid == authId && o.Value == itm.Value) > 0)
                        throw new DuplicateKeyException(itm.Value);
                }

                base.UpdateAssociatedItems<EntityIdentifier, Entity>(
                    context.Connection.Table<DbEntityIdentifier>().Where(o => o.SourceUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityIdentifier, EntityIdentifier>(o)).ToList(),
                    data.Identifiers,
                    retVal.Key,
                    context);

            }

            // Relationships
            if (data.Relationships != null)
            {
                data.Relationships.RemoveAll(o => o.IsEmpty());

                base.UpdateAssociatedItems<EntityRelationship, Entity>(
                    context.Connection.Table<DbEntityRelationship>().Where(o => o.SourceUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityRelationship, EntityRelationship>(o)).ToList(),
                    data.Relationships.Where(o => o.SourceEntityKey == data.Key || o.TargetEntityKey == data.Key || !o.TargetEntityKey.HasValue).Distinct(new EntityRelationshipPersistenceService.Comparer()).ToList(),
                    retVal.Key,
                    context);
            }

            // Telecoms
            if (data.Telecoms != null)
                base.UpdateAssociatedItems<EntityTelecomAddress, Entity>(
                    context.Connection.Table<DbTelecomAddress>().Where(o => o.SourceUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbTelecomAddress, EntityTelecomAddress>(o)).ToList(),
                    data.Telecoms,
                    retVal.Key,
                    context);

            // Extensions
            if (data.Extensions != null)
                base.UpdateAssociatedItems<EntityExtension, Entity>(
                    context.Connection.Table<DbEntityExtension>().Where(o => o.SourceUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityExtension, EntityExtension>(o)).ToList(),
                    data.Extensions,
                    retVal.Key,
                    context);

            // Names
            if (data.Names != null)
                base.UpdateAssociatedItems<EntityName, Entity>(
                    context.Connection.Table<DbEntityName>().Where(o => o.SourceUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityName, EntityName>(o)).ToList(),
                    data.Names,
                    retVal.Key,
                    context);

            // Addresses
            if (data.Addresses != null)
                base.UpdateAssociatedItems<EntityAddress, Entity>(
                    context.Connection.Table<DbEntityAddress>().Where(o => o.SourceUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityAddress, EntityAddress>(o)).ToList(),
                    data.Addresses,
                    retVal.Key,
                    context);

            // Notes
            if (data.Notes != null)
                base.UpdateAssociatedItems<EntityNote, Entity>(
                    context.Connection.Table<DbEntityNote>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityNote, EntityNote>(o)).ToList(),
                    data.Notes,
                    retVal.Key,
                    context);

            // Tags
            if (data.Tags != null)
                base.UpdateAssociatedItems<EntityTag, Entity>(
                    context.Connection.Table<DbEntityTag>().Where(o => o.SourceUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityTag, EntityTag>(o)).ToList(),
                    data.Tags,
                    retVal.Key,
                    context);

            // Participations - We don't touch as Act > Participation
            //if(data.Participations != null)
            //{
            //    foreach (var itm in data.Participations)
            //    {
            //        itm.PlayerEntityKey = retVal.Key;
            //        itm.Act?.EnsureExists(context);
            //        itm.SourceEntityKey = itm.Act?.Key ?? itm.SourceEntityKey;
            //    } 
            //    var existing = context.Table<DbActParticipation>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbActParticipation, ActParticipation>(o)).ToList();
            //    base.UpdateAssociatedItems<ActParticipation, Act>(
            //        existing,
            //        data.Participations,
            //        retVal.Key,
            //        context, 
            //        true);
            //}
          

            return retVal;
        }

        private TableQuery<object> DbActPersistence()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsoleted status key
        /// </summary>
        protected override Entity ObsoleteInternal(LocalDataContext context, Entity data)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.ObsoleteInternal(context, data);
        }

        /// <summary>
        /// Insert the object
        /// </summary>
        protected override Entity InsertInternal(LocalDataContext context, Entity data)
        {
            switch (data.ClassConceptKey.ToString().ToUpper())
            {
                case Device:
                    return new DeviceEntityPersistenceService().Insert(context, data as DeviceEntity);
                case NonLivingSubject:
                    return new ApplicationEntityPersistenceService().Insert(context, data as ApplicationEntity);
                case Person:
                    return new PersonPersistenceService().Insert(context, data as Person);
                case Patient:
                    return new PatientPersistenceService().Insert(context, data as Patient);
                case Provider:
                    return new ProviderPersistenceService().Insert(context, data as Provider);
                case Place:
                case CityOrTown:
                case Country:
                case CountyOrParish:
                case State:
                case ServiceDeliveryLocation:
                    return new PlacePersistenceService().Insert(context, data as Place);
                case Organization:
                    return new OrganizationPersistenceService().Insert(context, data as Organization);
                case Material:
                    return new MaterialPersistenceService().Insert(context, data as Material);
                case ManufacturedMaterial:
                    return new ManufacturedMaterialPersistenceService().Insert(context, data as ManufacturedMaterial);
                default:
                    return this.InsertCoreProperties(context, data);

            }
        }

        /// <summary>
        /// Insert the object
        /// </summary>
        protected override Entity UpdateInternal(LocalDataContext context, Entity data)
        {
            switch (data.ClassConceptKey.ToString().ToUpper())
            {
                case Device:
                    return new DeviceEntityPersistenceService().Update(context, data as DeviceEntity);
                case NonLivingSubject:
                    return new ApplicationEntityPersistenceService().Update(context, data as ApplicationEntity);
                case Person:
                    return new PersonPersistenceService().Update(context, data as Person);
                case Patient:
                    return new PatientPersistenceService().Update(context, data as Patient);
                case Provider:
                    return new ProviderPersistenceService().Update(context, data as Provider);
                case Place:
                case CityOrTown:
                case Country:
                case CountyOrParish:
                case State:
                case ServiceDeliveryLocation:
                    return new PlacePersistenceService().Update(context, data as Place);
                case Organization:
                    return new OrganizationPersistenceService().Update(context, data as Organization);
                case Material:
                    return new MaterialPersistenceService().Update(context, data as Material);
                case ManufacturedMaterial:
                    return new ManufacturedMaterialPersistenceService().Update(context, data as ManufacturedMaterial);
                default:
                    return this.UpdateCoreProperties(context, data);

            }
        }
    }
}
