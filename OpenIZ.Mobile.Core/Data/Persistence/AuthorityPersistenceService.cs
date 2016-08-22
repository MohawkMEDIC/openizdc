using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Data.Model.DataType;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persistence service for authorities
    /// </summary>
    public class AuthorityPersistenceService : BaseDataPersistenceService<AssigningAuthority, DbAssigningAuthority>
    {

        /// <summary>
        /// Convert assigning authority to model
        /// </summary>
        public override AssigningAuthority ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var dataAA = dataInstance as DbAssigningAuthority;
            var retVal = base.ToModelInstance(dataInstance, context, loadFast);
            retVal.AuthorityScopeXml = context.Table<DbAuthorityScope>().Where(o => o.AssigningAuthorityUuid == dataAA.Uuid).ToList().Select(o=>new Guid(o.ScopeConceptUuid)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified data
        /// </summary>
        public override AssigningAuthority Insert(SQLiteConnectionWithLock context, AssigningAuthority data)
        {
            var retVal = base.Insert(context, data);

            // Scopes?
            if (retVal.AuthorityScopeXml != null)
                context.InsertAll(retVal.AuthorityScopeXml.Select(o => new DbAuthorityScope() { Key = Guid.NewGuid(), ScopeConceptUuid = o.ToByteArray(), AssigningAuthorityUuid = retVal.Key.Value.ToByteArray() }));
            return retVal;
        }

        /// <summary>
        /// Update the specified data
        /// </summary>
        public override AssigningAuthority Update(SQLiteConnectionWithLock context, AssigningAuthority data)
        {
            var retVal = base.Update(context, data);
            var ruuid = retVal.Key.Value.ToByteArray();
            // Scopes?
            if (retVal.AuthorityScopeXml != null)
            {
                foreach (var itm in context.Table<DbAuthorityScope>().Where(o => o.Uuid == ruuid))
                    context.Delete(itm);
                context.InsertAll(retVal.AuthorityScopeXml.Select(o => new DbAuthorityScope() { ScopeConceptUuid = o.ToByteArray(), AssigningAuthorityUuid = retVal.Key.Value.ToByteArray() }));
            }
            return retVal;
        }
    }
}
