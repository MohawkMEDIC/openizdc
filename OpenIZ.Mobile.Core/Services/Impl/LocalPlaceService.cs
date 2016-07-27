using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
    /// Local place service
    /// </summary>
    public class LocalPlaceService : IPlaceRepositoryService
    {
        /// <summary>
        /// Find the specified place
        /// </summary>
        public IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Query(predicate);
        }

        /// <summary>
        /// Searches the place service for the specified place matching the 
        /// given predicate
        /// </summary>
        /// <param name="predicate">The predicate function to search by.</param>
        /// <returns>Returns a list of places.</returns>
        public IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate, int offset, int? count, out int totalCount)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Query(predicate, offset, count, out totalCount);
        }

        /// <summary>
        /// Gets the specified place
        /// </summary>
        public Place Get(Guid id, Guid versionId)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Get(id);
        }

        /// <summary>
        /// Inserts the specified place
        /// </summary>
        public Place Insert(Place plc)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Insert(plc);
        }

        /// <summary>
        /// Obsoletes the specified data
        /// </summary>
        public Place Obsolete(Guid id)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Obsolete(new Place() { Key = id });

        }

        /// <summary>
        /// Inserts or updates the specified place
        /// </summary>
        public Place Save(Place plc)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            try
            {
                return persistenceService.Update(plc);
            }
            catch (KeyNotFoundException)
            {
                return persistenceService.Insert(plc);
            }
        }
    }
}
