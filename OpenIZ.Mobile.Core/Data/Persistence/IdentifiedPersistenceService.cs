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
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Data.Model.Security;
using SQLite.Net;
using System.Linq.Expressions;
using System.Linq;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model;
using System.Text;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Interfaces;
using System.Collections;
using OpenIZ.Core.Services;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
	/// <summary>
	/// Generic persistence service which can persist between two simple types.
	/// </summary>
	public abstract class IdentifiedPersistenceService<TModel, TDomain> : LocalPersistenceServiceBase<TModel> 
		where TModel : IdentifiedData, new()
		where TDomain : DbIdentified, new()
	{

        // Caching service
        private IDataCachingService m_cache = ApplicationContext.Current.GetService<IDataCachingService>();

		#region implemented abstract members of LocalDataPersistenceService
		/// <summary>
		/// Maps the data to a model instance
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="dataInstance">Data instance.</param>
		public override TModel ToModelInstance (object dataInstance, SQLiteConnectionWithLock context)
		{
			var retVal = m_mapper.MapDomainInstance<TDomain, TModel> (dataInstance as TDomain);
            retVal.LoadAssociations(context);
            return retVal;
        }

        /// <summary>
        /// Froms the model instance.
        /// </summary>
        /// <returns>The model instance.</returns>
        /// <param name="modelInstance">Model instance.</param>
        /// <param name="context">Context.</param>
        public override object FromModelInstance (TModel modelInstance, SQLiteConnectionWithLock context)
		{
			return m_mapper.MapModelInstance<TModel, TDomain> (modelInstance);

		}

		/// <summary>
		/// Performthe actual insert.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		public override TModel Insert (SQLiteConnectionWithLock context, TModel data)
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
        public override TModel Update (SQLiteConnectionWithLock context, TModel data)
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
        public override TModel Obsolete (SQLiteConnectionWithLock context, TModel data)
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
        public override System.Collections.Generic.IEnumerable<TModel> Query (SQLiteConnectionWithLock context, Expression<Func<TModel, bool>> query, int offset, int count, out int totalResults)
		{
			var domainQuery = m_mapper.MapModelExpression<TModel, TDomain> (query);
			var retVal = context.Table<TDomain> ().Where (domainQuery);
            // Total count
            totalResults = retVal.Count();
            // Skip
            retVal = retVal.Skip(offset);
            if (count > 0)
				retVal = retVal.Take (count);

            var domainList = retVal.ToList();
            
            return domainList.Select(o=>this.CacheConvert(o, context)).ToList();
		}

        /// <summary>
        /// Try conversion from cache otherwise map
        /// </summary>
        private TModel CacheConvert(TDomain o, SQLiteConnectionWithLock context)
        {
            var cacheItem = this.m_cache.GetCacheItem<TModel>(new Guid(o.Uuid));
            if (cacheItem == null)
                cacheItem = this.ToModelInstance(o, context);
            return cacheItem;
        }

        /// <summary>
        /// Performs the actual query
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="query">Query.</param>
        /// <param name="storedQueryName">Stored query name.</param>
        /// <param name="parms">Parms.</param>
        public override IEnumerable<TModel> Query(SQLiteConnectionWithLock context, string storedQueryName, IDictionary<string, object> parms, int offset, int count, out int totalResults)
		{

            // Build a query
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT DISTINCT * FROM {0} WHERE uuid IN (", context.GetMapping<TDomain>().TableName);
			List<Object> vals = new List<Object> ();
			if (parms.Count > 0) {
				foreach (var s in parms) {

                    sb.AppendFormat("SELECT uuid FROM {0} WHERE ", storedQueryName);

                    object rValue = s.Value;
                    if (!(rValue is IList))
                        rValue = new List<Object>() { rValue };

                    string key = s.Key.Replace(".", "_");
                    // Are there guards?
                    while (key.Contains("["))
                    {
                        var guardRoot = key.Substring(0, key.IndexOf("["));
                        var guardValue = key.Substring(key.IndexOf("[") + 1, key.IndexOf("]") - key.IndexOf("[") - 1);
                        sb.AppendFormat(" {0}_guard = ? AND ", guardRoot);
                        vals.Add(guardValue);
                        key = guardRoot + key.Substring(key.IndexOf("]") + 1);
                    }

                    // Value is string
                    foreach (var itm in rValue as IList)
                    {
                        var value = itm;
                        
                        if (value is String)
                        {
                            var sValue = itm as String;
                            switch (sValue[0])
                            {
                                case '<':
                                    if (sValue[1] == '=')
                                    {
                                        sb.AppendFormat(" {0} <= ? AND ", key);
                                        value = sValue.Substring(2);
                                    }
                                    else
                                    {
                                        sb.AppendFormat(" {0} < ? AND ", key);
                                        value = sValue.Substring(1);
                                    }
                                    break;
                                case '>':
                                    if (sValue[1] == '=')
                                    {
                                        sb.AppendFormat(" {0} >= ? AND ", key);
                                        value = sValue.Substring(2);
                                    }
                                    else
                                    {
                                        sb.AppendFormat(" {0} > ? AND ", key);
                                        value = sValue.Substring(1);
                                    }
                                    break;
                                case '!':
                                    sb.AppendFormat(" {0} <> ? AND ", key);
                                    value = sValue.Substring(1);
                                    break;
                                case '~':
                                    sb.AppendFormat(" {0} LIKE '%' || ? || '%' AND ", key);
                                    value = sValue.Substring(1);
                                    break;
                                default:
                                    sb.AppendFormat(" {0} = ? AND ", key);
                                    break;
                            }
                        }
                        else
                            sb.AppendFormat(" {0} = ? AND ", key);

                        // Value correction
                        DateTime tdateTime = default(DateTime);
                        if (value is Guid)
                            vals.Add(((Guid)value).ToByteArray());
                        else if (DateTime.TryParse(value.ToString(), out tdateTime))
                            vals.Add(tdateTime);
                        else
                            vals.Add(value);
                    }
                    sb.Remove(sb.Length - 4, 4);

                    sb.Append(" INTERSECT ");
                }
            }
            sb.Remove(sb.Length - 10, 10);
            sb.Append(") ");

            if (count > 0)
                sb.AppendFormat("LIMIT {0} ", count);
            if (offset > 0)
                sb.AppendFormat("OFFSET {0}", count);

            sb.Append(";");

            // Log
#if DEBUG
            this.m_tracer.TraceVerbose("EXECUTE: {0}", sb);
            foreach (var v in vals)
                this.m_tracer.TraceVerbose(" --> {0}", v is byte[] ? BitConverter.ToString(v as Byte[]).Replace("-","") : v);
#endif


            var retVal = context.Query<TDomain>(sb.ToString(), vals.ToArray());
            totalResults = retVal.Count;
			return retVal.Select(o=>this.ToModelInstance(o, context)).ToList();
		}


        /// <summary>
        /// Update associated version items
        /// </summary>
        protected void UpdateAssociatedItems<TAssociation, TModelEx>(List<TAssociation> existing, List<TAssociation> storage, Guid? sourceKey, SQLiteConnectionWithLock dataContext)
            where TAssociation : IdentifiedData, ISimpleAssociation, new()
            where TModelEx : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAssociation>>() as LocalPersistenceServiceBase<TAssociation>;
            if (persistenceService == null || !sourceKey.HasValue)
            {
                this.m_tracer.TraceInfo("Missing persister for type {0}", typeof(TAssociation).Name);
                return;
            }
            // Ensure the source key is set
            foreach (var itm in storage)
                if (itm.SourceEntityKey == Guid.Empty || !itm.SourceEntityKey.HasValue)
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
                if (ins.IsEmpty()) continue;
                ins.SourceEntityKey = sourceKey;
                persistenceService.Insert(dataContext, ins);
            }


        }
        #endregion
    }
}

