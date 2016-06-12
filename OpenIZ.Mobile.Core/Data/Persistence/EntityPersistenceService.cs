using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Data.Model.DataType;
using OpenIZ.Mobile.Core.Data.Model.Extensibility;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity persistence service
    /// </summary>
    public class EntityPersistenceService : VersionedDataPersistenceService<Entity, DbEntity>
    {

        /// <summary>
        /// To model instance
        /// </summary>
        public virtual TEntityType ToModelInstance<TEntityType>(DbEntity dbInstance, SQLiteConnection context) where TEntityType : Entity, new()
        {
            return m_mapper.MapDomainInstance<DbEntity, TEntityType>(dbInstance);
        }

        /// <summary>
        /// Insert the specified entity into the data context
        /// </summary>
        public override Entity Insert(SQLiteConnection context, Entity data)
        {

            // Ensure FK exists
            data.ClassConcept?.EnsureExists(context);
            data.DeterminerConcept?.EnsureExists(context);
            data.StatusConcept?.EnsureExists(context);
            data.TypeConcept?.EnsureExists(context);

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
                    data.Relationships,
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

            return retVal;
        }

        /// <summary>
        /// Update the specified entity
        /// </summary>
        public override Entity Update(SQLiteConnection context, Entity data)
        {
            // Esnure exists
            data.ClassConcept?.EnsureExists(context);
            data.DeterminerConcept?.EnsureExists(context);
            data.StatusConcept?.EnsureExists(context);
            data.TypeConcept?.EnsureExists(context);

            var retVal = base.Update(context, data);

            byte[] entityUuid = retVal.Key.ToByteArray();

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
                    data.Relationships,
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

            return retVal;
        }

        /// <summary>
        /// Obsoleted status key
        /// </summary>
        public override Entity Obsolete(SQLiteConnection context, Entity data)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Obsolete(context, data);
        }
    }
}
