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
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
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
	/// Represents an act repository service.
	/// </summary>
	public class LocalActService : IActRepositoryService, IPersistableQueryProvider
	{

		/// <summary>
		/// Finds acts based on a specific query.
		/// </summary>
		public IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> filter, int offset, int? count, out int totalResults) where TAct : Act
		{
            var results = this.Query(filter, offset, count, out totalResults, Guid.Empty);
            results = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>()?.AfterQuery(results) ?? results;
            return results;
		}

		/// <summary>
		/// Get the specified act
		/// </summary>
		public TAct Get<TAct>(Guid key, Guid versionId) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

			var result = persistenceService.Get(key);
            result = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>()?.AfterRetrieve(result) ?? result;
            return result;
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
            insert = breService?.AfterInsert(insert) ?? insert;

            // Patient relationships
            if (insert.Relationships.Count > 0 || insert.Participations.Count > 0)
                SynchronizationQueue.Outbound.Enqueue(Bundle.CreateBundle(insert), DataOperationType.Insert);
            else
                SynchronizationQueue.Outbound.Enqueue(insert, DataOperationType.Insert);
            //SynchronizationQueue.Outbound.Enqueue(insert, DataOperationType.Insert);

            return insert;
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

            return act;
		}

        /// <summary>
        /// Queries the Act service using the specified state query id
        /// </summary>
        public IEnumerable<TAct> Query<TAct>(Expression<Func<TAct, bool>> filter, int offset, int? count, out int totalResults, Guid queryId) where TAct : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

            if (persistenceService == null)
                throw new InvalidOperationException("No concept persistence service found");

            var results = persistenceService.Query(filter, offset, count, out totalResults, queryId);
            results = breService?.AfterQuery(results) ?? results;
            return results;
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

                    // Fire before update
                    act = breService?.BeforeUpdate(act) ?? act;

                    // update
                    act = persistenceService.Update(act);

                    // First after update
                    act = breService?.AfterUpdate(act) ?? act;

                    var diff = ApplicationContext.Current.GetService<IPatchService>().Diff(old, act);

                    SynchronizationQueue.Outbound.Enqueue(diff, DataOperationType.Update);

                }
                else throw new KeyNotFoundException();
                return act;
            }
            catch (KeyNotFoundException)
            {
                // Fire before update
                act = breService?.BeforeInsert(act) ?? act;

                act = persistenceService.Insert(act);

                act = breService?.AfterInsert(act) ?? act;

                // Patient relationships
                if (act.Relationships.Count > 0 || act.Participations.Count > 0)
                    SynchronizationQueue.Outbound.Enqueue(Bundle.CreateBundle(act), DataOperationType.Insert);
                else
                    SynchronizationQueue.Outbound.Enqueue(act, DataOperationType.Insert);

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

			if (currentUserEntity != null && !data.Participations.Any(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator || o.ParticipationRole?.Mnemonic == "Authororiginator"))
			{
				data.Participations.Add(new ActParticipation(ActParticipationKey.Authororiginator, currentUserEntity));
			}

			return data;
		}
	}
}