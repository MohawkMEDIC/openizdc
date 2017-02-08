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
 * Date: 2016-7-30
 */
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenIZ.Core.Services;
using System.Collections;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Security;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Caching
{
    /// <summary>
    /// Memory cache functions
    /// </summary>
    public class MemoryCache : IDisposable
    {

        // Entry table for the cache
        private Dictionary<Type, Dictionary<Guid, CacheEntry>> m_entryTable = new Dictionary<Type, Dictionary<Guid, CacheEntry>>();

        // True if the object is disposed
        private bool m_disposed = false;

        // The lockbox used by the cache to ensure entries aren't written at the same time
        private object m_lock = new object();

        // Configuration of the ccache
        private ApplicationConfigurationSection m_configuration = ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>();

        // Tracer for logging
        private Tracer m_tracer = Tracer.GetTracer(typeof(MemoryCache));

        // Current singleton
        private static MemoryCache s_current = null;

        // Lockbox for singleton creation
        private static object s_lock = new object();

        // Thread pool for cleanup tasks
        private IThreadPoolService m_taskPool = ApplicationContext.Current.GetService<IThreadPoolService>();

        // Minimum age of a cache item (helps when large queries are returned)
        private long m_minAgeTicks = new TimeSpan(0, 0, 30).Ticks;


        /// <summary>
        /// Bind types
        /// </summary>
        private Type[] bindTypes = 
        {
            typeof(Concept),
            typeof(AssigningAuthority),
            typeof(Place),
            typeof(ConceptSet),
            typeof(ConceptName),
            typeof(SecurityUser),
            typeof(UserEntity)
        };

        /// <summary>
        /// Memory cache configuration
        /// </summary>
        private MemoryCache()
        {
            m_tracer.TraceInfo("Binding initial collections...");

            foreach (var t in this.bindTypes)
            {
                this.RegisterCacheType(t);
                
            }
        }

        /// <summary>
        /// Gets the current singleton
        /// </summary>
        public static MemoryCache Current
        {
            get
            {
                if (s_current == null)
                    lock (s_lock)
                        if (s_current == null)
                            s_current = new MemoryCache();
                return s_current;
            }
        }

        /// <summary>
        /// Gets the size of the current cache
        /// </summary>
        public int GetSize(Type t)
        {
            this.ThrowIfDisposed();

            Dictionary<Guid, CacheEntry> cache = null;
            if (this.m_entryTable.TryGetValue(t, out cache))
                return cache.Count;
            else
                return 0;

        }

        /// <summary>
        /// Adds an entry or updates it
        /// </summary>
        public void AddUpdateEntry(object data)
        {
            
            // Throw if disposed
            this.ThrowIfDisposed();

            Type objData = data?.GetType();
            var idData = data as IIdentifiedEntity;
            if (idData == null || !idData.Key.HasValue)
                return;

            Dictionary<Guid, CacheEntry> cache = null;
            if (this.m_entryTable.TryGetValue(objData, out cache))
            {
                Guid key = idData?.Key ?? Guid.Empty;
                CacheEntry entry = null;

                if (cache.TryGetValue(key, out entry))
                    lock (this.m_lock)
                    {
#if PERFMON
                        this.m_tracer.TraceVerbose("Update cache object ({0}) - {1} [@{2}]", objData, data, data.GetHashCode());
#endif
                        entry.Update(data as IdentifiedData);
                    }
                else
                    lock (this.m_lock)
                        if (!cache.ContainsKey(key))
                        {
#if PERFMON
                            this.m_tracer.TraceVerbose("Add cache object ({0}) - {1} [@{2}]", objData, data, data.GetHashCode());
#endif
                            cache.Add(key, new CacheEntry(DateTime.Now, data as IdentifiedData));
                            this.m_tracer.TraceVerbose("Cache {0} is now {1} large", objData, cache.Count);

                        }
            }
            else //if(data.GetType().GetTypeInfo().GetCustomAttribute<XmlRootAttribute>() != null) // only cache root elements
                this.RegisterCacheType(data.GetType());

        }

        /// <summary>
        /// Remove the specified object from the cache
        /// </summary>
        public void RemoveObject(Type objectType, Guid? key)
        {
            this.ThrowIfDisposed();

            if (!key.HasValue) return;
            else if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            Dictionary<Guid, CacheEntry> cache = null;
            if (this.m_entryTable.TryGetValue(objectType, out cache))
            {
                CacheEntry candidate = default(CacheEntry);
                if (cache.TryGetValue(key.Value, out candidate))
                {
                    lock (this.m_lock)
                    {
#if PERFMON
                        this.m_tracer.TraceVerbose("Remove cache object ({0}) - {1}", objectType, key);
#endif

                        cache.Remove(key.Value);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Try to get an entry from the cache returning null if not found
        /// </summary>
        public object TryGetEntry(Type objectType, Guid? key)
        {
            this.ThrowIfDisposed();

#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            if (!key.HasValue) return null;
            else if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            Dictionary<Guid, CacheEntry> cache = null;
            if(this.m_entryTable.TryGetValue(objectType, out cache))
            {
                CacheEntry candidate = default(CacheEntry);
                if (cache.TryGetValue(key.Value, out candidate))
                {
                    candidate.Touch();
#if PERFMON
                    this.m_tracer.TraceVerbose("Retrieved cache entry ({0}) {1}", objectType, candidate);
                    sw.Stop();
                    this.m_tracer.TraceVerbose("PERF: TryGetEntry HIT {0} ({1} ms)", key, sw.ElapsedMilliseconds);
#endif
                    return candidate.Data;
                }
                else /// try get entry slow
                {
                    return this.m_entryTable.Values.SelectMany(o => o.Values).FirstOrDefault(o => o.Data.Key == key);
                }
            }

#if PERFMON
            sw.Stop();
            this.m_tracer.TraceVerbose("PERF: TryGetEntry MISS {0} ({1} ms)", key, sw.ElapsedMilliseconds);
#endif
            return null;
        }

        /// <summary>
        /// Sets the minimum age
        /// </summary>
        /// <param name="age"></param>
        public  void SetMinAge(TimeSpan age)
        {
            this.ThrowIfDisposed();

            this.m_minAgeTicks = age.Ticks;
        }

        /// <summary>
        /// If a cache is reaching its maximum entry level clean some space 
        /// </summary>
        public void ReducePressure()
        {
            this.ThrowIfDisposed();
            

            // Entry table clean
            try {
                if (!Monitor.TryEnter(this.m_lock))
                    return; // Something else is locking the process

                this.m_tracer.TraceInfo("Starting memory cache pressure reduction...");
                var nowTicks = DateTime.Now.Ticks;

                foreach (var itm in this.m_entryTable)
                {
                    int maxSize = this.m_configuration.Cache.MaxSize;
                    var garbageBin = itm.Value.AsParallel().OrderByDescending(o => o.Value.LastUpdateTime).Take(itm.Value.Count - maxSize).Where(o => (nowTicks - o.Value.LastUpdateTime) >= this.m_minAgeTicks).Select(o => o.Key);

                    if (garbageBin.Count() > 0)
                    {
                        this.m_tracer.TraceInfo("Cache {0} overcommitted by {1} will remove entries older than min age..", garbageBin.Count(), itm.Key.FullName);

                        this.m_taskPool.QueueUserWorkItem((o) =>
                        {
                            IEnumerable<Guid> gc = o as IEnumerable<Guid>;
                            foreach (var g in gc)
                                lock (this.m_lock)
                                    itm.Value.Remove(g);
                        }, garbageBin);
                    }
                }
            }
            finally
            {
                Monitor.Exit(this.m_lock);
            }
        }

        /// <summary>
        /// Clears all caches
        /// </summary>
        public void Clear()
        {
            this.ThrowIfDisposed();

            foreach (var itm in this.m_entryTable)
                lock (this.m_lock)
                    itm.Value.Clear();
        }


        /// <summary>
        /// Clean the cache of old entries
        /// </summary>
        public void Clean()
        {
            this.ThrowIfDisposed();

           
            long nowTicks = DateTime.Now.Ticks;
            try// This time wait for a lock
            {
                if (!Monitor.TryEnter(this.m_lock))
                    return;

                this.m_tracer.TraceInfo("Starting memory cache deep clean...");

                // Entry table clean
                foreach (var itm in this.m_entryTable)
                {
                    // Clean old data
                    var garbageBin = itm.Value.AsParallel().Where(o => nowTicks - o.Value.LastUpdateTime > this.m_configuration.Cache.MaxAge).Select(o => o.Key);
                    if (garbageBin.Count() > 0)
                    {
                        this.m_tracer.TraceInfo("Will clean {0} stale entries from cache {1}..", garbageBin.Count(), itm.Key.FullName);
                        this.m_taskPool.QueueUserWorkItem((o) =>
                        {
                            IEnumerable<Guid> gc = o as IEnumerable<Guid>;
                            foreach (var g in gc)
                                lock (this.m_lock)
                                    itm.Value.Remove(g);
                        }, garbageBin);
                    }

                }
            }
            finally
            {
                Monitor.Exit(this.m_lock);
            }
        }

        /// <summary>
        /// Register caching type
        /// </summary>
        public void RegisterCacheType(Type t, int maxSize = 50, long maxAge = 0x23C34600)
        {

            this.ThrowIfDisposed();

            // Lock the master cache 
            Dictionary<Guid, CacheEntry> cache = null;
            lock (this.m_lock)
            {
                if (!this.m_entryTable.TryGetValue(t, out cache))
                {
                    cache = new Dictionary<Guid, CacheEntry>(10);
                    this.m_entryTable.Add(t, cache);
                }
                else
                    return;
            }

            // We want to subscribe when this object is changed so we can keep the cache fresh
            var idpType = typeof(IDataPersistenceService<>).MakeGenericType(t);
            var ppeArgType = typeof(DataPersistenceEventArgs<>).MakeGenericType(t);
            var pqeArgType = typeof(DataQueryEventArgsBase<>).MakeGenericType(t);
            var evtHdlrType = typeof(EventHandler<>).MakeGenericType(ppeArgType);
            var qevtHdlrType = typeof(EventHandler<>).MakeGenericType(pqeArgType);
            var svcInstance = ApplicationContext.Current.GetService(idpType);

            if (svcInstance != null)
            {
                // Construct the delegate
                var senderParm = Expression.Parameter(typeof(Object), "o");
                var eventParm = Expression.Parameter(ppeArgType, "e");
                var eventData = Expression.MakeMemberAccess(eventParm, ppeArgType.GetRuntimeProperty("Data"));
                var insertInstanceDelegate = Expression.Lambda(evtHdlrType, Expression.Call(Expression.Constant(this), typeof(MemoryCache).GetRuntimeMethod("HandlePostPersistenceEvent", new Type[] { typeof(Object) }), eventData), senderParm, eventParm).Compile();
                var updateInstanceDelegate = Expression.Lambda(evtHdlrType, Expression.Call(Expression.Constant(this), typeof(MemoryCache).GetRuntimeMethod("HandlePostPersistenceEvent", new Type[] { typeof(Object) }), eventData), senderParm, eventParm).Compile();
                var obsoleteInstanceDelegate = Expression.Lambda(evtHdlrType, Expression.Call(Expression.Constant(this), typeof(MemoryCache).GetRuntimeMethod("HandlePostPersistenceEvent", new Type[] { typeof(Object) }), eventData), senderParm, eventParm).Compile();

                eventParm = Expression.Parameter(pqeArgType, "e");
                var queryEventData = Expression.Convert(Expression.MakeMemberAccess(eventParm, pqeArgType.GetRuntimeProperty("Results")), typeof(IEnumerable));
                var queryInstanceDelegate = Expression.Lambda(qevtHdlrType, Expression.Call(Expression.Constant(this), typeof(MemoryCache).GetRuntimeMethod("HandlePostQueryEvent", new Type[] { typeof(IEnumerable) }), queryEventData), senderParm, eventParm).Compile();

                // Bind to events
                idpType.GetRuntimeEvent("Inserted").AddEventHandler(svcInstance, insertInstanceDelegate);
                idpType.GetRuntimeEvent("Updated").AddEventHandler(svcInstance, updateInstanceDelegate);
                idpType.GetRuntimeEvent("Obsoleted").AddEventHandler(svcInstance, obsoleteInstanceDelegate);
                idpType.GetRuntimeEvent("Queried").AddEventHandler(svcInstance, queryInstanceDelegate);
            }


        }

        /// <summary>
        /// Object is disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.m_disposed)
                throw new ObjectDisposedException(nameof(MemoryCache));
        }

        /// <summary>
        /// Handle post query event
        /// </summary>
        public void HandlePostQueryEvent(IEnumerable results)
        {
            foreach (var data in results)
            {
                this.AddUpdateEntry(data);
            }
        }

        /// <summary>
        /// Persistence event handler
        /// </summary>
        public void HandlePostPersistenceEvent(Object data)
        {
            // Bundles are special cases.
            if (data is Bundle)
            {
                foreach (var itm in (data as Bundle).Item)
                    HandlePostPersistenceEvent(itm);
            }
            else
            {
                this.AddUpdateEntry(data);
                ////this.RemoveObject(data.GetType(), (data as IIdentifiedEntity).Key.Value);
                //var idData = data as IIdentifiedEntity;
                //var objData = data.GetType();

                //Dictionary<Guid, CacheEntry> cache = null;
                //if (this.m_entryTable.TryGetValue(objData, out cache))
                //{
                //    Guid key = idData?.Key ?? Guid.Empty;
                //    if (cache.ContainsKey(key))
                //        lock (this.m_lock)
                //        {
                //            cache[key].Update(data);
                //        }
                //    //cache.Remove(key);
                //}
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {

            this.m_disposed = true;
        }

        //public void Update(object data)
        //{
        //    var properties = data.GetType().GetRuntimeProperties();

        //    foreach (var item in properties)
        //    {
        //        var value = item.GetValue(data);
                
        //        if (value is IList && (value as IList).Count > 0)
        //        {
        //            foreach (var listItem in value as IList)
        //            {
        //                this.Update(listItem);

        //                if (listItem is IdentifiedData)
        //                {
        //                    var identifiedData = listItem as IdentifiedData;

        //                    // if there is a key and the key is not an empty GUID
        //                    if (identifiedData.Key.HasValue && identifiedData.Key.Value != Guid.Empty)
        //                    {
        //                        this.RemoveObject(listItem.GetType(), identifiedData.Key.Value);
        //                        this.AddUpdateEntry(listItem);
        //                    }
        //                }
        //            }
        //        }
        //        if (value is IdentifiedData)
        //        {
        //            var identifiedData = value as IdentifiedData;

        //            // if there is a key and the key is not an empty GUID
        //            if (identifiedData.Key.HasValue && identifiedData.Key.Value != Guid.Empty)
        //            {
        //                this.RemoveObject(value.GetType(), identifiedData.Key.Value);
        //                this.AddUpdateEntry(value);
        //            }
        //        }
        //    }
        //}
    }
}
