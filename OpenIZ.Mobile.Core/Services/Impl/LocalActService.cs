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
 * Date: 2016-8-17
 */
using MARC.HI.EHRS.SVC.Auditing.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Synchronization.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenIZ.Core.Interfaces;

namespace OpenIZ.Mobile.Core.Services.Impl
{
	/// <summary>
	/// Represents an act repository service.
	/// </summary>
	public class LocalActService : IActRepositoryService, IPersistableQueryRepositoryService, 
        IRepositoryService<Act>,
        IRepositoryService<SubstanceAdministration>,
        IRepositoryService<QuantityObservation>,
        IRepositoryService<PatientEncounter>,
        IRepositoryService<CodedObservation>,
        IRepositoryService<TextObservation>
    {
        public event EventHandler<AuditDataEventArgs> DataCreated;
        public event EventHandler<AuditDataEventArgs> DataUpdated;
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;

        public IEnumerable<Act> Find(Expression<Func<Act, bool>> query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds acts based on a specific query.
        /// </summary>
        public IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> filter, int offset, int? count, out int totalResults) where TAct : Act
		{
            return this.Find<TAct>(filter, offset, count, out totalResults, Guid.Empty);
		}

        /// <summary>
        /// Get sbadm
        /// </summary>
        public Act Get(Guid key, Guid versionKey)
        {
            return this.Get<Act>(key, Guid.Empty);
        }
        
        /// <summary>
        /// Get by key
        /// </summary>
        public Act Get(Guid key)
        {
            return this.Get<Act>(key, Guid.Empty);
        }

        /// <summary>
        /// Get the specified act
        /// </summary>
        public TAct Get<TAct>(Guid key, Guid versionId) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var result = persistenceService.Get(key);

	        if (result != null)
	        {
				result = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>()?.AfterRetrieve(result) ?? result;
			}

            // Data disclosed
            this.DataDisclosed?.Invoke(this, new AuditDataDisclosureEventArgs(key.ToString(), new object[] { result }));

            return result;
		}

        public Act Insert(Act data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert the specified act
        /// </summary>
        public TAct Insert<TAct>(TAct insert) where TAct : Act
		{
            insert = this.Validate(insert);

			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

            insert = breService?.BeforeInsert(insert) ?? insert;
			insert = persistenceService.Insert(insert);
            
            insert = breService?.AfterInsert(insert);
            
            // Patient relationships
            if (insert.Relationships.Count > 0 || insert.Participations.Count > 0)
                SynchronizationQueue.Outbound.Enqueue(Bundle.CreateBundle(insert), DataOperationType.Insert);
            else
                SynchronizationQueue.Outbound.Enqueue(insert, DataOperationType.Insert);
            //SynchronizationQueue.Outbound.Enqueue(insert, DataOperationType.Insert);

            this.DataCreated?.Invoke(this, new AuditDataEventArgs(insert));

            return insert;
		}

        public Act Obsolete(Guid key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsolete the specified act
        /// </summary>
        public TAct Obsolete<TAct>(Guid key) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

			var act = persistenceService.Get(key);

			if (act == null)
				throw new KeyNotFoundException(key.ToString());

            act = breService?.BeforeObsolete(act) ?? act;
			act = persistenceService.Obsolete(act);
            act = breService?.AfterObsolete(act) ?? act;

            
            SynchronizationQueue.Outbound.Enqueue(act, DataOperationType.Obsolete);
            this.DataObsoleted?.Invoke(this, new AuditDataEventArgs(act));

            return act;
		}

        /// <summary>
        /// Queries the Act service using the specified state query id
        /// </summary>
        public IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> filter, int offset, int? count, out int totalResults, Guid queryId) where TAct : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

            if (persistenceService == null)
                throw new InvalidOperationException("No concept persistence service found");

            var results = persistenceService.Query(filter, offset, count, out totalResults, queryId);
            results = breService?.AfterQuery(results) ?? results;
            this.DataDisclosed?.Invoke(this, new AuditDataDisclosureEventArgs(filter.ToString(), results));
            return results;
        }

        public Act Save(Act data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert or update the specified act
        /// </summary>
        public TAct Save<TAct>(TAct act) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

            // Validate act
            act = this.Validate(act);

			try
			{
                // Get older version
                if (act.Key.HasValue)
                {
                    var old = persistenceService.Get(act.Key.Value).Clone();

                    if (old == null)
                        throw new KeyNotFoundException();

                    // Fire before update
                    act = breService?.BeforeUpdate(act) ?? act;

                    // update
                    act = persistenceService.Update(act);

                    // First after update
                    act = breService?.AfterUpdate(act);
                    
                    var diff = ApplicationContext.Current.GetService<IPatchService>().Diff(old, act);

                    SynchronizationQueue.Outbound.Enqueue(diff, DataOperationType.Update);
                    ApplicationContext.Current.GetService<IDataCachingService>().Remove( act.Key.Value);
                    ApplicationContext.Current.GetService<IDataCachingService>().Remove(act.Key.Value);
                }
                else throw new KeyNotFoundException();

                this.DataUpdated?.Invoke(this, new AuditDataEventArgs(act));
                return act;
            }
            catch (KeyNotFoundException)
            {
                // Fire before update
                act = breService?.BeforeInsert(act) ?? act;

                act = persistenceService.Insert(act);

                act = breService?.AfterInsert(act);
                
                // Patient relationships
                if (act.Relationships.Count > 0 || act.Participations.Count > 0)
                    SynchronizationQueue.Outbound.Enqueue(Bundle.CreateBundle(act), DataOperationType.Insert);
                else
                    SynchronizationQueue.Outbound.Enqueue(act, DataOperationType.Insert);

                this.DataCreated?.Invoke(this, new AuditDataEventArgs(act));

                return act;
            }
        }

		/// <summary>
		/// Validates an act.
		/// </summary>
		public TAct Validate<TAct>(TAct data) where TAct : Act
		{
            var details = ApplicationContext.Current.GetService<IBusinessRulesService<Act>>()?.Validate(data) ?? new List<DetectedIssue>();
            if (details.Any(d => d.Priority == DetectedIssuePriorityType.Error))
                throw new OpenIZ.Core.Exceptions.DetectedIssueException(details);

            // Correct author information and controlling act information
            data = data.Clean() as TAct;

			ISecurityRepositoryService userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			var currentUserEntity = AuthenticationContext.Current.Session?.UserEntity;
            var currentLocation = currentUserEntity.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);
            // Set authororiginator
            if (currentUserEntity != null && !data.Participations.Any(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator || o.ParticipationRole?.Mnemonic == "Authororiginator"))
				data.Participations.Add(new ActParticipation(ActParticipationKey.Authororiginator, currentUserEntity));
            // Set location if not done
            if (currentUserEntity != null && !data.Participations.Any(o => o.ParticipationRoleKey == ActParticipationKey.EntryLocation|| o.ParticipationRole?.Mnemonic == "EntryLocation" || o.ParticipationRoleKey == ActParticipationKey.Location || o.ParticipationRole?.Mnemonic == "Location") && currentLocation != null)
                data.Participations.Add(new ActParticipation(ActParticipationKey.EntryLocation, currentLocation?.TargetEntityKey));

            return data;
		}


        /// <summary>
        /// Find specified act
        /// </summary>
        public IEnumerable<Act> Find(Expression<Func<Act, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<Act>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Get sbadm
        /// </summary>
        SubstanceAdministration IRepositoryService<SubstanceAdministration>.Get(Guid key, Guid versionKey)
        {
            return this.Get<SubstanceAdministration>(key, Guid.Empty);
        }


        /// <summary>
        /// Get sbadm
        /// </summary>
        SubstanceAdministration IRepositoryService<SubstanceAdministration>.Get(Guid key)
        {
            return this.Get<SubstanceAdministration>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<SubstanceAdministration> Find(Expression<Func<SubstanceAdministration, bool>> query)
        {
            int tr = 0;
            return this.Find<SubstanceAdministration>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<SubstanceAdministration> Find(Expression<Func<SubstanceAdministration, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<SubstanceAdministration>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public SubstanceAdministration Insert(SubstanceAdministration data)
        {
            return this.Insert<SubstanceAdministration>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public SubstanceAdministration Save(SubstanceAdministration data)
        {
            return this.Save<SubstanceAdministration>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        SubstanceAdministration IRepositoryService<SubstanceAdministration>.Obsolete(Guid key)
        {
            return this.Obsolete<SubstanceAdministration>(key);
        }

        /// <summary>
        /// Get sbadm
        /// </summary>
        QuantityObservation IRepositoryService<QuantityObservation>.Get(Guid key, Guid versionKey)
        {
            return this.Get<QuantityObservation>(key, Guid.Empty);
        }

        /// <summary>
        /// Get sbadm
        /// </summary>
        QuantityObservation IRepositoryService<QuantityObservation>.Get(Guid key)
        {
            return this.Get<QuantityObservation>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<QuantityObservation> Find(Expression<Func<QuantityObservation, bool>> query)
        {
            int tr = 0;
            return this.Find<QuantityObservation>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<QuantityObservation> Find(Expression<Func<QuantityObservation, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<QuantityObservation>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public QuantityObservation Insert(QuantityObservation data)
        {
            return this.Insert<QuantityObservation>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public QuantityObservation Save(QuantityObservation data)
        {
            return this.Save<QuantityObservation>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        QuantityObservation IRepositoryService<QuantityObservation>.Obsolete(Guid key)
        {
            return this.Obsolete<QuantityObservation>(key);
        }

        /// <summary>
        /// Get sbadm
        /// </summary>
        CodedObservation IRepositoryService<CodedObservation>.Get(Guid key, Guid versionKey)
        {
            return this.Get<CodedObservation>(key, Guid.Empty);
        }


        /// <summary>
        /// Get sbadm
        /// </summary>
        CodedObservation IRepositoryService<CodedObservation>.Get(Guid key)
        {
            return this.Get<CodedObservation>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<CodedObservation> Find(Expression<Func<CodedObservation, bool>> query)
        {
            int tr = 0;
            return this.Find<CodedObservation>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<CodedObservation> Find(Expression<Func<CodedObservation, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<CodedObservation>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public CodedObservation Insert(CodedObservation data)
        {
            return this.Insert<CodedObservation>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public CodedObservation Save(CodedObservation data)
        {
            return this.Save<CodedObservation>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        CodedObservation IRepositoryService<CodedObservation>.Obsolete(Guid key)
        {
            return this.Obsolete<CodedObservation>(key);
        }

        /// <summary>
        /// Get sbadm
        /// </summary>
        TextObservation IRepositoryService<TextObservation>.Get(Guid key, Guid versionKey)
        {
            return this.Get<TextObservation>(key, Guid.Empty);
        }



        /// <summary>
        /// Get sbadm
        /// </summary>
        TextObservation IRepositoryService<TextObservation>.Get(Guid key)
        {
            return this.Get<TextObservation>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<TextObservation> Find(Expression<Func<TextObservation, bool>> query)
        {
            int tr = 0;
            return this.Find<TextObservation>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<TextObservation> Find(Expression<Func<TextObservation, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<TextObservation>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public TextObservation Insert(TextObservation data)
        {
            return this.Insert<TextObservation>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public TextObservation Save(TextObservation data)
        {
            return this.Save<TextObservation>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        TextObservation IRepositoryService<TextObservation>.Obsolete(Guid key)
        {
            return this.Obsolete<TextObservation>(key);
        }

        /// <summary>
        /// Get sbadm
        /// </summary>
        PatientEncounter IRepositoryService<PatientEncounter>.Get(Guid key, Guid versionKey)
        {
            return this.Get<PatientEncounter>(key, Guid.Empty);
        }


        /// <summary>
        /// Get sbadm
        /// </summary>
        PatientEncounter IRepositoryService<PatientEncounter>.Get(Guid key)
        {
            return this.Get<PatientEncounter>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<PatientEncounter> Find(Expression<Func<PatientEncounter, bool>> query)
        {
            int tr = 0;
            return this.Find<PatientEncounter>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<PatientEncounter> Find(Expression<Func<PatientEncounter, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<PatientEncounter>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public PatientEncounter Insert(PatientEncounter data)
        {
            return this.Insert<PatientEncounter>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public PatientEncounter Save(PatientEncounter data)
        {
            return this.Save<PatientEncounter>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        PatientEncounter IRepositoryService<PatientEncounter>.Obsolete(Guid key)
        {
            return this.Obsolete<PatientEncounter>(key);
        }

    }
}