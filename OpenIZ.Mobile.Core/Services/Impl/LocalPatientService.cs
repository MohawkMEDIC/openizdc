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
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Synchronization.Model;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
	/// Represents a patient repository service.
	/// </summary>
    public class LocalPatientService : IPatientRepositoryService
    {
		private IDataPersistenceService<Patient> persistenceService;

		public LocalPatientService()
		{
			ApplicationContext.Current.Started += (o, e) => { this.persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>(); };
		}

		/// <summary>
		/// Finds a patient based on a specific query.
		/// </summary>
		/// <param name="predicate">The query to use to find the patients.</param>
		/// <returns>Returns a list of patients which match the query.</returns>
        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate)
        {
			if (persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

            return persistenceService.Query(predicate);
        }

		/// <summary>
		/// Finds a patient based on a specific query.
		/// </summary>
		/// <param name="predicate">The query to use to find the patients.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of patients which match the query.</returns>
		public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate, int offset, int? count, out int totalCount)
        {
			if (persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

			return persistenceService.Query(predicate, offset, count, out totalCount);
        }

		/// <summary>
		/// Gets a specific patient by id and version id.
		/// </summary>
		/// <param name="id">The id of the patient to be retrieved.</param>
		/// <param name="versionId">The version id of the patient to be retrieved.</param>
		/// <returns>Returns a patient.</returns>
        public Patient Get(Guid id, Guid versionId)
        {
			if (persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

			return persistenceService.Get(id);
        }

		/// <summary>
		/// Inserts a patient.
		/// </summary>
		/// <param name="p">The patient to be inserted.</param>
		/// <returns>Returns the inserted patient.</returns>
        public Patient Insert(Patient p)
        {
			if (persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

			p = this.Validate(p);

            // Persist patient
            var patient = persistenceService.Insert(p);

			SynchronizationQueue.Outbound.Enqueue(patient, DataOperationType.Insert);

            return patient;
        }

		/// <summary>
		/// Merges two patients together.
		/// </summary>
		/// <param name="survivor">The survivor record of the merge.</param>
		/// <param name="victim">The victim record of the merge.</param>
		/// <returns>Returns the merged patient.</returns>
        public Patient Merge(Patient survivor, Patient victim)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Obsoletes a specific patient by key.
		/// </summary>
		/// <param name="key">The key of the patient to be obsoleted.</param>
		/// <returns>Returns the obsoleted patient.</returns>
        public Patient Obsolete(Guid key)
        {
			if (persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

			var result = persistenceService.Obsolete(new Patient() { Key = key });

			SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Obsolete);

			return result;
        }

		/// <summary>
		/// Saves a patient.
		/// </summary>
		/// <param name="p">The patient to be saved.</param>
		/// <returns>Returns the saved patient.</returns>
        public Patient Save(Patient p)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

			if (persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

            p = this.Validate(p);

			Patient patient = null;

            try
            {
                patient = persistenceService.Update(p);
            }
            catch (KeyNotFoundException)
            {
                patient = persistenceService.Insert(p);
            }

			SynchronizationQueue.Outbound.Enqueue(patient, DataOperationType.Update);

			return patient;
        }

        /// <summary>
		/// Unmerges two patients.
		/// </summary>
		/// <param name="patient">The patient to unmerge.</param>
		/// <param name="versionKey">The version key of the patient to unmerge.</param>
		/// <returns>Returns the unmerged patient.</returns>
        public Patient UnMerge(Patient patient, Guid versionKey)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Validates a patient record.
		/// </summary>
		/// <param name="p">The patient to be validated.</param>
		/// <returns>Returns the validated patient.</returns>
        public Patient Validate(Patient p)
        {   
            p = p.Clean() as Patient; // clean up messy data
            
            // Generate temporary identifier
            if (!(p.Identifiers?.Count > 0))
			{
				p.Identifiers = new List<EntityIdentifier>()
				{
					new EntityIdentifier(new AssigningAuthority()
					{
						DomainName = "TEMP",
						Name= "Temporary Identifiers",
					}, BitConverter.ToString(Guid.NewGuid().ToByteArray(), 0, 4).Replace("-",""))
				};
			}

            return p;
        }
    }
}
