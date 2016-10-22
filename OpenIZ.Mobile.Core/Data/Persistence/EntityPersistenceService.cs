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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Data.Model.DataType;
using OpenIZ.Mobile.Core.Data.Model.Extensibility;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model.Concepts;

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
        public virtual TEntityType ToModelInstance<TEntityType>(DbEntity dbInstance, SQLiteConnectionWithLock context, bool loadFast) where TEntityType : Entity, new()
        {
            var retVal = m_mapper.MapDomainInstance<DbEntity, TEntityType>(dbInstance, useCache: !context.IsInTransaction);
            
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

            // Now we want to load the relationships inversed!
            retVal.LoadAssociations(context);

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
        public override Entity ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            // Alright first, which type am I mapping to?
            var dbEntity = dataInstance as DbEntity;
            switch (new Guid(dbEntity.ClassConceptUuid).ToString().ToUpper())
            {
                case Device:
                    return new DeviceEntityPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case NonLivingSubject:
                    return new ApplicationEntityPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case Person:
                    return new PersonPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case Patient:
                    return new PatientPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case Provider:
                    return new ProviderPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case Place:
                case CityOrTown:
                case Country:
                case CountyOrParish:
                case State:
                case ServiceDeliveryLocation:
                    return new PlacePersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case Organization:
                    return new OrganizationPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case Material:
                    return new MaterialPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case ManufacturedMaterial:
                    return new ManufacturedMaterialPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                default:
                    return this.ToModelInstance<Entity>(dbEntity, context, loadFast);

            }
        }

        /// <summary>
        /// Insert the specified entity into the data context
        /// </summary>
        public override Entity Insert(SQLiteConnectionWithLock context, Entity data)
        {

            // Ensure FK exists
            data.ClassConcept?.EnsureExists(context);
            data.DeterminerConcept?.EnsureExists(context);
            data.StatusConcept?.EnsureExists(context);
            data.TypeConcept?.EnsureExists(context);
            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.DeterminerConceptKey = data.DeterminerConcept?.Key ?? data.DeterminerConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;

            data.StatusConceptKey = data.StatusConceptKey.GetValueOrDefault() == Guid.Empty ? StatusKeys.New : data.StatusConceptKey;

            var retVal = base.Insert(context, data);


            // Identifiers
            if (data.Identifiers != null)
                base.UpdateAssociatedItems<EntityIdentifier, Entity>(
                    new List<EntityIdentifier>(),
                    data.Identifiers,
                    retVal.Key,
                    context);

            // Relationships
            if (data.Relationships != null)
                base.UpdateAssociatedItems<EntityRelationship, Entity>(
                    new List<EntityRelationship>(),
                    data.Relationships.Where(o=>!o.InversionIndicator).ToList(),
                    retVal.Key,
                    context);

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

            // Ensure participations
            if (data.Participations != null)
                foreach (var itm in data.Participations)
                {
                    itm.PlayerEntityKey = retVal.Key;
                    itm.EnsureExists(context);
                }
            return retVal;
        }

        /// <summary>
        /// Update the specified entity
        /// </summary>
        public override Entity Update(SQLiteConnectionWithLock context, Entity data)
        {
            // Esnure exists
            data.ClassConcept?.EnsureExists(context);
            data.DeterminerConcept?.EnsureExists(context);
            data.StatusConcept?.EnsureExists(context);
            data.TypeConcept?.EnsureExists(context);

            var retVal = base.Update(context, data);

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
                base.UpdateAssociatedItems<EntityIdentifier, Entity>(
                    context.Table<DbEntityIdentifier>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityIdentifier, EntityIdentifier>(o)).ToList(),
                    data.Identifiers,
                    retVal.Key,
                    context);

            // Relationships
            if (data.Relationships != null)
                base.UpdateAssociatedItems<EntityRelationship, Entity>(
                    context.Table<DbEntityRelationship>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityRelationship, EntityRelationship>(o)).ToList(),
                    data.Relationships.Where(o => !o.InversionIndicator).ToList(),
                    retVal.Key,
                    context);

            // Telecoms
            if (data.Telecoms != null)
                base.UpdateAssociatedItems<EntityTelecomAddress, Entity>(
                    context.Table<DbTelecomAddress>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbTelecomAddress, EntityTelecomAddress>(o)).ToList(),
                    data.Telecoms,
                    retVal.Key,
                    context);

            // Extensions
            if (data.Extensions != null)
                base.UpdateAssociatedItems<EntityExtension, Entity>(
                    context.Table<DbEntityExtension>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityExtension, EntityExtension>(o)).ToList(),
                    data.Extensions,
                    retVal.Key,
                    context);

            // Names
            if (data.Names != null)
                base.UpdateAssociatedItems<EntityName, Entity>(
                    context.Table<DbEntityName>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityName, EntityName>(o)).ToList(),
                    data.Names,
                    retVal.Key,
                    context);

            // Addresses
            if (data.Addresses != null)
                base.UpdateAssociatedItems<EntityAddress, Entity>(
                    context.Table<DbEntityAddress>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityAddress, EntityAddress>(o)).ToList(),
                    data.Addresses,
                    retVal.Key,
                    context);

            // Notes
            if (data.Notes != null)
                base.UpdateAssociatedItems<EntityNote, Entity>(
                    context.Table<DbEntityNote>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityNote, EntityNote>(o)).ToList(),
                    data.Notes,
                    retVal.Key,
                    context);

            // Tags
            if (data.Tags != null)
                base.UpdateAssociatedItems<EntityTag, Entity>(
                    context.Table<DbEntityTag>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityTag, EntityTag>(o)).ToList(),
                    data.Tags,
                    retVal.Key,
                    context);

            // Participations
            if(data.Participations != null)
            {
                foreach (var itm in data.Participations)
                {
                    itm.PlayerEntityKey = retVal.Key;
                    itm.Act?.EnsureExists(context);
                    itm.SourceEntityKey = itm.Act?.Key ?? itm.SourceEntityKey;
                } 
                var existing = context.Table<DbActParticipation>().Where(o => o.EntityUuid == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbActParticipation, ActParticipation>(o)).ToList();
                base.UpdateAssociatedItems<ActParticipation, Act>(
                    existing,
                    data.Participations,
                    retVal.Key,
                    context);
            }
          

            return retVal;
        }

        private TableQuery<object> DbActPersistence()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsoleted status key
        /// </summary>
        public override Entity Obsolete(SQLiteConnectionWithLock context, Entity data)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Obsolete(context, data);
        }
    }
}
