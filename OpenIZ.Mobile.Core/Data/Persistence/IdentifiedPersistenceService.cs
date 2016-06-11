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
using OpenIZ.Mobile.Core.Services;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
	/// <summary>
	/// Generic persistence service which can persist between two simple types.
	/// </summary>
	public abstract class IdentifiedPersistenceService<TModel, TDomain> : LocalPersistenceServiceBase<TModel> 
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

			if (domainObject.Uuid == null || domainObject.Key == Guid.Empty)
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
			var domainObject = this.FromModelInstance (data, context) as TDomain;
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
			var domainObject = this.FromModelInstance (data, context) as TDomain;
			context.Delete (domainObject);
			return data;
		}

		/// <summary>
		/// Performs the actual query
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="query">Query.</param>
		internal override System.Collections.Generic.IEnumerable<TModel> Query (SQLiteConnection context, Expression<Func<TModel, bool>> query, int offset = 0, int count = -1)
		{
			var domainQuery = m_mapper.MapModelExpression<TModel, TDomain> (query);
			var retVal = context.Table<TDomain> ().Where (domainQuery).Skip (offset);
			if (count >= 0)
				retVal = retVal.Take (count);
			return retVal.ToList().Select(o=>this.ToModelInstance(o, context)).ToList();
		}

		/// <summary>
		/// Performs the actual query
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="query">Query.</param>
		/// <param name="storedQueryName">Stored query name.</param>
		/// <param name="parms">Parms.</param>
		internal override System.Collections.Generic.IEnumerable<TModel> Query (SQLiteConnection context, string storedQueryName, System.Collections.Generic.IDictionary<string, object> parms, int offset = 0, int count = -1)
		{

            // Build a query
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM {0} WHERE uuid IN (SELECT uuid FROM ", context.GetMapping<TDomain>().TableName);
			sb.AppendFormat (storedQueryName);
			List<Object> vals = new List<Object> ();
			if (parms.Count > 0) {
				sb.Append (" WHERE ");
				foreach (var s in parms) {
					sb.AppendFormat (" {0} = ? AND ", s.Key.Replace(".","_"));
					vals.Add (s.Value);
				}
				sb.Remove (sb.Length - 4, 4);
			}
            sb.Append(");");
			var retVal = context.Query<TDomain> (sb.ToString (), vals.ToArray ()).Skip (offset);
			if (count >= 0)
				retVal = retVal.Take (count);
			return retVal.ToList().Select(o=>this.ToModelInstance(o, context));
		}


        /// <summary>
        /// Update associated version items
        /// </summary>
        protected void UpdateAssociatedItems<TAssociation, TModelEx>(List<TAssociation> existing, List<TAssociation> storage, Guid sourceKey, SQLiteConnection dataContext)
            where TAssociation : Association<TModelEx>, new()
            where TModelEx : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAssociation>>() as LocalPersistenceServiceBase<TAssociation>;
            if (persistenceService == null)
            {
                this.m_tracer.TraceInfo("Missing persister for type {0}", typeof(TAssociation).Name);
                return;
            }
            // Ensure the source key is set
            foreach (var itm in storage)
                if (itm.SourceEntityKey == Guid.Empty)
                    itm.SourceEntityKey = sourceKey;

            // Remove old
            var obsoleteRecords = existing.Where(o => !storage.Exists(ecn => ecn.Key == o.Key));
            foreach (var del in obsoleteRecords)
                persistenceService.Obsolete(dataContext, del);

            // Update those that need it
            var updateRecords = storage.Where(o => existing.Any(ecn => ecn.Key == o.Key && o.Key != Guid.Empty && o != ecn));
            foreach (var upd in updateRecords)
                persistenceService.Update(dataContext, upd);

            // Insert those that do not exist
            var insertRecords = storage.Where(o => !existing.Any(ecn => ecn.Key == o.Key));
            foreach (var ins in insertRecords)
            {
                ins.SourceEntityKey = sourceKey;
                persistenceService.Insert(dataContext, ins);
            }


        }
        #endregion
    }
}

