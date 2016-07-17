using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Versioned domain data
    /// </summary>
    public abstract class VersionedDataPersistenceService<TModel, TDomain> : BaseDataPersistenceService<TModel, TDomain> 
        where TDomain : DbVersionedData, new() 
        where TModel : VersionedEntityData<TModel>, new()
    {

        /// <summary>
        /// Insert the data
        /// </summary>
        public override TModel Insert(SQLiteConnection context, TModel data)
        {
            data.VersionKey = data.VersionKey == Guid.Empty || !data.VersionKey.HasValue ? Guid.NewGuid() : data.VersionKey ;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the data with new version information
        /// </summary>
        public override TModel Update(SQLiteConnection context, TModel data)
        {
            data.PreviousVersionKey = data.VersionKey;
            data.VersionKey = Guid.NewGuid();
            return base.Update(context, data);
        }
    }
}
