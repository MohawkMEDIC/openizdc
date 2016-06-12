using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persistence service for entity addresses
    /// </summary>
    public class EntityAddressPersistenceService : IdentifiedPersistenceService<EntityAddress, DbEntityAddress>
    {

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override EntityAddress Insert(SQLiteConnection context, EntityAddress data)
        {

            // Ensure exists
            data.AddressUse?.EnsureExists(context);

            var retVal = base.Insert(context, data);

            // Data component
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
        public override EntityAddress Update(SQLiteConnection context, EntityAddress data)
        {

            // Ensure exists
            data.AddressUse?.EnsureExists(context);

            var retVal = base.Update(context, data);

            var sourceKey = data.Key.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<EntityAddressComponent, EntityAddress>(
                    context.Table<DbEntityAddressComponent>().Where(o => o.AddressUuid == sourceKey).ToList().Select(o => m_mapper.MapDomainInstance<DbEntityAddressComponent, EntityAddressComponent>(o)).ToList(),
                    data.Component,
                    data.Key,
                    context);

            return retVal;
        }

    }


}
