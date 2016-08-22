using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using SQLite.Net;
using System;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Persistence class which persists encounters
    /// </summary>
    public class EncounterPersistenceService : ActDerivedPersistenceService<PatientEncounter, DbPatientEncounter>
    {

        /// <summary>
        /// Convert database instance to patient encounter
        /// </summary>
        public override PatientEncounter ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var iddat = dataInstance as DbIdentified;
            var dbEnc = dataInstance as DbPatientEncounter ?? context.Table<DbPatientEncounter>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(a => a.Uuid == dbEnc.Uuid).First();
            var retVal = m_actPersister.ToModelInstance<PatientEncounter>(dba, context, loadFast);

            if (dbEnc.DischargeDispositionUuid != null)
                retVal.DischargeDispositionKey = new Guid(dbEnc.DischargeDispositionUuid);
            return retVal;
        }

        /// <summary>
        /// Insert the patient encounter
        /// </summary>
        public override PatientEncounter Insert(SQLiteConnectionWithLock context, PatientEncounter data)
        {
            data.DischargeDisposition?.EnsureExists(context);
            data.DischargeDispositionKey = data.DischargeDisposition?.Key ?? data.DischargeDispositionKey;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Updates the specified data
        /// </summary>
        public override PatientEncounter Update(SQLiteConnectionWithLock context, PatientEncounter data)
        {
            data.DischargeDisposition?.EnsureExists(context);
            data.DischargeDispositionKey = data.DischargeDisposition?.Key ?? data.DischargeDispositionKey;
            return base.Update(context, data);
        }
    }
}