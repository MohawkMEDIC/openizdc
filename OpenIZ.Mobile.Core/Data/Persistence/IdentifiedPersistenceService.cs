using System;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Data.Model.Security;
using SQLite;
using System.Linq.Expressions;
using System.Linq;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model;
using System.Text;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
	/// <summary>
	/// Generic persistence service which can persist between two simple types.
	/// </summary>
	public class IdentifiedPersistenceService<TModel, TDomain> : LocalDataPersistenceService<TModel> 
		where TModel : IdentifiedData, new()
		where TDomain : DbIdentified, new()
	{
		#region implemented abstract members of LocalDataPersistenceService
		/// <summary>
		/// Maps the data to a model instance
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="dataInstance">Data instance.</param>
		public override TModel ToModelInstance (object dataInstance, SQLiteConnection context)
		{
			return m_mapper.MapDomainInstance<TDomain, TModel> (dataInstance as TDomain);
		}

		/// <summary>
		/// Froms the model instance.
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="modelInstance">Model instance.</param>
		/// <param name="context">Context.</param>
		public override object FromModelInstance (TModel modelInstance, SQLiteConnection context)
		{
			return m_mapper.MapModelInstance<TModel, TDomain> (modelInstance);

		}

		/// <summary>
		/// Performthe actual insert.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		internal override TModel Insert (SQLiteConnection context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as TDomain;
			if (domainObject.Uuid == null)
				data.Key = domainObject.Key = Guid.NewGuid ();

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
			var domainObject = this.FromModelInstance (data, context) as DbSecurityUser;
			context.Update (domainObject);
			return data;
		}

		/// <summary>
		/// Performs the actual obsoletion
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		internal override TModel Obsolete (SQLiteConnection context, TModel data)
		{
			var domainObject = this.FromModelInstance (data, context) as DbSecurityUser;
			context.Update (domainObject);
			return data;
		}

		/// <summary>
		/// Performs the actual query
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="query">Query.</param>
		internal override System.Collections.Generic.IEnumerable<TModel> Query (SQLiteConnection context, Expression<Func<TModel, bool>> query)
		{
			var domainQuery = m_mapper.MapModelExpression<TModel, TDomain> (query);
			return context.Table<TDomain> ().Where (domainQuery).Select (o => this.ToModelInstance (o, context));
		}

		/// <summary>
		/// Performs the actual query
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="query">Query.</param>
		/// <param name="storedQueryName">Stored query name.</param>
		/// <param name="parms">Parms.</param>
		internal override System.Collections.Generic.IEnumerable<TModel> Query (SQLiteConnection context, string storedQueryName, System.Collections.Generic.IDictionary<string, object> parms)
		{

			// Build a query
			StringBuilder sb = new StringBuilder("SELECT * FROM ");
			sb.AppendFormat (storedQueryName);
			List<Object> vals = new List<Object> ();
			if (parms.Count > 0) {
				sb.Append (" WHERE ");
				foreach (var s in parms) {
					sb.AppendFormat (" {0} = ? AND ", s.Key);
					vals.Add (s.Value);
				}
				sb.Remove (sb.Length - 4, 4);
			}

			return context.Query<TDomain> (sb.ToString (), vals.ToArray ()).Select(o=>this.ToModelInstance(o, context));
		}
		#endregion
	}
}

