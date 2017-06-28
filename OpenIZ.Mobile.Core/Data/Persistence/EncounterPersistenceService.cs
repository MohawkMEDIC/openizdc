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
    public class EncounterPersistenceService : ActDerivedPersistenceService<PatientEncounter, DbPatientEncounter, DbPatientEncounter.QueryResult>
    {

        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(PatientEncounter modelInstance, LocalDataContext context)
        {
            modelInstance.Key = modelInstance.Key ?? Guid.NewGuid();
            return new DbPatientEncounter()
            {
                DischargeDispositionUuid = modelInstance.DischargeDispositionKey?.ToByteArray(),
                Uuid = modelInstance.Key?.ToByteArray()
            };
        }

        /// <summary>
        /// Convert database instance to patient encounter
        /// </summary>
        public override PatientEncounter ToModelInstance(object dataInstance, LocalDataContext context)
        {
            var iddat = dataInstance as DbIdentified ;
            var dbEnc = dataInstance as DbPatientEncounter ?? dataInstance.GetInstanceOf<DbPatientEncounter>() ?? context.Connection.Table<DbPatientEncounter>().Where(o => o.Uuid == iddat.Uuid).FirstOrDefault();
            var dba = dataInstance.GetInstanceOf<DbAct>() ?? dataInstance as DbAct ?? context.Connection.Table<DbAct>().Where(a => a.Uuid == dbEnc.Uuid).First();
            var retVal = m_actPersister.ToModelInstance<PatientEncounter>(dba, context);

            if (dbEnc?.DischargeDispositionUuid != null)
                retVal.DischargeDispositionKey = new Guid(dbEnc?.DischargeDispositionUuid);
            return retVal;
        }

        /// <summary>
        /// Insert the patient encounter
        /// </summary>
        protected override PatientEncounter InsertInternal(LocalDataContext context, PatientEncounter data)
        {
            if(data.DischargeDisposition != null) data.DischargeDisposition = data.DischargeDisposition?.EnsureExists(context);
            data.DischargeDispositionKey = data.DischargeDisposition?.Key ?? data.DischargeDispositionKey;
            return base.InsertInternal(context, data);
        }

        /// <summary>
        /// Updates the specified data
        /// </summary>
        protected override PatientEncounter UpdateInternal(LocalDataContext context, PatientEncounter data)
        {
            if (data.DischargeDisposition != null) data.DischargeDisposition = data.DischargeDisposition?.EnsureExists(context);
            data.DischargeDispositionKey = data.DischargeDisposition?.Key ?? data.DischargeDispositionKey;
            return base.UpdateInternal(context, data);
        }
    }
}