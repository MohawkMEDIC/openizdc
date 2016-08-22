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
 * Date: 2016-7-2
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Manufactured material persistence service
    /// </summary>
    public class ManufacturedMaterialPersistenceService : IdentifiedPersistenceService<ManufacturedMaterial, DbManufacturedMaterial>
    {
        // Material persister
        private MaterialPersistenceService m_materialPersister = new MaterialPersistenceService();

        /// <summary>
        /// Material persister
        /// </summary>
        /// <param name="dataInstance"></param>
        /// <param name="context"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public override ManufacturedMaterial ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {

            var domainMmat = dataInstance as DbManufacturedMaterial;
            var domainMat = dataInstance as DbMaterial;
            var dbm = domainMat ?? context.Table<DbMaterial>().Where(o => o.Uuid == domainMmat.Uuid).First();
            var retVal = this.m_materialPersister.ToModelInstance<ManufacturedMaterial>(dbm, context, loadFast);
            retVal.LotNumber = domainMmat.LotNumber;
            return retVal;

        }

        /// <summary>
        /// Insert the specified manufactured material
        /// </summary>
        public override ManufacturedMaterial Insert(SQLiteConnectionWithLock context, ManufacturedMaterial data)
        {
            var retVal = this.m_materialPersister.Insert(context, data);
            return base.Insert(context, data);
        }

        /// <summary>
        /// Updates the manufactured material
        /// </summary>
        public override ManufacturedMaterial Update(SQLiteConnectionWithLock context, ManufacturedMaterial data)
        {
            var updated = this.m_materialPersister.Update(context, data);
            return base.Update(context, data);
        }

        /// <summary>
        /// Obsolete the specified manufactured material
        /// </summary>
        public override ManufacturedMaterial Obsolete(SQLiteConnectionWithLock context, ManufacturedMaterial data)
        {
            var obsoleted = this.m_materialPersister.Obsolete(context, data);
            return data;
        }
    }
}
