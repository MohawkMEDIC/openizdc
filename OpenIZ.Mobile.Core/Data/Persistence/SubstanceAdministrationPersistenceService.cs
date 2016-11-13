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
    /// Represents a persistence service for substance administrations
    /// </summary>
    public class SubstanceAdministrationPersistenceService : ActDerivedPersistenceService<SubstanceAdministration, DbSubstanceAdministration>
    {
        /// <summary>
        /// Convert databased model to model
        /// </summary>
        public override SubstanceAdministration ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var iddat = dataInstance as DbIdentified;
            var dbSbadm = dataInstance as DbSubstanceAdministration ?? context.Table<DbSubstanceAdministration>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(a => a.Uuid == dbSbadm.Uuid).First();
            var retVal = m_actPersister.ToModelInstance<SubstanceAdministration>(dba, context, loadFast);

            if (dbSbadm.DoseUnitConceptUuid != null)
                retVal.DoseUnitKey = new Guid(dbSbadm.DoseUnitConceptUuid);
            if (dbSbadm.RouteConceptUuid != null)
                retVal.RouteKey = new Guid(dbSbadm.RouteConceptUuid);
            retVal.DoseQuantity = dbSbadm.DoseQuantity;
            retVal.SequenceId = (int)dbSbadm.SequenceId;
            
            return retVal;
        }

        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override SubstanceAdministration Insert(SQLiteConnectionWithLock context, SubstanceAdministration data)
        {
            data.DoseUnit?.EnsureExists(context);
            data.Route?.EnsureExists(context);
            data.DoseUnitKey = data.DoseUnit?.Key ?? data.DoseUnitKey;
            data.RouteKey = data.Route?.Key ?? data.RouteKey;
            return base.Insert(context, data);
        }


        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override SubstanceAdministration Update(SQLiteConnectionWithLock context, SubstanceAdministration data)
        {
            data.DoseUnit?.EnsureExists(context);
            data.Route?.EnsureExists(context);
            data.DoseUnitKey = data.DoseUnit?.Key ?? data.DoseUnitKey;
            data.RouteKey = data.Route?.Key ?? data.RouteKey;
            return base.Update(context, data);
        }
    }
}