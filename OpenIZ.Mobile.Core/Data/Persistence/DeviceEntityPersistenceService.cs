/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persistence service for a device entity
    /// </summary>
    public class DeviceEntityPersistenceService : EntityDerivedPersistenceService<DeviceEntity, DbDeviceEntity, DbDeviceEntity>
    {
        /// <summary>
        /// Convert the database representation to a model instance
        /// </summary>
        public override DeviceEntity ToModelInstance(object dataInstance, LocalDataContext context)
        {
            var deviceEntity = dataInstance as DbDeviceEntity;
            var dbe = context.Connection.Table<DbEntity>().Where(o => o.Uuid == deviceEntity.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<DeviceEntity>(dbe, context);
            retVal.SecurityDeviceKey = new Guid(deviceEntity.SecurityDeviceUuid);
            retVal.ManufacturerModelName = deviceEntity.ManufacturerModelName;
            retVal.OperatingSystemName = deviceEntity.OperatingSystemName;
            //retVal.LoadAssociations(context);

            return retVal;
        }

        /// <summary>
        /// Insert the specified device entity
        /// </summary>
        protected override DeviceEntity InsertInternal(LocalDataContext context, DeviceEntity data)
        {
            if(data.SecurityDevice != null) data.SecurityDevice = data.SecurityDevice?.EnsureExists(context);
            data.SecurityDeviceKey = data.SecurityDevice?.Key ?? data.SecurityDeviceKey;

            return base.InsertInternal(context, data);
        }

        /// <summary>
        /// Updates the specified user
        /// </summary>
        protected override DeviceEntity UpdateInternal(LocalDataContext context, DeviceEntity data)
        {
            if (data.SecurityDevice != null) data.SecurityDevice = data.SecurityDevice?.EnsureExists(context);
            data.SecurityDeviceKey = data.SecurityDevice?.Key ?? data.SecurityDeviceKey;
            return base.UpdateInternal(context, data);
        }
    }
}
