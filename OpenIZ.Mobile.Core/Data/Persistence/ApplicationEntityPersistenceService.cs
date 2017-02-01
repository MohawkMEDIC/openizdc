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
 * Date: 2016-6-28
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
    /// Represents the persistence service for application eneities
    /// </summary>
    public class ApplicationEntityPersistenceService : EntityDerivedPersistenceService<ApplicationEntity, DbApplicationEntity>
    {
        /// <summary>
        /// To model instance
        /// </summary>
        public override ApplicationEntity ToModelInstance(object dataInstance, LocalDataContext context, bool loadFast)
        {
            var applicationEntity = dataInstance as DbApplicationEntity;
            var dbe = context.Connection.Table<DbEntity>().Where(o => o.Uuid == applicationEntity.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<ApplicationEntity>(dbe, context, loadFast);
            retVal.SecurityApplicationKey = new Guid(applicationEntity.SecurityApplicationUuid);
            retVal.SoftwareName = applicationEntity.SoftwareName;
            retVal.VersionName = applicationEntity.VersionName;
            retVal.VendorName = applicationEntity.VendorName;
            retVal.LoadAssociations(context);
            return retVal;
        }

        /// <summary>
        /// Insert the application entity
        /// </summary>
        protected override ApplicationEntity InsertInternal(LocalDataContext context, ApplicationEntity data)
        {
            data.SecurityApplication?.EnsureExists(context);
            data.SecurityApplicationKey = data.SecurityApplication?.Key ?? data.SecurityApplicationKey;
            return base.InsertInternal(context, data);
        }
        
        /// <summary>
        /// Update the application entity
        /// </summary>
        protected override ApplicationEntity UpdateInternal(LocalDataContext context, ApplicationEntity data)
        {
            data.SecurityApplication?.EnsureExists(context);
            data.SecurityApplicationKey = data.SecurityApplication?.Key ?? data.SecurityApplicationKey;
            return base.UpdateInternal(context, data);
        }
    }
}
