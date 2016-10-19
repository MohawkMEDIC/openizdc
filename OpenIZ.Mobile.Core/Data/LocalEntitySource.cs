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
 * Date: 2016-6-14
 */
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
        public TObject Get<TObject>(Guid? key) where TObject : IdentifiedData, new()
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            if (persistenceService != null && key.HasValue)
                return persistenceService.Get(key.Value);
			return default(TObject);
		}

		/// <summary>
		/// Get the specified version
		/// </summary>
		public TObject Get<TObject>(Guid? key, Guid? versionKey) where TObject : IdentifiedData, IVersionedEntity, new()
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
        public List<TObject> GetRelations<TObject>(Guid? sourceKey, decimal? sourceVersionSequence) where TObject : IdentifiedData, IVersionedAssociation, new()
        {
            return this.Query<TObject>(o => o.SourceEntityKey == sourceKey).ToList();
        }

        /// <summary>
        /// Get versioned relationships for the object
        /// </summary>
        public List<TObject> GetRelations<TObject>(Guid? sourceKey) where TObject : IdentifiedData, ISimpleAssociation, new()
        {
            return this.Query<TObject>(o => o.SourceEntityKey == sourceKey).ToList();
        }

        /// <summary>
        /// Query the specified object
        /// </summary>
        public IEnumerable<TObject> Query<TObject>(Expression<Func<TObject, bool>> query) where TObject : IdentifiedData, new()
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
			if(persistenceService != null)
				return persistenceService.Query(query);
			return new List<TObject>();
		}

        #endregion

    }
}

