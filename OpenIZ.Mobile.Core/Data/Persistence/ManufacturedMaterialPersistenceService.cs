/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-2-4
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model;
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
    public class ManufacturedMaterialPersistenceService : IdentifiedPersistenceService<ManufacturedMaterial, DbManufacturedMaterial, DbManufacturedMaterial.QueryResult>
    {
        // Material persister
        private MaterialPersistenceService m_materialPersister = new MaterialPersistenceService();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override IEnumerable<ManufacturedMaterial> SortResults(IEnumerable<ManufacturedMaterial> data)
        {
            return data.OrderBy(d => d.ExpiryDate);
        }
        /// <summary>
        /// Material persister
        /// </summary>
        /// <param name="dataInstance"></param>
        /// <param name="context"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public override ManufacturedMaterial ToModelInstance(object dataInstance, LocalDataContext context)
        {

            var iddat = dataInstance as DbIdentified;
            var domainMmat = dataInstance as DbManufacturedMaterial ?? dataInstance.GetInstanceOf<DbManufacturedMaterial>() ?? context.Connection.Table<DbManufacturedMaterial>().Where(o=>o.Uuid == iddat.Uuid).First();
            var domainMat = dataInstance as DbMaterial ?? dataInstance.GetInstanceOf<DbMaterial>() ?? context.Connection.Table<DbMaterial>().Where(o=>o.Uuid == iddat.Uuid).First();
            //var dbm = domainMat ?? context.Table<DbMaterial>().Where(o => o.Uuid == domainMmat.Uuid).First();
            var retVal = this.m_materialPersister.ToModelInstance<ManufacturedMaterial>(domainMat, context);
            retVal.LotNumber = domainMmat.LotNumber;
            return retVal;

        }

        /// <summary>
        /// Insert the specified manufactured material
        /// </summary>
        protected override ManufacturedMaterial InsertInternal(LocalDataContext context, ManufacturedMaterial data)
        {
            var retVal = this.m_materialPersister.Insert(context, data);
            return base.InsertInternal(context, data);
        }

        /// <summary>
        /// Updates the manufactured material
        /// </summary>
        protected override ManufacturedMaterial UpdateInternal(LocalDataContext context, ManufacturedMaterial data)
        {
            var updated = this.m_materialPersister.Update(context, data);
            return base.UpdateInternal(context, data);
        }

        /// <summary>
        /// Obsolete the specified manufactured material
        /// </summary>
        protected override ManufacturedMaterial ObsoleteInternal(LocalDataContext context, ManufacturedMaterial data)
        {
            var obsoleted = this.m_materialPersister.Obsolete(context, data);
            return data;
        }
    }
}
