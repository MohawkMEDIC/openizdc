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
 * Date: 2016-8-18
 */

using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Synchronization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Services.Impl
{
	/// <summary>
	/// Represents the local provider service
	/// </summary>
	public class LocalProviderService : IProviderRepositoryService
	{
		/// <summary>
		/// The internal reference to the <see cref="IDataPersistenceService{TData}"/> instance.
		/// </summary>
		private IDataPersistenceService<Provider> persistenceService;

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalProviderService"/> class.
		/// </summary>
		public LocalProviderService()
		{
			ApplicationContext.Current.Started += (o, e) => { this.persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>(); };
		}

		/// <summary>
		/// Searches for a provider using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the provider.</param>
		/// <returns>Returns a list of providers who match the specified predicate.</returns>
		public IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate)
		{
			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Provider>)));
			}

			int totalCount = 0;
			return this.Find(predicate, 0, null, out totalCount);
		}

		/// <summary>
		/// Searches for a provider using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the provider.</param>
		/// <param name="count">The count of the providers to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of providers who match the specified predicate.</returns>
		public IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate, int offset, int? count, out int totalCount)
		{
			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Provider>)));
			}

			return persistenceService.Query(predicate, offset, count, out totalCount);
		}

		/// <summary>
		/// Get the provider based off the user identity
		/// </summary>
		public Provider Get(IIdentity identity)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider.</param>
		/// <param name="versionId">The version id of the provider.</param>
		/// <returns>Returns the specified provider.</returns>
		public Provider Get(Guid id, Guid versionId)
		{
			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Provider>)));
			}

			return persistenceService.Query(p => p.Key == id && p.VersionKey == versionId).FirstOrDefault();
		}

		/// <summary>
		/// Inserts the specified provider.
		/// </summary>
		/// <param name="provider">The provider to insert.</param>
		/// <returns>Returns the inserted provider.</returns>
		public Provider Insert(Provider provider)
		{
			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Provider>)));
			}

			var result = persistenceService.Insert(provider);

			SynchronizationQueue.Outbound.Enqueue(result, Synchronization.Model.DataOperationType.Insert);

			return result;
		}

		/// <summary>
		/// Obsoletes the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider to obsolete.</param>
		/// <returns>Returns the obsoleted provider.</returns>
		public Provider Obsolete(Guid id)
		{
			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Provider>)));
			}

			var result = persistenceService.Obsolete(new Provider { Key = id });

			SynchronizationQueue.Outbound.Enqueue(result, Synchronization.Model.DataOperationType.Obsolete);

			return result;
		}

		/// <summary>
		/// Saves the specified provider.
		/// </summary>
		/// <param name="provider">The provider to save.</param>
		/// <returns>Returns the saved provider.</returns>
		public Provider Save(Provider provider)
		{
			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Provider>)));
			}

			var result = persistenceService.Update(provider);

			SynchronizationQueue.Outbound.Enqueue(result, Synchronization.Model.DataOperationType.Update);

			return result;
		}
	}
}