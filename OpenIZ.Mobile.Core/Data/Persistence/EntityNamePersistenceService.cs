using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity name persistence service
    /// </summary>
    public class EntityNamePersistenceService : IdentifiedPersistenceService<EntityName, DbEntityName>
    {
        /// <summary>
        /// Represents the name as a model instance
        /// </summary>
        public override EntityName ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            DbEntityName en = dataInstance as DbEntityName;
            var retVal = base.ToModelInstance(dataInstance, context);
            retVal.Component = new List<EntityNameComponent>(context.Table<DbEntityNameComponent>().Where(o => o.NameUuid == en.Uuid).ToArray().Select(o => new EntityNameComponent(new Guid(o.ComponentTypeUuid), o.Value)));
            return retVal;

        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override EntityName Insert(SQLiteConnection context, EntityName data)
        {

            // Ensure exists
            data.NameUse?.EnsureExists(context);

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
        public override EntityName Update(SQLiteConnection context, EntityName data)
        {
            // Ensure exists
            data.NameUse?.EnsureExists(context);

            var retVal = base.Update(context, data);

            var sourceKey = data.Key.ToByteArray();

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
