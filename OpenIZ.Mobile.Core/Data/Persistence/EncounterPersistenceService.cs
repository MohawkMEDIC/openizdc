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
 * Date: 2016-7-24
 */
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
            var dbEnc = dataInstance as DbPatientEncounter ?? context.Table<DbPatientEncounter>().Where(o => o.Uuid == iddat.Uuid).FirstOrDefault();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(a => a.Uuid == dbEnc.Uuid).First();
            var retVal = m_actPersister.ToModelInstance<PatientEncounter>(dba, context, loadFast);

            if (dbEnc?.DischargeDispositionUuid != null)
                retVal.DischargeDispositionKey = new Guid(dbEnc?.DischargeDispositionUuid);
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