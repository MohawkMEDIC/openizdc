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
 * Date: 2016-8-17
 */
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
                context.InsertAll(retVal.AuthorityScopeXml.Select(o => new DbAuthorityScope() { Key = Guid.NewGuid(), ScopeConceptUuid = o.ToByteArray(), AssigningAuthorityUuid = retVal.Key.Value.ToByteArray() }));
            }
            return retVal;
        }
    }
}
