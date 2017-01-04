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
	public abstract class BaseDataPersistenceService<TModel, TDomain> : IdentifiedPersistenceService<TModel, TDomain>
		where TModel : BaseEntityData, new()
		where TDomain : DbBaseData, new()
	{
        /// <summary>
        /// Perform the actual insert.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Insert (SQLiteConnectionWithLock context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;

			if (data.Key == Guid.Empty || !data.Key.HasValue)
				data.Key = domainObject.Key = Guid.NewGuid ();

            // Ensure created by exists
            data.CreatedBy?.EnsureExists(context);
			data.CreatedByKey = domainObject.CreatedByKey = domainObject.CreatedByKey == Guid.Empty ? base.CurrentUserUuid (context) : domainObject.CreatedByKey;
			domainObject.CreationTime = domainObject.CreationTime == DateTimeOffset.MinValue || domainObject.CreationTime == null ? DateTimeOffset.Now : domainObject.CreationTime;
			data.CreationTime = (DateTimeOffset)domainObject.CreationTime;

            if (!context.Table<TDomain>().Where(o => o.Uuid == domainObject.Uuid).Any())
                context.Insert(domainObject);
            else
                context.Update(domainObject);

			return data;
		}

        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Update (SQLiteConnectionWithLock context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;
            var existing = context.Table<TDomain>().Where(o=>o.Uuid == domainObject.Uuid).FirstOrDefault();
            if (existing == null)
                throw new KeyNotFoundException(data.Key.ToString());

            // Created by is the updated by
            domainObject.CopyObjectData(existing);
            domainObject.CreatedByUuid = existing.CreatedByUuid;
            data.CreatedBy?.EnsureExists(context);
			domainObject.UpdatedByKey = domainObject.CreatedByKey == Guid.Empty || domainObject.CreatedByKey == null ? base.CurrentUserUuid (context) : domainObject.CreatedByKey;
			domainObject.UpdatedTime = DateTime.Now;
			context.Update(domainObject);

            return data;
		}

        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Obsolete (SQLiteConnectionWithLock context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;
            data.ObsoletedBy?.EnsureExists(context);
            data.ObsoletedByKey = domainObject.ObsoletedByKey = data.ObsoletedBy?.Key ?? domainObject.ObsoletedByKey ?? base.CurrentUserUuid(context);
            domainObject.ObsoletionTime = domainObject.ObsoletionTime ?? DateTime.Now;
			data.ObsoletionTime = (DateTimeOffset)domainObject.ObsoletionTime;
			context.Update (domainObject);
			return data;
		}


    }
}

