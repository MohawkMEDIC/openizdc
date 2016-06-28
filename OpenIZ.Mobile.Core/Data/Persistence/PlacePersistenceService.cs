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
    /// Represents a persister which persists places
    /// </summary>
    public class PlacePersistenceService : EntityDerivedPersistenceService<Place, DbPlace>
    {
        /// <summary>
        /// Load to a model instance
        /// </summary>
        public override Place ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var place = dataInstance as DbPlace;
            var dbe = context.Table<DbEntity>().Where(o => o.Uuid == place.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<Place>(dbe, context);
            retVal.IsMobile = place.IsMobile;
            retVal.Lat = place.Lat;
            retVal.Lng = place.Lng;
            return retVal;
        }

        /// <summary>
        /// Insert 
        /// </summary>
        public override Place Insert(SQLiteConnection context, Place data)
        {
            var retVal = base.Insert(context, data);

            if (data.Services != null)
                base.UpdateAssociatedItems<PlaceService, Entity>(
                    new List<PlaceService>(),
                    data.Services,
                    data.Key,
                    context);

            return retVal;
        }

        /// <summary>
        /// Update the place
        /// </summary>
        public override Place Update(SQLiteConnection context, Place data)
        {
            var retVal = base.Update(context, data);

            byte[] sourceKey = data.Key.ToByteArray();
            if (data.Services != null)
                base.UpdateAssociatedItems<PlaceService, Entity>(
                    context.Table<DbPlaceService>().Where(o => o.EntityUuid == sourceKey).ToList().Select(o => m_mapper.MapDomainInstance<DbPlaceService, PlaceService>(o)).ToList(),
                    data.Services,
                    data.Key,
                    context);

            return retVal;
        }
    }
}
