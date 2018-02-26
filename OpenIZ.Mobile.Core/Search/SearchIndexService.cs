/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Connection;
using SQLite.Net;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Mobile.Core.Search.Model;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Constants;
using System.Threading;
using OpenIZ.Mobile.Core.Resources;
using OpenIZ.Core.Model.DataTypes;
using SQLite.Net.Interop;
using OpenIZ.Mobile.Core.Data;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Core.Interfaces;
using OpenIZ.Core.Model;

namespace OpenIZ.Mobile.Core.Search
{
    /// <summary>
    /// Search indexing service
    /// </summary>
    public class SearchIndexService : IFreetextSearchService, IDaemonService, IAuditEventSource
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SearchIndexService));

        // Is bound?
        private bool m_patientBound = false;
        private bool m_bundleBound = false;

        /// <summary>
        /// Returns whether the service is running or not
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return true;
            }
        }

        // Lock object
        private object m_lock = new object();
        /// <summary>
        /// The service is starting
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// The service is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// The service has stopped
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// The service is stopping
        /// </summary>
        public event EventHandler Stopping;
        public event EventHandler<AuditDataEventArgs> DataCreated;
        public event EventHandler<AuditDataEventArgs> DataUpdated;
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;

        /// <summary>
        /// Create a connection
        /// </summary>
        private LockableSQLiteConnection CreateConnection()
        {
            return SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current.Configuration.GetConnectionString("openIzSearch").Value);
        }

        /// <summary>
        /// Perform a search of the free-text index
        /// </summary>
        public IEnumerable<TEntity> Search<TEntity>(string term, int offset, int? count, out int totalResults) where TEntity : Entity
        {
            // Tokenize on space
            var tokens = term.Split(' ');
            return this.Search<TEntity>(tokens, offset, count, out totalResults);
        }

        /// <summary>
        /// Search based on already tokenized string
        /// </summary>
        public IEnumerable<TEntity> Search<TEntity>(String[] tokens, int offset, int? count, out int totalResults) where TEntity : Entity
        {
            try
            {
                var conn = this.CreateConnection();
                using (conn.Lock())
                {
                    // Search query builder
                    StringBuilder queryBuilder = new StringBuilder();

                    queryBuilder.AppendFormat("SELECT DISTINCT {1}.* FROM {0} INNER JOIN {1} ON ({0}.entity = {1}.key) WHERE {1}.type = '{2}' AND {0}.entity IN (",
                        conn.GetMapping<Model.SearchTermEntity>().TableName,
                        conn.GetMapping<Model.SearchEntityType>().TableName,
                        typeof(TEntity).FullName);

                    foreach (var tkn in tokens)
                    {
                        queryBuilder.AppendFormat("SELECT {0}.entity FROM {0} INNER JOIN {1} ON ({0}.term = {1}.key) WHERE ",
                            conn.GetMapping<Model.SearchTermEntity>().TableName,
                            conn.GetMapping<Model.SearchTerm>().TableName,
                            typeof(TEntity).FullName);

                        if (tkn.Contains("*"))
                            queryBuilder.AppendFormat("{0}.term LIKE '{1}' ", conn.GetMapping<Model.SearchTerm>().TableName, tkn.Replace("'", "''").Replace("*", "%"));
                        else
                            queryBuilder.AppendFormat("{0}.term = '{1}' ", conn.GetMapping<Model.SearchTerm>().TableName, tkn.ToLower().Replace("'", "''"));
                        queryBuilder.Append(" INTERSECT ");
                    }

                    queryBuilder.Remove(queryBuilder.Length - 11, 11);
                    queryBuilder.Append(")");

                    // Search now!
                    this.m_tracer.TraceVerbose("FREETEXT SEARCH: {0}", queryBuilder);

                    // Perform query
                    var results = conn.Query<Model.SearchEntityType>(queryBuilder.ToString());

                    var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<TEntity>>();
                    totalResults = results.Count();

                    var retVal = results.Skip(offset).Take(count ?? 100).AsParallel().Select(o => persistence.Get(new Guid(o.Key)));

                    this.DataDisclosed?.Invoke(this, new AuditDataDisclosureEventArgs("FTS:" + String.Join(":", tokens), retVal));
                    return retVal;
                }
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error performing search: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Perform an index of the entity
        /// </summary>
        private bool IndexEntity(params Entity[] entities)
        {
            if (entities.Length == 0) return true;

            var conn = this.CreateConnection();
            using (conn.Lock())
            {
                try
                {
                    conn.BeginTransaction();

                    foreach (var e in entities)
                    {
                        var entityUuid = e.Key.Value.ToByteArray();
                        var entityVersionUuid = e.VersionKey.Value.ToByteArray();

                        if (conn.Table<SearchEntityType>().Where(o => o.Key == entityUuid && o.VersionKey == entityVersionUuid).Count() > 0) return true; // no change
                        else if (e.LoadCollection<EntityTag>("Tags").Any(t => t.TagKey == "isAnonymous")) // Anonymous?
                            continue;

                        var tokens = (e.Names ?? new List<EntityName>()).SelectMany(o => o.Component.Select(c => c.Value.Trim().ToLower()))
                        .Union((e.Identifiers ?? new List<EntityIdentifier>()).Select(o => o.Value.ToLower()))
                        .Union((e.Addresses ?? new List<EntityAddress>()).SelectMany(o => o.Component.Select(c => c.Value.Trim().ToLower())))
                        .Union((e.Telecoms ?? new List<EntityTelecomAddress>()).Select(o => o.Value.ToLower()))
                        .Union((e.Relationships ?? new List<EntityRelationship>()).Where(o => o.TargetEntity is Person).SelectMany(o => (o.TargetEntity.Names ?? new List<EntityName>()).SelectMany(n => n.Component?.Select(c => c.Value?.Trim().ToLower()))))
                        .Union((e.Relationships ?? new List<EntityRelationship>()).Where(o => o.TargetEntity is Person).SelectMany(o => (o.TargetEntity.Telecoms ?? new List<EntityTelecomAddress>()).Select(c => c.Value?.Trim().ToLower())))
                        .Where(o => o != null);

                        // Insert new terms
                        var existing = conn.Table<SearchTerm>().Where(o => tokens.Contains(o.Term)).ToArray();
                        var inserting = tokens.Where(t => !existing.Any(x => x.Term == t)).Select(o => new SearchTerm() { Term = o }).ToArray();
                        conn.InsertAll(inserting);

                        this.m_tracer.TraceVerbose("{0}", e);
                        foreach (var itm in existing.Union(inserting))
                            this.m_tracer.TraceVerbose("\t+{0}", itm.Term);

                        // Now match tokens with this 
                        conn.Execute(String.Format(String.Format("DELETE FROM {0} WHERE entity = ?", conn.GetMapping<SearchTermEntity>().TableName), e.Key.Value.ToByteArray()));
                        conn.Delete<SearchEntityType>(e.Key.Value.ToByteArray());
                        var insertRefs = existing.Union(inserting).Distinct().Select(o => new SearchTermEntity() { EntityId = e.Key.Value.ToByteArray(), TermId = o.Key }).ToArray();
                        conn.InsertAll(insertRefs);
                        conn.Insert(new SearchEntityType() { Key = e.Key.Value.ToByteArray(), SearchType = e.GetType().FullName, VersionKey = e.VersionKey.Value.ToByteArray() });

                        this.m_tracer.TraceVerbose("Indexed {0}", e);

                    }

                    // Now commit
                    conn.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    this.m_tracer.TraceError("Error indexing {0} : {1}", entities, ex);
                    conn.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Perform an index of the entity
        /// </summary>
        private bool DeleteEntity(Entity e)
        {
            var conn = this.CreateConnection();
            using (conn.Lock())
            {
                try
                {
                    conn.BeginTransaction();

                    // Now match tokens with this 
                    conn.Execute(String.Format(String.Format("DELETE FROM {0} WHERE entity = ?", conn.GetMapping<SearchTermEntity>().TableName), e.Key.Value.ToByteArray()));
                    conn.Delete<SearchEntityType>(e.Key.Value.ToByteArray());

                    // Now commit
                    conn.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    this.m_tracer.TraceError("Error indexing {0} : {1}", e, ex);
                    conn.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Queue indexing in the background
        /// </summary>
        private bool IndexBackground(Entity e)
        {
            ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem((o) => this.IndexEntity(o as Entity), e);
            return true;
        }

        /// <summary>
        /// Start the service. 
        /// </summary>
        /// <remarks>In reality this forces a background re-index of the database and subscription to the entity persistence services
        /// to update the index where possible</remarks>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            // After start then we want to start up the indexer
            ApplicationContext.Current.Started += (so, se) =>
            {
                try
                {
                    // Subscribe to persistence events which will have an impact on the index
                    var patientPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
                    var bundlePersistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();

                    // Bind entity
                    if (patientPersistence != null && !this.m_patientBound)
                    {
                        patientPersistence.Inserted += (o, e) => this.IndexBackground(e.Data);
                        patientPersistence.Updated += (o, e) => this.IndexBackground(e.Data);
                        patientPersistence.Obsoleted += (o, e) => this.DeleteEntity(e.Data);
                        this.m_patientBound = true;
                    }

                    // Bind entity
                    if (bundlePersistence != null && !this.m_bundleBound)
                    {
                        Action<Object> doBundleIndex = (o) =>
                        {
                            this.IndexEntity((o as Bundle).Item.Where(e => e is Patient).OfType<Entity>().ToArray());
                        };

                        bundlePersistence.Inserted += (o, e) => ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(doBundleIndex, e.Data);
                        bundlePersistence.Updated += (o, e) => ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(doBundleIndex, e.Data);
                        bundlePersistence.Obsoleted += (o, e) => ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(doBundleIndex, e.Data);
                    }


                    try
                    {
                        // Not Indexed
                        if (ApplicationContext.Current.Configuration.GetAppSetting("openiz.mobile.core.search.lastIndex") == null)
                            this.Index();
                    }
                    catch { }

                    this.Started?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error starting search index: {0}", e);
                }

            };

            return true;
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Perform full index
        /// </summary>
        public bool Index()
        {
            ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem((o) =>
            {
                if (Monitor.TryEnter(this.m_lock))
                    try
                    {
                        this.m_tracer.TraceInfo("Starting complete full-text indexing of the primary datastore");
                        try
                        {
                                // Load all entities in database and index them
                                int tr = 101, ofs = 0;
                            var patientService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
                            Guid queryId = Guid.NewGuid();

                            while (tr > ofs + 50)
                            {

                                if (patientService == null) break;
                                var entities = patientService.Query(e => e.StatusConceptKey != StatusKeys.Obsolete, ofs, 50, out tr, queryId);

                                    // Index 
                                    this.IndexEntity(entities.ToArray());

                                    // Let user know the status
                                    ofs += 50;
                                ApplicationContext.Current.SetProgress(Strings.locale_indexing, (float)ofs / tr);
                            }
                            if (patientService != null)
                                ApplicationContext.Current.Configuration.SetAppSetting("openiz.mobile.core.search.lastIndex", DateTime.Now);
                        }
                        catch (Exception e)
                        {
                            this.m_tracer.TraceError("Error indexing primary database: {0}", e);
                            throw;
                        }
                    }
                    finally
                    {
                        Monitor.Exit(this.m_lock);
                    }
            });
            return true; // indexing is going to occur
        }
    }
}
