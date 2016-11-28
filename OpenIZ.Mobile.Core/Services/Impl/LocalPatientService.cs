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
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
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
	public class LocalPatientService : IPatientRepositoryService
	{
		/// <summary>
		/// The internal reference to the <see cref="IDataPersistenceService{TData}"/> instance.
		/// </summary>
		private IDataPersistenceService<Patient> m_persistenceService;

        /// <summary>
        /// Internal reference to the bre service
        /// </summary>
        private IBusinessRulesService<Patient> m_breService;

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalPatientService"/> class.
		/// </summary>
		public LocalPatientService()
		{
			ApplicationContext.Current.Started += (o, e) => {
                this.m_persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
                this.m_breService = ApplicationContext.Current.GetService<IBusinessRulesService<Patient>>();
            };
		}

		/// <summary>
		/// Finds a patient based on a specific query.
		/// </summary>
		/// <param name="predicate">The query to use to find the patients.</param>
		/// <returns>Returns a list of patients which match the query.</returns>
		public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate)
		{
			if (this.m_persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

			var results = this.m_persistenceService.Query(predicate);
            results = this.m_breService?.AfterQuery(results) ?? results;
            return results;
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
			if (this.m_persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

			var results = m_persistenceService.Query(predicate, offset, count, out totalCount, Guid.Empty);
            results = this.m_breService?.AfterQuery(results) ?? results;
            return results;
		}

		/// <summary>
		/// Gets a specific patient by id and version id.
		/// </summary>
		/// <param name="id">The id of the patient to be retrieved.</param>
		/// <param name="versionId">The version id of the patient to be retrieved.</param>
		/// <returns>Returns a patient.</returns>
		public Patient Get(Guid id, Guid versionId)
		{
			if (this.m_persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

			var result = m_persistenceService.Get(id);
            result = this.m_breService?.AfterRetrieve(result) ?? result;
            return result;
		}

		/// <summary>
		/// Inserts a patient.
		/// </summary>
		/// <param name="p">The patient to be inserted.</param>
		/// <returns>Returns the inserted patient.</returns>
		public Patient Insert(Patient p)
		{
			if (this.m_persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

			p = this.Validate(p);

            p = this.m_breService?.BeforeInsert(p) ?? p;

			// Persist patient
			var patient = this.m_persistenceService.Insert(p);
            p = this.m_breService?.AfterInsert(p) ?? p;

            if(patient.Relationships.Count > 0 || patient.Participations.Count > 0)
                SynchronizationQueue.Outbound.Enqueue(Bundle.CreateBundle(patient), DataOperationType.Insert);
            else
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
			if (this.m_persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

            var p = this.m_persistenceService.Get(key);
            if (p == null)
                throw new KeyNotFoundException(key.ToString());
            p = this.m_breService?.BeforeObsolete(p) ?? p;
            var result = this.m_persistenceService.Obsolete(p);
            p = this.m_breService?.AfterObsolete(result) ?? result;
            
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
			if (this.m_persistenceService == null)
			{
				throw new ArgumentException(string.Format("{0} not found", nameof(IDataPersistenceService<Patient>)));
			}

			p = this.Validate(p);

			Patient patient = null;

			try
			{
               
                // Get older version
                if (p.Key.HasValue)
                {
                    var old = this.m_persistenceService.Get(p.Key.Value)?.Clone();

                    if (old == null) throw new KeyNotFoundException();

                    // Fire before update
                    p = this.m_breService?.BeforeUpdate(p) ?? p;

                    // update
                    patient = this.m_persistenceService.Update(p);

                    // First after update
                    patient = this.m_breService?.AfterUpdate(p) ?? p;

                    var diff = ApplicationContext.Current.GetService<IPatchService>().Diff(old, patient);

                    SynchronizationQueue.Outbound.Enqueue(diff, DataOperationType.Update);

                }
                else throw new KeyNotFoundException();

            }
			catch (KeyNotFoundException)
			{
                // Fire before update
                p = this.m_breService?.BeforeInsert(p) ?? p;

                patient = this.m_persistenceService.Insert(p);

                patient = this.m_breService?.AfterInsert(patient) ?? patient;

                // Patient relationships
                if (patient.Relationships.Count > 0 || patient.Participations.Count > 0)
                    SynchronizationQueue.Outbound.Enqueue(Bundle.CreateBundle(patient), DataOperationType.Insert);
                else
                    SynchronizationQueue.Outbound.Enqueue(patient, DataOperationType.Insert);

            }

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
            var details = this.m_breService?.Validate(p) ?? new List<DetectedIssue>();
            if (details?.Any(d => d.Priority == DetectedIssuePriorityType.Error) == true)
                throw new OpenIZ.Core.Exceptions.DetectedIssueException(details);

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
                        Oid = "0.0.0.0",
					}, BitConverter.ToString(Guid.NewGuid().ToByteArray(), 0, 4).Replace("-",""))
				};
			}

			return p;
		}
	}
}