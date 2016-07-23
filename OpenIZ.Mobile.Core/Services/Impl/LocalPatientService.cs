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
 * Date: 2016-7-8
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
    /// Local patient repository service
    /// </summary>
    public class LocalPatientService : IPatientRepositoryService
    {
        

        /// <summary>
        /// Locates the specified patient
        /// </summary>
        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate)
        {
            // Repository service
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            return persistenceService.Query(predicate);
        }

        /// <summary>
        /// Finds the specified patient with query controls
        /// </summary>
        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate, int offset, int? count, out int totalCount)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            return persistenceService.Query(predicate, offset, count, out totalCount);
        }

        /// <summary>
        /// Gets the specified patient
        /// </summary>
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

            return persistenceService.Get(id);
        }

        /// <summary>
        /// Insert the specified patient
        /// </summary>
        public Patient Insert(Patient p)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

            return persistenceService.Insert(p);
        }

        /// <summary>
        /// Merges two patients together
        /// </summary>
        public Patient Merge(Patient survivor, Patient victim)
        {
            // TODO: Do this
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsoletes the specified patient
        /// </summary>
        public Patient Obsolete(Guid key)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            return persistenceService.Obsolete(new Patient() { Key = key });
        }

        /// <summary>
        /// Save / update the patient
        /// </summary>
        public Patient Save(Patient p)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            try
            {
                return persistenceService.Update(p);
            }
            catch (KeyNotFoundException)
            {
                return persistenceService.Insert(p);
            }
        }

        /// <summary>
        /// Un-merge two patients
        /// </summary>
        public Patient UnMerge(Patient patient, Guid versionKey)
        {
            throw new NotImplementedException();
        }
    }
}
