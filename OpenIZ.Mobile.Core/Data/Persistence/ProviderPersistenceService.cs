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
 * Date: 2016-7-17
 */
using OpenIZ.Core.Model.Roles;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Roles;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Provider persistence service
    /// </summary>
    public class ProviderPersistenceService : IdentifiedPersistenceService<Provider, DbProvider>
    {
        // Entity persisters
        private PersonPersistenceService m_personPersister = new PersonPersistenceService();
        protected EntityPersistenceService m_entityPersister = new EntityPersistenceService();

        /// <summary>
        /// Model instance
        /// </summary>
        public override Provider ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var iddat = dataInstance as DbVersionedData;
            var provider = dataInstance as DbProvider ?? context.Table<DbProvider>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbe = dataInstance as DbEntity ?? context.Table<DbEntity>().Where(o => o.Uuid == provider.Uuid).First();
            var dbp = context.Table<DbPerson>().Where(o => o.Uuid == provider.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<Provider>(dbe, context);

            retVal.DateOfBirth = dbp.DateOfBirth;
            // Reverse lookup
            // Reverse lookup
            if (!String.IsNullOrEmpty(dbp.DateOfBirthPrecision))
                retVal.DateOfBirthPrecision = PersonPersistenceService.PrecisionMap.Where(o => o.Value == dbp.DateOfBirthPrecision).Select(o => o.Key).First();
            retVal.ProviderSpecialtyKey = provider.Specialty == null ? null : (Guid?)new Guid(provider.Specialty);
            retVal.LoadAssociations(context);

            return retVal;
        }

        /// <summary>
        /// Insert the specified person into the database
        /// </summary>
        public override Provider Insert(SQLiteConnection context, Provider data)
        {
            data.ProviderSpecialty?.EnsureExists(context);
            data.ProviderSpecialtyKey = data.ProviderSpecialty?.Key ?? data.ProviderSpecialtyKey;

            var inserted = this.m_personPersister.Insert(context, data);
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified person
        /// </summary>
        public override Provider Update(SQLiteConnection context, Provider data)
        {
            // Ensure exists
            data.ProviderSpecialty?.EnsureExists(context);
            data.ProviderSpecialtyKey = data.ProviderSpecialty?.Key ?? data.ProviderSpecialtyKey;

            this.m_personPersister.Update(context, data);
            return base.Update(context, data);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override Provider Obsolete(SQLiteConnection context, Provider data)
        {
            var retVal = this.m_personPersister.Obsolete(context, data);
            return data;
        }

    }
}
