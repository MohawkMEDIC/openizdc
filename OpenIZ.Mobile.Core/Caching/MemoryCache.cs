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
using OpenIZ.Core.Model.Acts;

namespace OpenIZ.Mobile.Core.Caching
{
    /// <summary>
    /// Memory cache functions
    /// </summary>
    public class MemoryCache : IDisposable
    {

        // Entry table for the cache
        private Dictionary<Guid, CacheEntry> m_entryTable = new Dictionary<Guid, CacheEntry>();

        // Subscribed types
        private HashSet<Type> m_subscribedTypes = new HashSet<Type>();

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

            return this.m_entryTable.Count;

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

            CacheEntry candidate = null;
            if (this.m_entryTable.TryGetValue(idData.Key.Value, out candidate))
                candidate.Update(data as IdentifiedData);
            else
                lock (this.m_lock)
                    if (!this.m_entryTable.ContainsKey(idData.Key.Value))
                        this.m_entryTable.Add(idData.Key.Value, new CacheEntry(DateTime.Now, data as IdentifiedData));

        }

        /// <summary>
        /// Remove the specified object from the cache
        /// </summary>
        public void RemoveObject(Guid? key)
        {
            this.ThrowIfDisposed();

            if (!key.HasValue) return;

            CacheEntry candidate = null;
            if (this.m_entryTable.TryGetValue(key.Value, out candidate))
                lock (this.m_lock)
                    this.m_entryTable.Remove(key.Value);

        }

        /// <summary>
        /// Try to get an entry from the cache returning null if not found
        /// </summary>
        public object TryGetEntry(Guid? key)
        {
            this.ThrowIfDisposed();

            if (!key.HasValue) return null;

            CacheEntry candidate = null;
            if (this.m_entryTable.TryGetValue(key.Value, out candidate))
            {
                candidate.Touch();
                return candidate.Data;
            }

            return null;
        }

        /// <summary>
        /// Sets the minimum age
        /// </summary>
        /// <param name="age"></param>
        public void SetMinAge(TimeSpan age)
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
            if (Monitor.TryEnter(this.m_lock))
                try
                {

                    this.m_tracer.TraceInfo("Starting memory cache pressure reduction...");
                    var nowTicks = DateTime.Now.Ticks;


                    int maxSize = this.m_configuration.Cache.MaxSize;
                    var garbageBin = this.m_entryTable.OrderByDescending(o => o.Value.LastUpdateTime).Take(this.m_entryTable.Count - maxSize).Where(o => (nowTicks - o.Value.LastUpdateTime) >= this.m_minAgeTicks).Select(o => o.Key);

                    if (garbageBin.Count() > 0)
                    {
                        this.m_tracer.TraceInfo("Cache overcommitted by {0} will remove entries older than min age..", garbageBin.Count());

                        this.m_taskPool.QueueUserWorkItem((o) =>
                        {
                            try
                            {
                                IEnumerable<Guid> gc = o as IEnumerable<Guid>;
                                foreach (var g in gc.ToArray())
                                    lock (this.m_lock)
                                        this.m_entryTable.Remove(g);
                            }
                            catch { }
                        }, garbageBin);
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

            this.m_entryTable.Clear();
        }


        /// <summary>
        /// Clean the cache of old entries
        /// </summary>
        public void Clean()
        {
            this.ThrowIfDisposed();


            long nowTicks = DateTime.Now.Ticks;
            if (Monitor.TryEnter(this.m_lock))
                try// This time wait for a lock
                {

                    this.m_tracer.TraceInfo("Starting memory cache deep clean...");

                    // Entry table clean

                    // Clean old data
                    var garbageBin = this.m_entryTable.Where(o => nowTicks - o.Value.LastUpdateTime > this.m_configuration.Cache.MaxAge).Select(o => o.Key);
                    if (garbageBin.Count() > 0)
                    {
                        this.m_tracer.TraceInfo("Will clean {0} stale entries from cache..", garbageBin.Count());
                        this.m_taskPool.QueueUserWorkItem((o) =>
                        {
                            IEnumerable<Guid> gc = o as IEnumerable<Guid>;
                            foreach (var g in gc.ToArray())
                                lock (this.m_lock)
                                    this.m_entryTable.Remove(g);
                        }, garbageBin);
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

            if (this.m_subscribedTypes.Contains(t))
                return;
            this.m_subscribedTypes.Add(t);

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
