using System;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model;
using SQLite;

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
		internal override TModel Insert (SQLiteConnection context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;

			if (data.Key == Guid.Empty)
				data.Key = domainObject.Key = Guid.NewGuid ();

			data.CreatedByKey = domainObject.CreatedByKey = domainObject.CreatedByKey == Guid.Empty ? base.CurrentUserUuid (context) : domainObject.CreatedByKey;
			domainObject.CreationTime = domainObject.CreationTime ?? DateTime.Now;
			data.CreationTime = (DateTimeOffset)domainObject.CreationTime;
			context.Insert (domainObject);

			return data;
		}

		/// <summary>
		/// Perform the actual update.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		internal override TModel Update (SQLiteConnection context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;
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
		internal override TModel Obsolete (SQLiteConnection context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;
			data.ObsoletedByKey = domainObject.ObsoletedByKey = domainObject.ObsoletedByKey ?? base.CurrentUserUuid (context);
			domainObject.ObsoletionTime = domainObject.ObsoletionTime ?? DateTime.Now;
			data.ObsoletionTime = (DateTimeOffset)domainObject.ObsoletionTime;
			context.Update (domainObject);
			return data;
		}

	}
}

