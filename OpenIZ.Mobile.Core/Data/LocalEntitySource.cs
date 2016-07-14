using System;
using System.Linq;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Interfaces;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Data
{
	/// <summary>
	/// Entity source which loads local objects
	/// </summary>
	public class LocalEntitySource : IEntitySourceProvider
	{


        #region IEntitySourceProvider implementation
        /// <summary>
        /// Get the persistence service source
        /// </summary>
        public TObject Get<TObject>(Guid? key) where TObject : IdentifiedData
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            if (persistenceService != null && key.HasValue)
                return persistenceService.Get(key.Value);
			return default(TObject);
		}

		/// <summary>
		/// Get the specified version
		/// </summary>
		public TObject Get<TObject>(Guid? key, Guid? versionKey) where TObject : IdentifiedData, IVersionedEntity
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            if (persistenceService != null && key.HasValue)
                return persistenceService.Query(o => o.Key == key).FirstOrDefault();
            else if(persistenceService != null && key.HasValue && versionKey.HasValue)
                return persistenceService.Query(o => o.Key == key && o.VersionKey == versionKey).FirstOrDefault ();
			return default(TObject);
		}

        /// <summary>
        /// Get versioned relationships for the object
        /// </summary>
        public List<TObject> GetRelations<TObject>(Guid? sourceKey, decimal? sourceVersionSequence, List<TObject> currentInstance) where TObject : IdentifiedData, IVersionedAssociation
        {
            return this.Query<TObject>(o => o.SourceEntityKey == sourceKey).ToList();
        }

        /// <summary>
        /// Query the specified object
        /// </summary>
        public IEnumerable<TObject> Query<TObject>(Expression<Func<TObject, bool>> query) where TObject : IdentifiedData
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
			if(persistenceService != null)
				return persistenceService.Query(query);
			return new List<TObject>();
		}

        #endregion

    }
}

