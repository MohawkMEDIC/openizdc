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

        public Provider Get(Guid id, Guid versionId)
        {
            throw new NotImplementedException();
        }

        public Provider Insert(Provider provider)
        {
            throw new NotImplementedException();
        }

        public Provider Obsolete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Provider Save(Provider provider)
        {
            throw new NotImplementedException();
        }
    }
}
