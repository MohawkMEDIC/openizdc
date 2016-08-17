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
        public override SubstanceAdministration ToModelInstance(object dataInstance, SQLiteConnectionWithLock context)
        {
            var iddat = dataInstance as DbIdentified;
            var dbSbadm = dataInstance as DbSubstanceAdministration ?? context.Table<DbSubstanceAdministration>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(a => a.Uuid == dbSbadm.Uuid).First();
            var retVal = m_actPersister.ToModelInstance<SubstanceAdministration>(dba, context);

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