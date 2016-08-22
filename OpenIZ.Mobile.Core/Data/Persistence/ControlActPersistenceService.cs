using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using SQLite.Net;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Control act persistence service
    /// </summary>
    public class ControlActPersistenceService : ActDerivedPersistenceService<ControlAct, DbControlAct>
    {
        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override ControlAct ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var iddat = dataInstance as DbIdentified;
            var controlAct = dataInstance as DbControlAct ?? context.Table<DbControlAct>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(a => a.Uuid == controlAct.Uuid).First();
            // TODO: Any other cact fields
            return m_actPersister.ToModelInstance<ControlAct>(dba, context, loadFast);
        }
    }
}