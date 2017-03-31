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
 * Date: 2016-11-14
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
	/// Represents a material repository service.
	/// </summary>
	public class LocalMaterialService : IMaterialRepositoryService, IRepositoryService<Material>,
        IRepositoryService<ManufacturedMaterial>
	{

        /// <summary>
        /// Finds a specific manufactured material based on a specific query.
        /// </summary>
        /// <param name="expression">The query to use to find the manufactured materials.</param>
        /// <returns>Returns a list of manufactured materials which match the specific query.</returns>
        public IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression)
		{
			var pers = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();
			if (pers == null)
				throw new InvalidOperationException("Persistence service not found");
			return pers.Query(expression);
		}

		/// <summary>
		/// Find manufactured material with the specified controls
		/// </summary>
		public IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression, int offset, int? count, out int totalCount)
		{
			var pers = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();
			if (pers == null)
				throw new InvalidOperationException("Persistence service not found");
			return pers.Query(expression, offset, count, out totalCount, Guid.Empty);
		}

		/// <summary>
		/// Searches for a material using a given query.
		/// </summary>
		/// <param name="expression">The query to use to search for the material.</param>
		/// <returns>Returns a list of materials.</returns>
		public IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Material>)));
			}

			return persistenceService.Query(expression);
		}

		/// <summary>
		/// Searches for a material using a given query.
		/// </summary>
		/// <param name="expression">The query to use to search for the material.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of materials.</returns>
		public IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Material>)));
			}

			return persistenceService.Query(expression, offset, count, out totalCount, Guid.Empty);
		}
        

        /// <summary>
        /// Gets a manufactured material by id and version id.
        /// </summary>
        /// <param name="id">The id of the manufactured material to be retrieved.</param>
        /// <param name="versionId">The version id of the manufactured material to be retrieved.</param>
        /// <returns>Returns a manufactured material.</returns>
        public ManufacturedMaterial GetManufacturedMaterial(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<ManufacturedMaterial>)));
			}

			return persistenceService.Get(id);
		}

		/// <summary>
		/// Gets a material by id and version id.
		/// </summary>
		/// <param name="id">The id of the material to be retrieved.</param>
		/// <param name="versionId">The version id of the material to be retrieved.</param>
		/// <returns>Returns a material.</returns>
		public Material GetMaterial(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Material>)));
			}

			return persistenceService.Get(id);
		}

        /// <summary>
        /// Inserts a manufactured material.
        /// </summary>
        /// <param name="manufacturedMaterial">The manufactured material to be inserted.</param>
        /// <returns>Returns the inserted manufactured material.</returns>
        public ManufacturedMaterial Insert(ManufacturedMaterial manufacturedMaterial)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<ManufacturedMaterial>)));
			}

			var result = persistenceService.Insert(manufacturedMaterial);

			SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Insert);

			return result;
		}

		/// <summary>
		/// Inserts a material.
		/// </summary>
		/// <param name="material">The material to be inserted.</param>
		/// <returns>Returns the inserted material.</returns>
		public Material Insert(Material material)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Material>)));
			}

			var result = persistenceService.Insert(material);

			SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Insert);

			return result;
		}

        /// <summary>
        /// Obsoletes a manufactured material.
        /// </summary>
        /// <param name="key">The key of the manufactured material to be obsoleted.</param>
        /// <returns>Returns the obsoleted manufactured material.</returns>
        public ManufacturedMaterial ObsoleteManufacturedMaterial(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<ManufacturedMaterial>)));
			}

			var manufacturedMaterial = persistenceService.Get(key);

			if (manufacturedMaterial == null)
			{
				throw new ArgumentException("Unable to locate manufactured material");
			}

			var result = persistenceService.Obsolete(manufacturedMaterial);

			SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Obsolete);

			return result;
		}

		/// <summary>
		/// Obsoletes a material.
		/// </summary>
		/// <param name="key">The key of the material to be obsoleted.</param>
		/// <returns>Returns the obsoleted material.</returns>
		public Material ObsoleteMaterial(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Material>)));
			}

			var material = persistenceService.Get(key);

			if (material == null)
			{
				throw new ArgumentException("Unable to locate material");
			}

			var result = persistenceService.Obsolete(material);

			SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Obsolete);

			return result;
		}

        /// <summary>
        /// Saves or inserts a manufactured material.
        /// </summary>
        /// <param name="manufacturedMaterial">The manufactured material to be saved.</param>
        /// <returns>Returns the saved manufactured material.</returns>
        public ManufacturedMaterial Save(ManufacturedMaterial manufacturedMaterial)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<ManufacturedMaterial>)));
			}

			ManufacturedMaterial result = null;

			try
			{
				result = persistenceService.Update(manufacturedMaterial);

				SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Update);
			}
			catch (KeyNotFoundException)
			{
				result = persistenceService.Insert(manufacturedMaterial);

				SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Insert);
			}

			return result;
		}

		/// <summary>
		/// Saves or inserts the material.
		/// </summary>
		/// <param name="material">The material to be saved.</param>
		/// <returns>Returns the saved material.</returns>
		public Material Save(Material material)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate persistence service: {0}", nameof(IDataPersistenceService<Material>)));
			}

			Material result = null;

			try
			{
				result = persistenceService.Update(material);

				SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Update);
			}
			catch (KeyNotFoundException)
			{
				result = persistenceService.Insert(material);

				SynchronizationQueue.Outbound.Enqueue(result, DataOperationType.Insert);
			}

			return result;
		}

        /// <summary>
        /// Get the specified material
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Material IRepositoryService<Material>.Get(Guid key)
        {
            return this.GetMaterial(key, Guid.Empty);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        Material IRepositoryService<Material>.Obsolete(Guid key)
        {
            return this.ObsoleteMaterial(key);
        }

        /// <summary>
        /// Get manufactured material
        /// </summary>
        ManufacturedMaterial IRepositoryService<ManufacturedMaterial>.Get(Guid key)
        {
            return this.GetManufacturedMaterial(key, Guid.Empty);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        ManufacturedMaterial IRepositoryService<ManufacturedMaterial>.Obsolete(Guid key)
        {
            return this.ObsoleteManufacturedMaterial(key);
        }

        /// <summary>
        /// Find material
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<Material> IRepositoryService<Material>.Find(Expression<Func<Material, bool>> query)
        {
            return this.FindMaterial(query);
        }

        /// <summary>
        /// Find material
        /// </summary>
        IEnumerable<Material> IRepositoryService<Material>.Find(Expression<Func<Material, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.FindMaterial(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Find mmat
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<ManufacturedMaterial> IRepositoryService<ManufacturedMaterial>.Find(Expression<Func<ManufacturedMaterial, bool>> query)
        {
            return this.FindManufacturedMaterial(query);
        }

        /// <summary>
        /// Find mmat
        /// </summary>
        IEnumerable<ManufacturedMaterial> IRepositoryService<ManufacturedMaterial>.Find(Expression<Func<ManufacturedMaterial, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.FindManufacturedMaterial(query, offset, count, out totalResults);
        }

    }
}