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
    /// Represents the persistence service for application eneities
    /// </summary>
    public class ApplicationEntityPersistenceService : EntityDerivedPersistenceService<ApplicationEntity, DbApplicationEntity>
    {
        /// <summary>
        /// To model instance
        /// </summary>
        public override ApplicationEntity ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var applicationEntity = dataInstance as DbApplicationEntity;
            var dbe = context.Table<DbEntity>().Where(o => o.Uuid == applicationEntity.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<ApplicationEntity>(dbe, context);
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
        public override ApplicationEntity Insert(SQLiteConnection context, ApplicationEntity data)
        {
            data.SecurityApplication?.EnsureExists(context);
            data.SecurityApplicationKey = data.SecurityApplication?.Key ?? data.SecurityApplicationKey;
            return base.Insert(context, data);
        }
        
        /// <summary>
        /// Update the application entity
        /// </summary>
        public override ApplicationEntity Update(SQLiteConnection context, ApplicationEntity data)
        {
            data.SecurityApplication?.EnsureExists(context);
            data.SecurityApplicationKey = data.SecurityApplication?.Key ?? data.SecurityApplicationKey;
            return base.Update(context, data);
        }
    }
}
