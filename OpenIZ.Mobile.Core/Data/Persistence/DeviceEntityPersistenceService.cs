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
    /// Represents a persistence service for a device entity
    /// </summary>
    public class DeviceEntityPersistenceService : EntityDerivedPersistenceService<DeviceEntity, DbDeviceEntity>
    {
        /// <summary>
        /// Convert the database representation to a model instance
        /// </summary>
        public override DeviceEntity ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var deviceEntity = dataInstance as DbDeviceEntity;
            var dbe = context.Table<DbEntity>().Where(o => o.Uuid == deviceEntity.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<DeviceEntity>(dbe, context);
            retVal.SecurityDeviceKey = new Guid(deviceEntity.SecurityDeviceUuid);
            retVal.ManufacturedModelName = deviceEntity.ManufacturerModelName;
            retVal.OperatingSystemName = deviceEntity.OperatingSystemName;
            return retVal;
        }

        /// <summary>
        /// Insert the specified device entity
        /// </summary>
        public override DeviceEntity Insert(SQLiteConnection context, DeviceEntity data)
        {
            data.SecurityDevice?.EnsureExists(context);
            data.SecurityDeviceKey = data.SecurityDevice?.Key ?? data.SecurityDeviceKey;

            return base.Insert(context, data);
        }

        /// <summary>
        /// Updates the specified user
        /// </summary>
        public override DeviceEntity Update(SQLiteConnection context, DeviceEntity data)
        {
            data.SecurityDevice?.EnsureExists(context);
            data.SecurityDeviceKey = data.SecurityDevice?.Key ?? data.SecurityDeviceKey;
            return base.Update(context, data);
        }
    }
}
