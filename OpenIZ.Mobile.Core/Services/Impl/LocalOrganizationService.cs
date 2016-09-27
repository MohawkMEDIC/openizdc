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
 * User: khannan
 * Date: 2016-9-27
 */

using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Synchronization.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Services.Impl
{
	/// <summary>
	/// Represents an organization repository service.
	/// </summary>
	public class LocalOrganizationService : IOrganizationRepositoryService
	{
		/// <summary>
		/// The internal reference to the <see cref="IDataPersistenceService{TData}"/> instance.
		/// </summary>
		private IDataPersistenceService<Organization> persistenceService;

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalOrganizationService"/> class.
		/// </summary>
		public LocalOrganizationService()
		{
			ApplicationContext.Current.Started += (o, e) => { this.persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Organization>>(); };
		}

		/// <summary>
		/// Searches for a organization using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the organization.</param>
		/// <returns>Returns a list of organizations who match the specified query.</returns>
		public IEnumerable<Organization> Find(Expression<Func<Organization, bool>> query)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Organization>)));
			}

			return this.persistenceService.Query(query);
		}

		/// <summary>
		/// Searches for a organization using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the organization.</param>
		/// <param name="count">The count of the organizations to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of organizations who match the specified query.</returns>
		public IEnumerable<Organization> Find(Expression<Func<Organization, bool>> query, int offset, int? count, out int totalCount)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Organization>)));
			}

			return this.persistenceService.Query(query, offset, count, out totalCount);
		}

		/// <summary>
		/// Gets the specified organization.
		/// </summary>
		/// <param name="id">The id of the organization.</param>
		/// <param name="versionId">The version id of the organization.</param>
		/// <returns>Returns the specified organization.</returns>
		public Organization Get(Guid id, Guid versionId)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Organization>)));
			}

			return this.persistenceService.Get(id);
		}

		/// <summary>
		/// Inserts the specified organization.
		/// </summary>
		/// <param name="organization">The organization to insert.</param>
		/// <returns>Returns the inserted organization.</returns>
		public Organization Insert(Organization organization)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Organization>)));
			}

			var result = this.persistenceService.Insert(organization);

			SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Insert);

			return result;
		}

		/// <summary>
		/// Obsoletes the specified organization.
		/// </summary>
		/// <param name="id">The id of the organization to obsolete.</param>
		/// <returns>Returns the obsoleted organization.</returns>
		public Organization Obsolete(Guid id)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Organization>)));
			}

			var result = this.persistenceService.Obsolete(new Organization { Key = id });

			SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Obsolete);

			return result;
		}

		/// <summary>
		/// Saves the specified organization.
		/// </summary>
		/// <param name="organization">The organization to save.</param>
		/// <returns>Returns the saved organization.</returns>
		public Organization Save(Organization organization)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Organization>)));
			}

			Organization result = null;

			try
			{
				result = this.persistenceService.Update(organization);
				SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Update);
			}
			catch (KeyNotFoundException)
			{
				result = this.persistenceService.Insert(organization);
				SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Insert);
			}

			return result;
		}
	}
}