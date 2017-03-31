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
 * User: fyfej
 * Date: 2017-1-16
 */
using OpenIZ.Core.Model.Entities;
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
	/// Represents an entity repository service.
	/// </summary>
	public class LocalEntityRepositoryService : IEntityRepositoryService, IRepositoryService<Entity>, IRepositoryService<EntityRelationship>
	{
		/// <summary>
		/// The internal reference to the <see cref="IDataPersistenceService{TData}"/> instance.
		/// </summary>
		private IDataPersistenceService<Entity> persistenceService;

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalEntityRepositoryService"/> class.
		/// </summary>
		public LocalEntityRepositoryService()
		{
			ApplicationContext.Current.Started += (o, e) => { this.persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>(); };
		}

        public IEnumerable<EntityRelationship> Find(Expression<Func<EntityRelationship, bool>> query)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<EntityRelationship> Find(Expression<Func<EntityRelationship, bool>> query, int offset, int? count, out int totalResults)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds a list of entities.
        /// </summary>
        /// <param name="query">The query to use to find the entities.</param>
        /// <returns>Returns a list of entities.</returns>
        public IEnumerable<Entity> Find(Expression<Func<Entity, bool>> query)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Entity>)));
			}

			return persistenceService.Query(query);
		}

		/// <summary>
		/// Finds a list of entities.
		/// </summary>
		/// <param name="query">The query to use to find the entities.</param>
		/// <param name="offSet">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of entities.</returns>
		public IEnumerable<Entity> Find(Expression<Func<Entity, bool>> query, int offSet, int? count, out int totalCount)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Entity>)));
			}

			return persistenceService.Query(query, offSet, count, out totalCount, Guid.Empty);
		}

        /// <summary>
        /// Get entity by key
        /// </summary>
        public Entity Get(Guid key)
        {
            return this.Get(key, Guid.Empty);
        }

        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <param name="key">The key of the entity to be retrieved.</param>
        /// <param name="versionKey">The version key of the entity to be retrieved.</param>
        /// <returns>Returns an entity.</returns>
        public Entity Get(Guid key, Guid versionKey)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Entity>)));
			}

			return persistenceService.Get(key);
		}

        /// <summary>
        /// Insert entity relationship
        /// </summary>
        public EntityRelationship Insert(EntityRelationship data)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();
            if (persistence == null)
                throw new InvalidOperationException("Unable to locate persistence service");
            return persistence.Insert(data);
        }

        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        /// <returns>Returns the inserted entity.</returns>
        public Entity Insert(Entity entity)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Entity>)));
			}

			var result = this.persistenceService.Insert(entity);

			SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Insert);

			return result;
		}

		/// <summary>
		/// Obsoletes an entity.
		/// </summary>
		/// <param name="key">The key of the entity to be obsoleted.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		public Entity Obsolete(Guid key)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Entity>)));
			}

			var result = this.persistenceService.Obsolete(new Entity { Key = key });

			SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Obsolete);

			return result;
		}
        
        /// <summary>
        /// Save or insert
        /// </summary>
        public EntityRelationship Save(EntityRelationship data)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();
            if (persistence == null)
                throw new InvalidOperationException("Unable to locate persistence service");
            try
            {
                int tr = 0;
                EntityRelationship old = null;
                if (data.Key != null)
                    old = persistence.Get(data.Key.Value);
                if (old == null)
                    old = persistence.Query(o => o.SourceEntityKey == data.SourceEntityKey && o.TargetEntityKey == data.TargetEntityKey, 0, 1, out tr, Guid.Empty).FirstOrDefault();
                if (old == null)
                    throw new KeyNotFoundException(data.Key?.ToString());
                return persistence.Update(data);
            }
            catch(KeyNotFoundException)
            {
                return persistence.Insert(data);
            }
        }

        /// <summary>
        /// Saves or inserts an entity.
        /// </summary>
        /// <param name="entity">The entity to be saved.</param>
        /// <returns>Returns the saved entity.</returns>
        public Entity Save(Entity entity)
		{
			if (this.persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Entity>)));
			}

			Entity result = null;

			try
			{
				result = persistenceService.Update(entity);
				SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Update);
			}
			catch (KeyNotFoundException)
			{
				result = persistenceService.Insert(entity);
				SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Insert);
			}

			return result;
		}

        /// <summary>
        /// Get entity relationship
        /// </summary>
        EntityRelationship IRepositoryService<EntityRelationship>.Get(Guid key)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();
            if (persistence == null)
                throw new InvalidOperationException("Unable to locate persistence service");
            return persistence.Get(key);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        EntityRelationship IRepositoryService<EntityRelationship>.Obsolete(Guid key)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();
            if (persistence == null)
                throw new InvalidOperationException("Unable to locate persistence service");
            return persistence.Obsolete(new EntityRelationship() { Key = key });
        }
    }
}
