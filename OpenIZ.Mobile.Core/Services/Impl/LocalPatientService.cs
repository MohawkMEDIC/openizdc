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
 * Date: 2016-7-8
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Synchronization.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Services.Impl
{
	/// <summary>
	/// Represents a patient repository service.
	/// </summary>
	public class LocalPatientService : EntityRepositoryBase, IPatientRepositoryService, IRepositoryService<Patient>
	{

        /// <summary>
        /// Locates the specified patient.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Returns a list of patients which match the specific predicate.</returns>
        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate)
        {
            var totalResults = 0;
            return this.Find(predicate, 0, null, out totalResults);
        }

        /// <summary>
        /// Finds the specified patient with query controls.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns>Returns a list of patient which match the specific predicate.</returns>
        /// <exception cref="System.InvalidOperationException">If the persistence service is not found.</exception>
        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate, int offset, int? count, out int totalCount)
        {
            return base.Find(predicate, offset, count, out totalCount, Guid.Empty, true);
        }

        /// <summary>
        /// Finds the specified patient with query controls.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns>Returns a list of patient which match the specific predicate.</returns>
        /// <exception cref="System.InvalidOperationException">If the persistence service is not found.</exception>
        public IEnumerable<Patient> FindFast(Expression<Func<Patient, bool>> predicate, int offset, int? count, out int totalCount, Guid queryId)
        {
            return base.Find(predicate, offset, count, out totalCount, queryId, true);
        }

        /// <summary>
        /// Gets the specified patient.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="versionId">The version identifier.</param>
        /// <returns>Returns the patient or null if no patient is found.</returns>
        /// <exception cref="System.InvalidOperationException">If the persistence service is not found.</exception>
        public Patient Get(Guid id, Guid versionId)
        {
            return base.Get<Patient>(id, versionId);
        }

        /// <summary>
        /// Gets the specified patient.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="versionId">The version identifier.</param>
        /// <returns>Returns the patient or null if no patient is found.</returns>
        /// <exception cref="System.InvalidOperationException">If the persistence service is not found.</exception>
        public Patient Get(Guid id)
        {
            return base.Get<Patient>(id, Guid.Empty);

        }

        /// <summary>
        /// Insert the specified patient
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns>Returns the inserted patient.</returns>
        /// <exception cref="System.InvalidOperationException">If the persistence service is not found.</exception>
        public Patient Insert(Patient patient)
        {
            patient = base.Insert(patient);
            
            return patient;
        }

        /// <summary>
        /// Merges two patients together
        /// </summary>
        /// <param name="survivor">The surviving patient record</param>
        /// <param name="victim">The victim patient record</param>
        /// <returns>A new version of patient <paramref name="a" /> representing the merge</returns>
        /// <exception cref="System.InvalidOperationException">If the persistence service is not found.</exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public Patient Merge(Patient survivor, Patient victim)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<Patient>)} not found");
            }
            
            // TODO: Do this
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsoletes the specified patient
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the obsoleted patient.</returns>
        /// <exception cref="System.InvalidOperationException">If the persistence service is not found or if the patient is not found.</exception>
        public Patient Obsolete(Guid key)
        {
            return base.Obsolete<Patient>(key);
        }

        /// <summary>
        /// Save / update the patient.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns>Returns the updated patient.</returns>
        /// <exception cref="System.InvalidOperationException">If the persistence service is not found.</exception>
        public Patient Save(Patient patient)
        {

            patient = base.Save(patient);

           
            return patient;
        }

        /// <summary>
        /// Un-merge two patients
        /// </summary>
        public Patient UnMerge(Patient patient, Guid versionKey)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<Patient>)} not found");
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate the patient and prepare for storage
        /// </summary>
        public Patient Validate(Patient p)
        {
            return base.Validate(p);
        }
    }
}