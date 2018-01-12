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
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Map;
using System.Reflection;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Synchronization.Model;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model;
using System.Collections;
using System.Collections.Generic;
using SQLite.Net;
using OpenIZ.Mobile.Core.Data.Model.Security;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.EntityLoader;
using System.Threading;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Data.Connection;
using System.Diagnostics;
using OpenIZ.Core.Services;
using OpenIZ.Core.Data.QueryBuilder;
using OpenIZ.Mobile.Core.Data.Hacks;

namespace OpenIZ.Mobile.Core.Data
{
    /// <summary>
    /// Represents a data persistence service which stores data in the local SQLite data store
    /// </summary>
    public abstract class LocalPersistenceServiceBase<TData> : IDataPersistenceService<TData>, ILocalPersistenceService where TData : IdentifiedData, new()
    {

       

        // Get tracer
        protected Tracer m_tracer; //= Tracer.GetTracer(typeof(LocalPersistenceServiceBase<TData>));

        // Configuration
        protected static DataConfigurationSection m_configuration = ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>();

        // Mapper
        protected static ModelMapper m_mapper;

        // Builder
        protected static QueryBuilder m_builder;

        // Static CTOR
        static LocalPersistenceServiceBase()
        {

            m_mapper = LocalPersistenceService.Mapper;
            m_builder = new QueryBuilder(m_mapper, new RelationshipQueryHack());
        }

        public LocalPersistenceServiceBase()
        {
            this.m_tracer = Tracer.GetTracer(this.GetType());
        }

        #region IDataPersistenceService implementation
        /// <summary>
        /// Occurs when inserted.
        /// </summary>
        public event EventHandler<DataPersistenceEventArgs<TData>> Inserted;
        /// <summary>
        /// Occurs when inserting.
        /// </summary>
        public event EventHandler<DataPersistencePreEventArgs<TData>> Inserting;
        /// <summary>
        /// Occurs when updated.
        /// </summary>
        public event EventHandler<DataPersistenceEventArgs<TData>> Updated;
        /// <summary>
        /// Occurs when updating.
        /// </summary>
        public event EventHandler<DataPersistencePreEventArgs<TData>> Updating;
        /// <summary>
        /// Occurs when obsoleted.
        /// </summary>
        public event EventHandler<DataPersistenceEventArgs<TData>> Obsoleted;
        /// <summary>
        /// Occurs when obsoleting.
        /// </summary>
        public event EventHandler<DataPersistencePreEventArgs<TData>> Obsoleting;
        /// <summary>
        /// Occurs when queried.
        /// </summary>
        public event EventHandler<DataQueryEventArgsBase<TData>> Queried;
        /// <summary>
        /// Occurs when querying.
        /// </summary>
        public event EventHandler<DataQueryEventArgsBase<TData>> Querying;

        /// <summary>
        /// Fire inserting event
        /// </summary>
        protected void FireInserting(DataPersistencePreEventArgs<TData> evt)
        {
            this.Inserting?.Invoke(this, evt);
        }

        /// <summary>
        /// Fire inserting event
        /// </summary>
        protected void FireInserted(DataPersistenceEventArgs<TData> evt)
        {
            this.Inserted?.Invoke(this, evt);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns>The connection.</returns>
        protected LocalDataContext CreateConnection()
        {
            return new LocalDataContext(SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current.Configuration.GetConnectionString(m_configuration.MainDataSourceConnectionStringName).Value));
        }

        /// <summary>
        /// Create readonly connection
        /// </summary>
        private LocalDataContext CreateReadonlyConnection()
        {
            return new LocalDataContext(SQLiteConnectionManager.Current.GetReadonlyConnection(ApplicationContext.Current.Configuration.GetConnectionString(m_configuration.MainDataSourceConnectionStringName).Value));
        }

        /// <summary>
        /// Insert the specified data.
        /// </summary>
        /// <param name="data">Data.</param>
        public virtual TData Insert(TData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            DataPersistencePreEventArgs<TData> preArgs = new DataPersistencePreEventArgs<TData>(data);
            this.Inserting?.Invoke(this, preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceWarning("Pre-Event handler indicates abort insert for {0}", data);
                return data;
            }

#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            // Persist object
            using (var context = this.CreateConnection())
                try
                {
                    using (context.LockConnection())
                    {
                        try
                        {
                            
                            this.m_tracer.TraceVerbose("INSERT {0}", data);

                            context.Connection.BeginTransaction();
                            data = this.Insert(context, data);
                            context.Connection.Commit();
                            // Remove from the cache
                            foreach (var itm in context.CacheOnCommit.AsParallel())
                                ApplicationContext.Current.GetService<IDataCachingService>().Add(itm);
                        }
                        catch (Exception e)
                        {
                            this.m_tracer.TraceError("Error : {0}", e);
                            context.Connection.Rollback();
                            throw new LocalPersistenceException(DataOperationType.Insert, data, e);
                        }

                    }
                    this.Inserted?.Invoke(this, new DataPersistenceEventArgs<TData>(data));
                    return data;
                }
                catch { throw; }

#if PERFMON
            finally
            {
                sw.Stop();
                    ApplicationContext.Current.PerformanceLog(typeof(TData).Name, nameof(Insert), "Complete", sw.Elapsed);
            }
#endif

        }

        /// <summary>
        /// Update the specified data
        /// </summary>
        /// <param name="data">Data.</param>
        public virtual TData Update(TData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            else if (!data.Key.HasValue || data.Key == Guid.Empty)
                throw new InvalidOperationException("Data missing key");

            DataPersistencePreEventArgs<TData> preArgs = new DataPersistencePreEventArgs<TData>(data);
            this.Updating?.Invoke(this, preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceWarning("Pre-Event handler indicates abort update for {0}", data);
                return data;
            }
#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            // Persist object
            using (var context = this.CreateConnection())
                try
                {
                    using (context.LockConnection())
                    {
                        try
                        {
                            this.m_tracer.TraceVerbose("UPDATE {0}", data);
                            context.Connection.BeginTransaction();

                            data = this.Update(context, data);

                            context.Connection.Commit();

                            // Remove from the cache
                            foreach (var itm in context.CacheOnCommit.AsParallel())
                                ApplicationContext.Current.GetService<IDataCachingService>().Add(itm);

                        }
                        catch (Exception e)
                        {
                            this.m_tracer.TraceError("Error : {0}", e);
                            context.Connection.Rollback();
                            throw new LocalPersistenceException(DataOperationType.Update, data, e);

                        }
                    }
                    this.Updated?.Invoke(this, new DataPersistenceEventArgs<TData>(data));
                    return data;
                }
                catch { throw; }
#if PERFMON
                finally
                {
                    sw.Stop();
                    ApplicationContext.Current.PerformanceLog(typeof(TData).Name, nameof(Update), "Complete", sw.Elapsed);
                }
#endif
        }

        /// <summary>
        /// Obsolete the specified identified data
        /// </summary>
        /// <param name="data">Data.</param>
        public virtual TData Obsolete(TData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            else if (!data.Key.HasValue || data.Key == Guid.Empty)
                throw new InvalidOperationException("Data missing key");
#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            DataPersistencePreEventArgs<TData> preArgs = new DataPersistencePreEventArgs<TData>(data);
            this.Obsoleting?.Invoke(this, preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceWarning("Pre-Event handler indicates abort for {0}", data);
                return data;
            }

            // Obsolete object
            using (var context = this.CreateConnection())
                try
                {
                    using (context.LockConnection())
                    {
                        try
                        {
                            this.m_tracer.TraceVerbose("OBSOLETE {0}", data);
                            context.Connection.BeginTransaction();

                            data = this.Obsolete(context, data);

                            context.Connection.Commit();

                            // Remove from the cache
                            foreach (var itm in context.CacheOnCommit.AsParallel())
                                ApplicationContext.Current.GetService<IDataCachingService>().Remove(itm.Key.Value);

                        }
                        catch (Exception e)
                        {
                            this.m_tracer.TraceError("Error : {0}", e);
                            context.Connection.Rollback();
                            throw new LocalPersistenceException(DataOperationType.Obsolete, data, e);
                        }
                    }
                    this.Obsoleted?.Invoke(this, new DataPersistenceEventArgs<TData>(data));

                    return data;
                }
                catch { throw; }
#if PERFMON
                finally
                {
                    sw.Stop();
                    ApplicationContext.Current.PerformanceLog(typeof(TData).Name, nameof(Obsolete), "Complete", sw.Elapsed);
                }

#endif
        }

        /// <summary>
        /// Get the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        public virtual TData Get(Guid key)
        {
            if (key == Guid.Empty) return null;
            var existing = ApplicationContext.Current.GetService<IDataCachingService>().GetCacheItem(key);
            if ((existing as IdentifiedData)?.LoadState <= LoadState.FullLoad) {
                using (var context = this.CreateConnection())
                    try
                    {
                        using (context.LockConnection())
                        {
                            (existing as IdentifiedData).LoadAssociations(context);
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Error loading associations: {0}", e);
                    }
            }
            if (existing != null)
                return existing as TData;
            int toss = 0;
            this.m_tracer.TraceInfo("GET: {0}", key);
            return this.Query(o => o.Key == key, 0, 1, out toss, Guid.Empty, false, false)?.SingleOrDefault();
        }

        /// <summary>
        /// Query the specified data
        /// </summary>
        /// <param name="query">Query.</param>
        public virtual System.Collections.Generic.IEnumerable<TData> Query(System.Linq.Expressions.Expression<Func<TData, bool>> query)
        {
            int totalResults = 0;
            return this.Query(query, 0, null, out totalResults, Guid.Empty, false, false);
        }

        /// <summary>
        /// Query the specified data
        /// </summary>
        /// <param name="query">Query.</param>
        public virtual System.Collections.Generic.IEnumerable<TData> Query(System.Linq.Expressions.Expression<Func<TData, bool>> query, int offset, int? count, out int totalResults, Guid queryId)
        {
            return this.Query(query, offset, count, out totalResults, queryId, true, false);
        }

        /// <summary>
        /// Query the specified data
        /// </summary>
        /// <param name="query">Query.</param>
        public virtual System.Collections.Generic.IEnumerable<TData> QueryFast(System.Linq.Expressions.Expression<Func<TData, bool>> query, int offset, int? count, out int totalResults, Guid queryId)
        {
            return this.Query(query, offset, count, out totalResults, queryId, true, true);
        }

        /// <summary>
        /// Query the specified data
        /// </summary>
        /// <param name="query">Query.</param>
        public virtual System.Collections.Generic.IEnumerable<TData> QueryExplicitLoad(System.Linq.Expressions.Expression<Func<TData, bool>> query, int offset, int? count, out int totalResults, Guid queryId, IEnumerable<String> expandProperties)
        {
            return this.Query(query, offset, count, out totalResults, queryId, true, true, expandProperties);
        }

        /// <summary>
        /// Query function returning results and count control
        /// </summary>
        private IEnumerable<TData> Query(System.Linq.Expressions.Expression<Func<TData, bool>> query, int offset, int? count, out int totalResults, Guid queryId, bool countResults, bool fastQuery, IEnumerable<String> expandProperties = null)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            DataQueryPreEventArgs<TData> preArgs = new DataQueryPreEventArgs<TData>(query, offset, count);
            this.Querying?.Invoke(this, preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceWarning("Pre-Event handler indicates abort query {0}", query);
                totalResults = preArgs.TotalResults;
                return preArgs.Results;
            }

#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            // Query object
            using (var context = this.CreateReadonlyConnection())
                try
                {
                    IEnumerable<TData> results = null;
                    using (context.LockConnection())
                    {
                        this.m_tracer.TraceVerbose("QUERY {0}", query);

                        if (fastQuery)
                            context.DelayLoadMode = LoadState.PartialLoad;
                        else
                            context.DelayLoadMode = LoadState.FullLoad;

                        if (expandProperties != null)
                            context.LoadAssociations = expandProperties.ToArray();

                        results = this.Query(context, query, offset, count ?? -1, out totalResults, queryId, countResults);
                    }

                    var postData = new DataQueryResultEventArgs<TData>(query, results, offset, count, totalResults);
                    this.Queried?.Invoke(this, postData);

                    totalResults = postData.TotalResults;

                    // Remove from the cache
                    foreach (var itm in context.CacheOnCommit.AsParallel())
                        ApplicationContext.Current.GetService<IDataCachingService>()?.Add(itm);

                    return postData.Results;


                }
                catch (NotSupportedException e)
                {
                    this.m_tracer.TraceVerbose("Cannot perform LINQ query, switching to stored query sqp_{0}", typeof(TData).Name, e);

                    // Build dictionary
                    var httpValues = QueryExpressionBuilder.BuildQuery<TData>(query, true);
                    var filter = new Dictionary<String, Object>();

                    foreach (var f in httpValues)
                    {
                        object existing = null;
                        if (filter.TryGetValue(f.Key, out existing))
                        {
                            if (!(existing is IList))
                            {
                                existing = new List<Object>() { existing };
                                filter[f.Key] = existing;
                            }
                            (existing as IList).Add(f.Value);
                        }
                        else
                            filter.Add(f.Key, f.Value);

                    }
                    // Query
                    return this.Query(String.Format("sqp_{0}", typeof(TData).Name), filter, offset, count, out totalResults, queryId);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error : {0}", e);
                    throw;
                }
#if PERFMON
                finally
                {
                    sw.Stop();
                    ApplicationContext.Current.PerformanceLog(typeof(TData).Name, nameof(Query), query.ToString(), sw.Elapsed);
                }

#endif

        }

 
        /// <summary>
        /// Query this instance.
        /// </summary>
        public virtual IEnumerable<TData> Query(String storedQueryName, IDictionary<String, Object> parms)
        {
            int totalResults = 0;
            return this.Query(storedQueryName, parms, 0, null, out totalResults, Guid.Empty, false);
        }

        /// <summary>
        /// Perform a count
        /// </summary>
        public virtual int Count(Expression<Func<TData, bool>> query)
        {
            var tr = 0;
            this.Query(query, 0, null, out tr, Guid.Empty);
            return tr;
        }

        /// <summary>
        /// Query this instance.
        /// </summary>
        public virtual IEnumerable<TData> Query(String storedQueryName, IDictionary<String, Object> parms, int offset, int? count, out int totalResults, Guid queryId)
        {
            return this.Query(storedQueryName, parms, offset, count, out totalResults, queryId, true);
        }

        /// <summary>
        /// Perform query with control
        /// </summary>
        private IEnumerable<TData> Query(String storedQueryName, IDictionary<String, Object> parms, int offset, int? count, out int totalResults, Guid queryId, bool countResults)
        {
            if (String.IsNullOrEmpty(storedQueryName))
                throw new ArgumentNullException(nameof(storedQueryName));
            else if (parms == null)
                throw new ArgumentNullException(nameof(parms));

            DataStoredQueryPreEventArgs<TData> preArgs = new DataStoredQueryPreEventArgs<TData>(storedQueryName, parms, offset, count);
            this.Querying?.Invoke(this, preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceWarning("Pre-Event handler indicates abort query {0}", storedQueryName);
                totalResults = preArgs.TotalResults;
                return preArgs.Results;
            }

            // Query object
            using (var context = this.CreateConnection())
                try
                {
                    List<TData> results = null;
                    using (context.LockConnection())
                    {
                        this.m_tracer.TraceVerbose("STORED QUERY {0}", storedQueryName);

                        results = this.Query(context, storedQueryName, parms, offset, count ?? -1, out totalResults, queryId, countResults).ToList();
                    }

                    var postArgs = new DataStoredQueryResultEventArgs<TData>(storedQueryName, parms, results, offset, count, totalResults);
                    this.Queried?.Invoke(this, postArgs);

                    totalResults = postArgs.TotalResults;


                    // Remove from the cache
                    foreach (var itm in context.CacheOnCommit.AsParallel())
                        ApplicationContext.Current.GetService<IDataCachingService>().Add(itm);

                    return postArgs.Results;
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error : {0}", e);
                    throw;
                }
        }

        #endregion

        /// <summary>
        /// Get the current user UUID.
        /// </summary>
        /// <returns>The user UUID.</returns>
        protected Guid CurrentUserUuid(LocalDataContext context)
        {
            if (AuthenticationContext.Current.Principal == null)
                return Guid.Empty;
            String name = AuthenticationContext.Current.Principal.Identity.Name;
            var securityUser = context.Connection.Table<DbSecurityUser>().Where(o => o.UserName == name).ToList().SingleOrDefault();
            if (securityUser == null)
            {
                this.m_tracer.TraceWarning("User doesn't exist locally, using GUID.EMPTY");
                return Guid.Empty;
                //throw new InvalidOperationException("Constraint Violation: User doesn't exist locally");
            }
            return securityUser.Key;
        }

        /// <summary>
        /// Performthe actual insert.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public TData Insert(LocalDataContext context, TData data)
        {
            var retVal = this.InsertInternal(context, data);
            //if (retVal != data) System.Diagnostics.Debugger.Break();
            context.AddCacheCommit(retVal);
            return retVal;
        }
        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public TData Update(LocalDataContext context, TData data)
        {
            // JF- Probably no need to do this now
            // Make sure we're updating the right thing
            //if (data.Key.HasValue)
            //{
            //    var cacheItem = ApplicationContext.Current.GetService<IDataCachingService>()?.GetCacheItem(data.GetType(), data.Key.Value);
            //    if (cacheItem != null)
            //    {
            //        cacheItem.CopyObjectData(data);
            //        data = cacheItem as TData;
            //    }
            //}

            var retVal = this.UpdateInternal(context, data);
            //if (retVal != data) System.Diagnostics.Debugger.Break();
            context.AddCacheCommit(retVal);
            return retVal;

        }
        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public TData Obsolete(LocalDataContext context, TData data)
        {
            var retVal = this.ObsoleteInternal(context, data);
            //if (retVal != data) System.Diagnostics.Debugger.Break();
            context.AddCacheCommit(retVal);
            return retVal;
        }
        /// <summary>
        /// Performs the actual query
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="query">Query.</param>
        public IEnumerable<TData> Query(LocalDataContext context, Expression<Func<TData, bool>> query, int offset, int count, out int totalResults, Guid queryId, bool countResults)
        {
            var retVal = this.QueryInternal(context, query, offset, count, out totalResults, queryId, countResults);

            foreach (var i in retVal.Where(i => i != null))
                context.AddCacheCommit(i);

            return retVal;

        }
        /// <summary>
        /// Performs the actual query
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="query">Query.</param>
        public IEnumerable<TData> Query(LocalDataContext context, String storedQueryName, IDictionary<String, Object> parms, int offset, int count, out int totalResults, Guid queryId, bool countResults)
        {
            var retVal = this.QueryInternal(context, storedQueryName, parms, offset, count, out totalResults, queryId, countResults);

            foreach (var i in retVal.Where(i => i != null))
                context.AddCacheCommit(i);

            return retVal;

        }


        /// <summary>
        /// Maps the data to a model instance
        /// </summary>
        /// <returns>The model instance.</returns>
        /// <param name="dataInstance">Data instance.</param>
        public abstract TData ToModelInstance(Object dataInstance, LocalDataContext context);

        /// <summary>
        /// Froms the model instance.
        /// </summary>
        /// <returns>The model instance.</returns>
        /// <param name="modelInstance">Model instance.</param>
        public abstract Object FromModelInstance(TData modelInstance, LocalDataContext context);

        /// <summary>
        /// Performthe actual insert.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        protected abstract TData InsertInternal(LocalDataContext context, TData data);
        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        protected abstract TData UpdateInternal(LocalDataContext context, TData data);
        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        protected abstract TData ObsoleteInternal(LocalDataContext context, TData data);
        /// <summary>
        /// Performs the actual query
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="query">Query.</param>
        protected abstract IEnumerable<TData> QueryInternal(LocalDataContext context, Expression<Func<TData, bool>> query, int offset, int count, out int totalResults, Guid queryId, bool countResults);
        /// <summary>
        /// Performs the actual query
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="query">Query.</param>
        protected abstract IEnumerable<TData> QueryInternal(LocalDataContext context, String storedQueryName, IDictionary<String, Object> parms, int offset, int count, out int totalResults, Guid queryId, bool countResults);

        /// <summary>
        /// Query internal without caring about limiting
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public IEnumerable<TData> Query(LocalDataContext context, Expression<Func<TData, bool>> expr)
        {
            int t;
            return this.QueryInternal(context, expr, 0, -1, out t, Guid.Empty, false);
        }

        /// <summary>
        /// Get the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        internal virtual TData Get(LocalDataContext context, Guid key)
        {
            int totalResults = 0;
            var existing = ApplicationContext.Current.GetService<IDataCachingService>().GetCacheItem(key);
            if (existing != null)
                return existing as TData;
            return this.QueryInternal(context, o => o.Key == key, 0, 1, out totalResults, Guid.Empty, false)?.SingleOrDefault();
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public virtual object Insert(object data)
        {
            return this.Insert(data as TData);
        }

        /// <summary>
        /// Update the specified object
        /// </summary>
        public virtual object Update(object data)
        {
            return this.Update(data as TData);
        }

        /// <summary>
        /// Obsoletes the specified data
        /// </summary>
        public virtual object Obsolete(object data)
        {
            return this.Obsolete(data as TData);
        }

        /// <summary>
        /// Gets the specified data
        /// </summary>
        object IDataPersistenceService.Get(Guid id)
        {
            return this.Get(id);
        }

        /// <summary>
        /// Query the specified object
        /// </summary>
        public IEnumerable Query(Expression query, int offset, int? count, out int totalResults)
        {
            return this.Query((Expression<Func<TData, bool>>)query, offset, count, out totalResults, Guid.Empty);
        }

        /// <summary>
        /// Insert the specified data
        /// </summary>
        object ILocalPersistenceService.Insert(LocalDataContext context, object data)
        {
            return this.Insert(context, (TData)data);
        }

        /// <summary>
        /// Update
        /// </summary>
        object ILocalPersistenceService.Update(LocalDataContext context, object data)
        {
            return this.Update(context, (TData)data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        object ILocalPersistenceService.Obsolete(LocalDataContext context, object data)
        {
            return this.Obsolete(context, (TData)data);
        }

        /// <summary>
        /// Get the specified data
        /// </summary>
        object ILocalPersistenceService.Get(LocalDataContext context, Guid id)
        {
            return this.Get(context, id);
        }

        /// <summary>
        /// To model instance
        /// </summary>
        object ILocalPersistenceService.ToModelInstance(object domainInstance, LocalDataContext context)
        {
            return this.ToModelInstance(domainInstance, context);
        }
    }
}

