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
using System.Diagnostics;
using System.Reflection;
using SQLite.Net.Attributes;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Core.Data.QueryBuilder;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Mobile.Core.Data.Persistence
{

    /// <summary>
    /// Generic persistence service
    /// </summary>
    public abstract class IdentifiedPersistenceService<TModel, TDomain> : IdentifiedPersistenceService<TModel, TDomain, TDomain>
        where TModel : IdentifiedData, new()
        where TDomain : DbIdentified, new()
    {
    }

    /// <summary>
    /// Generic persistence service which can persist between two simple types.
    /// </summary>
    public abstract class IdentifiedPersistenceService<TModel, TDomain, TQueryResult> : LocalPersistenceServiceBase<TModel>
    where TModel : IdentifiedData, new()
    where TDomain : DbIdentified, new()
    where TQueryResult : DbIdentified
    {

        // Required properties
        private Dictionary<Type, PropertyInfo[]> m_requiredProperties = new Dictionary<Type, PropertyInfo[]>();

        // Query persistence
        private IQueryPersistenceService m_queryPersistence = ApplicationContext.Current.GetService<IQueryPersistenceService>();

        #region implemented abstract members of LocalDataPersistenceService
        /// <summary>
        /// Maps the data to a model instance
        /// </summary>
        /// <returns>The model instance.</returns>
        /// <param name="dataInstance">Data instance.</param>
        public override TModel ToModelInstance(object dataInstance, LocalDataContext context)
        {
            var retVal = m_mapper.MapDomainInstance<TDomain, TModel>(dataInstance as TDomain);

            retVal.LoadAssociations(context);
            return retVal;
        }

        /// <summary>
        /// Froms the model instance.
        /// </summary>
        /// <returns>The model instance.</returns>
        /// <param name="modelInstance">Model instance.</param>
        /// <param name="context">Context.</param>
        public override object FromModelInstance(TModel modelInstance, LocalDataContext context)
        {
            return m_mapper.MapModelInstance<TModel, TDomain>(modelInstance);

        }

        /// <summary>
        /// Performthe actual insert.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        protected override TModel InsertInternal(LocalDataContext context, TModel data)
        {
            var domainObject = this.FromModelInstance(data, context) as TDomain;

            if (domainObject.Uuid == null || domainObject.Key == Guid.Empty)
                data.Key = domainObject.Key = Guid.NewGuid();

#if DB_DEBUG
            PropertyInfo[] properties = null;
            if(!this.m_requiredProperties.TryGetValue(typeof(TDomain), out properties))
            {
                properties = typeof(TDomain).GetRuntimeProperties().Where(o => o.GetCustomAttribute<NotNullAttribute>() != null).ToArray();
                lock (this.m_requiredProperties)
                    if (this.m_requiredProperties.ContainsKey(typeof(TDomain)))
                        this.m_requiredProperties.Add(typeof(TDomain), properties);
            }
            foreach (var itm in properties)
                if (itm.GetValue(domainObject) == null)
                    throw new ArgumentNullException(itm.Name, "Requires a value");
#endif

            // Does this already exist?

            //if(context.Connection.Get<TDomain>(domainObject.Uuid) == null)
            try
            {
                context.Connection.Insert(domainObject);
            }
            catch // doubt this will even work
            {
                context.Connection.Update(domainObject);
            }

            return data;
        }

        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        protected override TModel UpdateInternal(LocalDataContext context, TModel data)
        {
            var domainObject = this.FromModelInstance(data, context) as TDomain;
            context.Connection.Update(domainObject);
            return data;
        }

        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        protected override TModel ObsoleteInternal(LocalDataContext context, TModel data)
        {
            var domainObject = this.FromModelInstance(data, context) as TDomain;
            context.Connection.Delete(domainObject);
            return data;
        }

        /// <summary>
        /// Performs the actual query
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="query">Query.</param>
        protected override System.Collections.Generic.IEnumerable<TModel> QueryInternal(LocalDataContext context, Expression<Func<TModel, bool>> query, int offset, int count, out int totalResults, Guid queryId, bool countResults)
        {

            // Query has been registered?
            if (queryId != Guid.Empty && this.m_queryPersistence?.IsRegistered(queryId) == true)
            {
                totalResults = (int)this.m_queryPersistence.QueryResultTotalQuantity(queryId);
                return this.m_queryPersistence.GetQueryResults(queryId, offset, count).Select(p => this.Get(context, p)).ToList();
            }

            SqlStatement queryStatement = null;
            queryStatement = new SqlStatement<TDomain>().SelectFrom();
            var expression = m_mapper.MapModelExpression<TModel, TDomain>(query, false);
            if (expression != null)
            {
                if (typeof(TQueryResult) != typeof(TDomain))
                {
                    var tableMap = OpenIZ.Core.Data.QueryBuilder.TableMapping.Get(typeof(TDomain));
                    var fkStack = new Stack<OpenIZ.Core.Data.QueryBuilder.TableMapping>();
                    fkStack.Push(tableMap);
                    var scopedTables = new HashSet<Object>();
                    // Always join tables?
                    do
                    {
                        var dt = fkStack.Pop();
                        foreach (var jt in dt.Columns.Where(o => o.IsAlwaysJoin))
                        {
                            var fkTbl = OpenIZ.Core.Data.QueryBuilder.TableMapping.Get(jt.ForeignKey.Table);
                            var fkAtt = fkTbl.GetColumn(jt.ForeignKey.Column);
                            queryStatement.InnerJoin(dt.OrmType, fkTbl.OrmType);
                            if (!scopedTables.Contains(fkTbl))
                                fkStack.Push(fkTbl);
                            scopedTables.Add(fkAtt.Table);
                        }
                    } while (fkStack.Count > 0);


                }

                //queryStatement = new SqlStatement<TDomain>().SelectFrom()
                queryStatement = queryStatement.Where<TDomain>(expression).Build();
                m_tracer.TraceVerbose("Built Query: {0}", queryStatement.SQL);
            }
            else
            {
                queryStatement = m_builder.CreateQuery(query).Build();
                m_tracer.TraceVerbose("Built Query: {0}", queryStatement.SQL);
            }

            // Is this a cached query?
            var retVal = context.CacheQuery(queryStatement)?.OfType<TModel>();
            if (retVal != null && !countResults)
            {
                totalResults = 0;
                return retVal;
            }

            // Preare SQLite Args
            var args = queryStatement.Arguments.Select(o =>
            {
                if (o is Guid || o is Guid?)
                    return ((Guid)o).ToByteArray();
                else if (o is DateTime || o is DateTime?)
                    return ((DateTime)o).Ticks;
                else if (o is DateTimeOffset || o is DateTimeOffset?)
                    return ((DateTimeOffset)o).Ticks;
                else if (o is bool || o is bool?)
                    return ((bool)o) ? 1 : 0;
                else
                    return o;
            }).ToArray();


            if (queryId != Guid.Empty)
                this.m_queryPersistence?.RegisterQuerySet(queryId, context.Connection.Query<TQueryResult>(queryStatement.Build().SQL, args).Select(o => o.Key), query);
            // Total count
            if (countResults && queryId == Guid.Empty)
                totalResults = context.Connection.ExecuteScalar<Int32>("SELECT COUNT(*) FROM (" + queryStatement.SQL + ")", args);
            else if (countResults)
            {
                totalResults = (int)this.m_queryPersistence.QueryResultTotalQuantity(queryId);
                if (totalResults == 0)
                    return new List<TModel>();
            }
            else
                totalResults = 0;

            if (count > 0)
                queryStatement.Limit(count);
            if (offset > 0)
            {
                if (count == 0)
                    queryStatement.Limit(100).Offset(offset);
                else
                    queryStatement.Offset(offset);
            }

            // Exec query
            var domainList = context.Connection.Query<TQueryResult>(queryStatement.Build().SQL, args).ToList();
            var modelList = domainList.Select(o => this.CacheConvert(o, context)).ToList();
            context.AddQuery(queryStatement, modelList);

            //foreach (var i in modelList)
            //    context.AddCacheCommit(i);
            return modelList;
        }

        /// <summary>
        /// Try conversion from cache otherwise map
        /// </summary>
        protected virtual TModel CacheConvert(DbIdentified o, LocalDataContext context)
        {
            if (o == null) return null;
            var cacheItem = ApplicationContext.Current.GetService<IDataCachingService>()?.GetCacheItem<TModel>(new Guid(o.Uuid));
            if (cacheItem == null)
            {
                cacheItem = this.ToModelInstance(o, context);
                if (!context.Connection.IsInTransaction)
                    ApplicationContext.Current.GetService<IDataCachingService>()?.Add(cacheItem);
            }
            else if (cacheItem.LoadState <= context.DelayLoadMode)
                cacheItem.LoadAssociations(context);

            return cacheItem;
        }

        /// <summary>
        /// Performs the actual query
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="query">Query.</param>
        /// <param name="storedQueryName">Stored query name.</param>
        /// <param name="parms">Parms.</param>
        protected override IEnumerable<TModel> QueryInternal(LocalDataContext context, string storedQueryName, IDictionary<string, object> parms, int offset, int count, out int totalResults, Guid queryId, bool countResults)
        {


            // Query has been registered?
            if (queryId != Guid.Empty && this.m_queryPersistence.IsRegistered(queryId))
            {
                totalResults = (int)this.m_queryPersistence.QueryResultTotalQuantity(queryId);
                return this.m_queryPersistence.GetQueryResults(queryId, offset, count).Select(p => this.Get(context, p));
            }

            var useIntersect = parms.Keys.Count(o => o.StartsWith("relationship")) > 1 ||
                parms.Keys.Count(o => o.StartsWith("participation")) > 1;

            // Build a query
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("SELECT DISTINCT * FROM {0} WHERE uuid IN (", context.Connection.GetMapping<TDomain>().TableName);
            List<Object> vals = new List<Object>();
            if (parms.Count > 0)
            {
                if (!useIntersect)
                    sb.AppendFormat("SELECT uuid FROM {0} WHERE ", storedQueryName);

                foreach (var s in parms)
                {
                    if (useIntersect)
                        sb.AppendFormat("SELECT uuid FROM {0} WHERE ", storedQueryName);

                    object rValue = s.Value;
                    if (!(rValue is IList))
                        rValue = new List<Object>() { rValue };

                    sb.Append("(");

                    string key = s.Key.Replace(".", "_"),
                        scopedQuery = storedQueryName + ".";
                    // Are there guards?
                    if (key.EndsWith("]")) // Exists!!!
                    {
                        if ((string)s.Value == "null")
                            sb.Append("NOT ");
                        sb.AppendFormat("EXISTS (SELECT uuid FROM {0} AS exist_check WHERE exist_check.uuid = {1}uuid AND ", storedQueryName, scopedQuery);
                        scopedQuery = "exist_check.";
                    }

                    while (key.Contains("["))
                    {
                        var guardRoot = key.Substring(0, key.IndexOf("["));
                        var guardValue = key.Substring(key.IndexOf("[") + 1, key.IndexOf("]") - key.IndexOf("[") - 1);
                        sb.AppendFormat(" {0}{1}_guard = ? AND ", scopedQuery, guardRoot);
                        vals.Add(guardValue);
                        key = guardRoot + key.Substring(key.IndexOf("]") + 1);
                    }

                    if (s.Key.EndsWith("]")) // Exists - Finish query
                    {
                        sb.Remove(sb.Length - 4, 4);
                        sb.Append(")");
                    }
                    else
                    {
                        foreach (var itm in rValue as IList)
                        {
                            var value = itm;

                            if (key == "id") key = "uuid";

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
                                        if (sValue.Equals("!null"))
                                        {
                                            sb.AppendFormat(" {0} IS NOT NULL AND ", key);
                                            value = sValue = null;
                                        }
                                        else
                                        {
                                            sb.AppendFormat(" {0} <> ? AND ", key);
                                            value = sValue.Substring(1);
                                        }
                                        break;
                                    case '~':
                                        sb.AppendFormat(" {0} LIKE '%' || ? || '%'  OR ", key);
                                        value = sValue.Substring(1);
                                        break;
                                    default:
                                        if (sValue.Equals("null"))
                                        {
                                            sb.AppendFormat("{0} IS NULL OR ", key);
                                            value = sValue = null;
                                        }
                                        else
                                            sb.AppendFormat(" {0} = ?  OR ", key);
                                        break;
                                }
                            }
                            else
                                sb.AppendFormat(" {0} = ?  OR ", key);

                            // Value correction
                            DateTime tdateTime = default(DateTime);
                            Guid gValue = Guid.Empty;
                            if (value == null)
                                ;
                            else if (value is Guid)
                                vals.Add(((Guid)value).ToByteArray());
                            else if (Guid.TryParse(value.ToString(), out gValue))
                                vals.Add(gValue.ToByteArray());
                            else if (DateTime.TryParse(value.ToString(), out tdateTime))
                                vals.Add(tdateTime);
                            else
                                vals.Add(value);
                        }
                        sb.Remove(sb.Length - 4, 4);
                    } // exist or value

                    if (useIntersect)
                        sb.Append(") INTERSECT ");
                    else
                        sb.Append(") AND ");

                }
            }

            if (useIntersect)
                sb.Remove(sb.Length - 10, 10);
            else
                sb.Remove(sb.Length - 4, 4);
            sb.Append(") ");

            if (typeof(DbBaseData).GetTypeInfo().IsAssignableFrom(typeof(TDomain).GetTypeInfo()))
                sb.Append(" ORDER BY creationTime DESC ");
            else
                sb.Append(" ORDER BY uuid ASC ");

#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
            this.m_tracer.TraceVerbose("EXECUTE: {0}", sb);
            foreach (var v in vals)
                this.m_tracer.TraceVerbose(" --> {0}", v is byte[] ? BitConverter.ToString(v as Byte[]).Replace("-", "") : v);
#endif

            // First get total results before we reduce the result-set size
            var retVal = context.Connection.Query<TDomain>(sb.ToString(), vals.ToArray()).ToList();

            if (queryId != Guid.Empty)
                this.m_queryPersistence.RegisterQuerySet(queryId, retVal.Select(o => o.Key), parms);

            // Retrieve the results
            if (countResults)
                totalResults = retVal.Count();
            else
                totalResults = 0;
#if DEBUG
            sw.Stop();
            this.m_tracer.TraceVerbose("Query Finished: {0}", sw.ElapsedMilliseconds);
#endif
            //totalResults = retVal.Count;
            return retVal.Skip(offset).Take(count < 0 ? 100 : count).Select(o => this.CacheConvert(o, context)).ToList();
        }

        /// <summary>
        /// Get the specified data element
        /// </summary>
        internal override TModel Get(LocalDataContext context, Guid key)
        {
            var existing = ApplicationContext.Current.GetService<IDataCachingService>().GetCacheItem(typeof(TModel), key);
            if (existing != null)
                return existing as TModel;
            // Get from the database
            try
            {
                int t = 0;
                if (typeof(TQueryResult) != typeof(TDomain)) // faster to join
                    return this.QueryInternal(context, o => o.Key == key, 0, 1, out t, Guid.Empty, false).FirstOrDefault();
                else
                {
                    var kuuid = key.ToByteArray();
                    return this.CacheConvert(context.Connection.Table<TDomain>().Where(o => o.Uuid == kuuid).FirstOrDefault(), context);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Update associated version items
        /// </summary>
        protected void UpdateAssociatedItems<TAssociation, TModelEx>(IEnumerable<TAssociation> existing, IEnumerable<TAssociation> storage, Guid? sourceKey, LocalDataContext dataContext, bool inversionInd = false)
            where TAssociation : IdentifiedData, ISimpleAssociation, new()
            where TModelEx : IdentifiedData
        {

            // Remove all empty associations (associations which are messy)
            storage = storage.Where(o => !o.IsEmpty()).ToList();

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
                else if (!inversionInd && itm.SourceEntityKey != sourceKey) // ODD!!! This looks like a copy / paste job from a down-level serializer fix it
                {
                    itm.SourceEntityKey = sourceKey;
                    itm.Key = null;
                }

            // Remove old
            var obsoleteRecords = existing.Where(o => !storage.Any(ecn => ecn.Key == o.Key));
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

