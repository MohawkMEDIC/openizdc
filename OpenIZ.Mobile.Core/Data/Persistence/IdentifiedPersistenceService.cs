using System;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Data.Model.Security;
using SQLite;
using System.Linq.Expressions;
using System.Linq;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
	/// <summary>
	/// Security user persistence service.
	/// </summary>
	public class SecurityUserPersistenceService : LocalDataPersistenceService<SecurityUser>
	{
		#region implemented abstract members of LocalDataPersistenceService
		/// <summary>
		/// Maps the data to a model instance
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="dataInstance">Data instance.</param>
		public override SecurityUser ToModelInstance (object dataInstance, SQLiteConnection context)
		{
			return m_mapper.MapDomainInstance<DbSecurityUser, SecurityUser> (dataInstance as DbSecurityUser);
		}

		/// <summary>
		/// Froms the model instance.
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="modelInstance">Model instance.</param>
		/// <param name="context">Context.</param>
		public override object FromModelInstance (SecurityUser modelInstance, SQLiteConnection context)
		{
			return m_mapper.MapModelInstance<SecurityUser, DbSecurityUser> (modelInstance);

		}

		/// <summary>
		/// Performthe actual insert.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		internal override SecurityUser Insert (SQLiteConnection context, SecurityUser data)
		{
			var domainObject = this.FromModelInstance (data, context) as DbSecurityUser;
			if (domainObject.Uuid == null)
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
		internal override SecurityUser Update (SQLiteConnection context, SecurityUser data)
		{
			var domainObject = this.FromModelInstance (data, context) as DbSecurityUser;
			data.UpdatedByKey = domainObject.UpdatedByKey = domainObject.UpdatedByKey ?? base.CurrentUserUuid (context);
			domainObject.UpdatedTime = domainObject.UpdatedTime ?? DateTime.Now;
			data.UpdatedTime = (DateTimeOffset)domainObject.UpdatedTime;
			context.Update (domainObject);
			return data;
		}

		/// <summary>
		/// Performs the actual obsoletion
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		internal override SecurityUser Obsolete (SQLiteConnection context, SecurityUser data)
		{
			var domainObject = this.FromModelInstance (data, context) as DbSecurityUser;
			data.ObsoletedByKey = domainObject.ObsoletedByKey = domainObject.ObsoletedByKey ?? base.CurrentUserUuid (context);
			domainObject.ObsoletionTime = domainObject.ObsoletionTime ?? DateTime.Now;
			data.ObsoletionTime = (DateTimeOffset)domainObject.ObsoletionTime;
			context.Update (domainObject);
			return data;
		}

		/// <summary>
		/// Performs the actual query
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="query">Query.</param>
		internal override System.Collections.Generic.IEnumerable<SecurityUser> Query (SQLiteConnection context, Expression<Func<SecurityUser, bool>> query)
		{
			var domainQuery = m_mapper.MapModelExpression<SecurityUser, DbSecurityUser> (query);
			return context.Table<DbSecurityUser> ().Where (domainQuery).Select (o => this.ToModelInstance (o, context));
		}

		#endregion
	}
}

