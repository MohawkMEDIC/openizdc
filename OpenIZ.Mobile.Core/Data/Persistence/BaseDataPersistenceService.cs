using System;
using System.Linq;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model;
using SQLite;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Services;

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
        /// Performthe actual insert.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Insert (SQLiteConnection context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;

			if (data.Key == Guid.Empty || !data.Key.HasValue)
				data.Key = domainObject.Key = Guid.NewGuid ();

            // Ensure created by exists
            data.CreatedBy?.EnsureExists(context);
			data.CreatedByKey = domainObject.CreatedByKey = domainObject.CreatedByKey == Guid.Empty ? base.CurrentUserUuid (context) : domainObject.CreatedByKey;
			domainObject.CreationTime = domainObject.CreationTime == DateTime.MinValue || domainObject.CreationTime == null ? DateTime.Now : domainObject.CreationTime;
			data.CreationTime = (DateTimeOffset)domainObject.CreationTime;
			context.Insert (domainObject);

			return data;
		}

        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Update (SQLiteConnection context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;

            data.CreatedBy?.EnsureExists(context);
			domainObject.UpdatedByKey = domainObject.CreatedByKey == Guid.Empty ? base.CurrentUserUuid (context) : domainObject.CreatedByKey;
			domainObject.UpdatedTime = DateTime.Now;
			context.Update(domainObject);

			return data;
		}

        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Obsolete (SQLiteConnection context, TModel data)
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

