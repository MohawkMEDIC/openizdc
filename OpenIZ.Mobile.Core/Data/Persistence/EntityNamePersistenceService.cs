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
        /// Insert the specified object
        /// </summary>
        internal override EntityName Insert(SQLiteConnection context, EntityName data)
        {
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

    }

    /// <summary>
    /// Persistence for name components
    /// </summary>
    public class EntityNameComponentPersistenceService : IdentifiedPersistenceService<EntityNameComponent, DbEntityNameComponent>
    {

    }
}
