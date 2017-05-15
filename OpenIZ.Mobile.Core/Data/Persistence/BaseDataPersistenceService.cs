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
 * User: justi
 * Date: 2017-2-4
 */
using System;
using System.Linq;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model;
using SQLite.Net;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Services;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Base data persistence service
    /// </summary>
    public abstract class BaseDataPersistenceService<TModel, TDomain> : BaseDataPersistenceService<TModel, TDomain, TDomain>
        where TModel : BaseEntityData, new()
        where TDomain : DbBaseData, new()
    { }

    /// <summary>
    /// Base data persistence service
    /// </summary>
    public abstract class BaseDataPersistenceService<TModel, TDomain, TQueryResult> : IdentifiedPersistenceService<TModel, TDomain, TQueryResult>
		where TModel : BaseEntityData, new()
		where TDomain : DbBaseData, new()
        where TQueryResult : DbIdentified
	{
        /// <summary>
        /// Perform the actual insert.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        protected override TModel InsertInternal (LocalDataContext context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;

			if (data.Key == Guid.Empty || !data.Key.HasValue)
				data.Key = domainObject.Key = Guid.NewGuid ();

            // Ensure created by exists
            if(data.CreatedBy != null) data.CreatedBy = data.CreatedBy?.EnsureExists(context);
			data.CreatedByKey = domainObject.CreatedByKey = domainObject.CreatedByKey == Guid.Empty ? base.CurrentUserUuid (context) : domainObject.CreatedByKey;
			domainObject.CreationTime = domainObject.CreationTime == DateTimeOffset.MinValue || domainObject.CreationTime == null ? DateTimeOffset.Now : domainObject.CreationTime;
			data.CreationTime = (DateTimeOffset)domainObject.CreationTime;

            if (!context.Connection.Table<TDomain>().Where(o => o.Uuid == domainObject.Uuid).Any())
				context.Connection.Insert(domainObject);
			else
                context.Connection.Update(domainObject);

			return data;
		}

        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        protected override TModel UpdateInternal (LocalDataContext context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;
            var existing = context.Connection.Table<TDomain>().Where(o=>o.Uuid == domainObject.Uuid).FirstOrDefault();
            if (existing == null)
                throw new KeyNotFoundException(data.Key.ToString());

            // Created by is the updated by
            existing.CopyObjectData(domainObject);
            domainObject = existing;
            domainObject.CreatedByUuid = existing.CreatedByUuid;
            if(data.CreatedBy != null) data.CreatedBy = data.CreatedBy?.EnsureExists(context);
			domainObject.UpdatedByKey = domainObject.CreatedByKey == Guid.Empty || domainObject.CreatedByKey == null ? base.CurrentUserUuid (context) : domainObject.CreatedByKey;
			domainObject.UpdatedTime = DateTime.Now;
			context.Connection.Update(domainObject);

            return data;
		}

        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        protected override TModel ObsoleteInternal (LocalDataContext context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;
            if(data.ObsoletedBy != null) data.ObsoletedBy = data.ObsoletedBy?.EnsureExists(context);
            data.ObsoletedByKey = domainObject.ObsoletedByKey = data.ObsoletedBy?.Key ?? domainObject.ObsoletedByKey ?? base.CurrentUserUuid(context);
            domainObject.ObsoletionTime = domainObject.ObsoletionTime ?? DateTime.Now;
			data.ObsoletionTime = (DateTimeOffset)domainObject.ObsoletionTime;
			context.Connection.Update (domainObject);
			return data;
		}


    }
}

