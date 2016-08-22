using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Roles;
using System.Linq.Expressions;
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
    /// Represents the local provider service
    /// </summary>
    public class LocalProviderService : IProviderRepositoryService
    {
        public IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find the specified provider
        /// </summary>
        public IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate, int offset, int? count, out int totalCount)
        {
            var rep = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();
            if (rep == null)
                throw new InvalidOperationException("Missing repository");
            return rep.Query(predicate, offset, count, out totalCount);
        }

        public Provider Get(IIdentity identity)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Gets a provider by key and version key.
		/// </summary>
		/// <param name="id">The key of the provider.</param>
		/// <param name="versionId">The version key of the provider.</param>
		/// <returns>Returns the provider.</returns>
        public Provider Get(Guid id, Guid versionId)
        {
			var providerRepository = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();

			if (providerRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Provider>)));
			}

			return providerRepository.Query(p => p.Key == id && p.VersionKey == versionId).FirstOrDefault();
        }

		/// <summary>
		/// Inserts a provider.
		/// </summary>
		/// <param name="provider">The provider to be inserted.</param>
		/// <returns>Returns the newly inserted provider.</returns>
        public Provider Insert(Provider provider)
        {
			var providerRepository = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();

			if (providerRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Provider>)));
			}

			return providerRepository.Insert(provider);
        }

		/// <summary>
		/// Obsoletes a provider.
		/// </summary>
		/// <param name="id">The id of the provider to be obsoleted.</param>
		/// <returns>Returns the obsoleted provider.</returns>
        public Provider Obsolete(Guid id)
        {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Saves a provider.
		/// </summary>
		/// <param name="provider">The provider to be saved.</param>
		/// <returns>Returns the updated provider.</returns>
        public Provider Save(Provider provider)
        {
			var providerRepository = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();

			if (providerRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Provider>)));
			}

			return providerRepository.Update(provider);
		}
    }
}
