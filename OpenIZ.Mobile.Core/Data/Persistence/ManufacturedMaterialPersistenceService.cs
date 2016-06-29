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
        public override ManufacturedMaterial ToModelInstance(object dataInstance, SQLiteConnection context)
        {

            var domainMmat = dataInstance as DbManufacturedMaterial;
            var domainMat = dataInstance as DbMaterial;
            var dbm = context.Table<DbMaterial>().FirstOrDefault(o => o.Uuid == domainMat.Uuid);
            var retVal = this.m_materialPersister.ToModelInstance<ManufacturedMaterial>(dbm, context);
            retVal.LotNumber = domainMmat.LotNumber;
            return base.ToModelInstance(dataInstance, context);

        }

        /// <summary>
        /// Insert the specified manufactured material
        /// </summary>
        public override ManufacturedMaterial Insert(SQLiteConnection context, ManufacturedMaterial data)
        {
            var retVal = this.m_materialPersister.Insert(context, data);
            return base.Insert(context, data);
        }

        /// <summary>
        /// Updates the manufactured material
        /// </summary>
        public override ManufacturedMaterial Update(SQLiteConnection context, ManufacturedMaterial data)
        {
            var updated = this.m_materialPersister.Update(context, data);
            return base.Update(context, data);
        }

        /// <summary>
        /// Obsolete the specified manufactured material
        /// </summary>
        public override ManufacturedMaterial Obsolete(SQLiteConnection context, ManufacturedMaterial data)
        {
            var obsoleted = this.m_materialPersister.Obsolete(context, data);
            return data;
        }
    }
}
