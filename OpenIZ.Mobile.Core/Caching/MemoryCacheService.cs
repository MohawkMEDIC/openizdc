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
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Caching
{
	/// <summary>
	/// Memory cache service
	/// </summary>
	public class MemoryCacheService : IDataCachingService, IDaemonService
	{

		/// <summary>
		/// Cache of data
		/// </summary>
		private EventHandler<ModelMapEventArgs> m_mappingHandler = null;
		private EventHandler<ModelMapEventArgs> m_mappedHandler = null;

		private object lockInstance = new object();

		// Memory cache configuration
		private Tracer m_tracer = Tracer.GetTracer(typeof(MemoryCacheService));

		/// <summary>
		/// True when the memory cache is running
		/// </summary>
		public bool IsRunning
		{
			get
			{
				return this.m_mappedHandler != null && m_mappedHandler != null;
			}
		}

		/// <summary>
		/// Service is starting
		/// </summary>
		public event EventHandler Started;
		/// <summary>
		/// Service has started
		/// </summary>
		public event EventHandler Starting;
		/// <summary>
		/// Service is stopping
		/// </summary>
		public event EventHandler Stopped;
		/// <summary>
		/// Service has stopped
		/// </summary>
		public event EventHandler Stopping;
        public event EventHandler<DataCacheEventArgs> Added;
        public event EventHandler<DataCacheEventArgs> Updated;
        public event EventHandler<DataCacheEventArgs> Removed;

        /// <summary>
        /// Start the service
        /// </summary>
        /// <returns></returns>
        public bool Start()
		{
			this.m_tracer.TraceInfo("Starting Memory Caching Service...");

			this.Starting?.Invoke(this, EventArgs.Empty);

            // subscribe to events
            this.Added += (o, e) => this.EnsureCacheConsistency(e);
            this.Updated += (o, e) => this.EnsureCacheConsistency(e);
            this.Removed += (o, e) => this.EnsureCacheConsistency(e, true);

            // Initialization parameters - Load concept dictionary
            ApplicationContext.Current.Started += (os, es) => ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem((a) =>
			 {

				 // Seed the cache
				 try
				 {
					 this.m_tracer.TraceInfo("Loading concept dictionary ...");
					 //ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>().Query(q => q.StatusConceptKey == StatusKeys.Active);
					 //ApplicationContext.Current.GetService<IDataPersistenceService<IdentifierType>>().Query(q => true);
					 //ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>().Query(q => true);

                     foreach(var i in ApplicationContext.Current.Configuration.GetSection<SynchronizationConfigurationSection>().Facilities)
                        ApplicationContext.Current.GetService<IDataPersistenceService<Place>>().Get(Guid.Parse(i));

                     // Seed cache
                     this.m_tracer.TraceInfo("Loading materials dictionary...");
					 ApplicationContext.Current.GetService<IDataPersistenceService<Material>>().Query(q => q.StatusConceptKey == StatusKeys.Active);
					 ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>().Query(q => q.StatusConceptKey == StatusKeys.Active);

					 // handles when a item is being mapped
					 if (this.m_mappingHandler == null)
					 {
						 this.m_mappingHandler = (o, e) =>
						 {
							 var obj = MemoryCache.Current.TryGetEntry(e.Key);
							 if (obj != null)
							 {
								 var cVer = obj as IVersionedEntity;
								 var dVer = e.ModelObject as IVersionedEntity;
								 if (cVer?.VersionSequence <= dVer?.VersionSequence) // Cache is older than this item
								 {
									 e.ModelObject = obj as IdentifiedData;
									 e.Cancel = true;
								 }
							 }
							 //this.GetOrUpdateCacheItem(e);
						 };

						 // Handles when an item is no longer being mapped
						 this.m_mappedHandler = (o, e) =>
						 {
							 //MemoryCache.Current.RegisterCacheType(e.ObjectType);
							 //this.GetOrUpdateCacheItem(e);
						 };

						 // Subscribe to message mapping
						 ModelMapper.MappingToModel += this.m_mappingHandler;
						 ModelMapper.MappedToModel += this.m_mappedHandler;
                         
						 // Now start the clean timers
						 this.m_tracer.TraceInfo("Starting clean timers...");

						 Action<Object> cleanProcess = null, pressureProcess = null;

						 cleanProcess = o =>
						 {
							 MemoryCache.Current.Clean();
							 ApplicationContext.Current.GetService<IThreadPoolService>()?.QueueUserWorkItem(new TimeSpan(ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Cache.MaxDirtyAge), cleanProcess, null);
						 };
						 pressureProcess = o =>
						 {
							 MemoryCache.Current.ReducePressure();
							 ApplicationContext.Current.GetService<IThreadPoolService>()?.QueueUserWorkItem(new TimeSpan(ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Cache.MaxDirtyAge), pressureProcess, null);
						 };

						 // Register processes on a delay
						 ApplicationContext.Current.GetService<IThreadPoolService>()?.QueueUserWorkItem(new TimeSpan(ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Cache.MaxDirtyAge), pressureProcess, null);
						 ApplicationContext.Current.GetService<IThreadPoolService>()?.QueueUserWorkItem(new TimeSpan(ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Cache.MaxDirtyAge), cleanProcess, null);

					 }
				 }
				 catch
				 {
					 this.m_tracer.TraceWarning("Caching will be disabled due to cache load error");
				 }

			 });
			// Now we start timers
			this.Started?.Invoke(this, EventArgs.Empty);
			return true;
		}

        /// <summary>
        /// Ensure cache consistency
        /// </summary>
        private void EnsureCacheConsistency(DataCacheEventArgs e, bool remove=false)
        {

            //// Relationships should always be clean of source/target so the source/target will load the new relationship
            if (e.Object is ActParticipation)
            {
	            lock (lockInstance)
	            {
					var ptcpt = (e.Object as ActParticipation);
		            var sourceEntity = MemoryCache.Current.TryGetEntry(ptcpt.SourceEntityKey) as Act;
		            var targetEntity = MemoryCache.Current.TryGetEntry(ptcpt.PlayerEntityKey) as Entity;

		            if (sourceEntity != null) // search and replace
		            {
			            var idx = sourceEntity.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ptcpt.ParticipationRoleKey &&
																							o.ActKey == ptcpt.ActKey && o.PlayerEntityKey == ptcpt.PlayerEntityKey);
			            if (idx != null)
				            sourceEntity.Participations.Remove(idx);

			            if (!remove)
				            sourceEntity.Participations.Add(ptcpt);
		            }
		            if (targetEntity != null)
		            {
			            var idx = targetEntity.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ptcpt.ParticipationRoleKey &&
																				o.ActKey == ptcpt.ActKey && o.PlayerEntityKey == ptcpt.PlayerEntityKey);
			            if (idx != null)
			            {
				            targetEntity.Participations.Remove(idx);
				            if (!remove)
					            targetEntity.Participations.Add(ptcpt);
			            }
		            }
				}
            }
            else if (e.Object is ActRelationship)
            {
                var rel = (e.Object as ActRelationship);
                var sourceEntity = MemoryCache.Current.TryGetEntry(rel.SourceEntityKey) as Act;
                var targetEntity = MemoryCache.Current.TryGetEntry(rel.TargetActKey) as Act;

                if (sourceEntity != null) // search and replace
                {
                    var idx = sourceEntity.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == rel.RelationshipTypeKey &&
                        o.SourceEntityKey == rel.SourceEntityKey && o.TargetActKey == rel.TargetActKey);
                    if (idx != null)
                        sourceEntity.Relationships.Remove(idx);
                    if (!remove)
                        sourceEntity.Relationships.Add(rel);
                }
                if (targetEntity != null)
                {
                    var idx = targetEntity.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == rel.RelationshipTypeKey &&
                        o.SourceEntityKey == rel.SourceEntityKey && o.TargetActKey == rel.TargetActKey);
                    if (idx != null)
                    {
                        targetEntity.Relationships.Remove(idx);
                        if (!remove)
                            targetEntity.Relationships.Add(rel);
                    }
                }
            }
            else if (e.Object is EntityRelationship)
            {
                var rel = (e.Object as EntityRelationship);
                var sourceEntity = MemoryCache.Current.TryGetEntry(rel.SourceEntityKey) as Entity;
                var targetEntity = MemoryCache.Current.TryGetEntry(rel.TargetEntityKey) as Entity;

                if (sourceEntity != null) // search and replace
                {
                    var idx = sourceEntity.Relationships.ToArray().FirstOrDefault(o => o.RelationshipTypeKey == rel.RelationshipTypeKey &&
                        o.SourceEntityKey == rel.SourceEntityKey && o.TargetEntityKey == rel.TargetEntityKey);
                    if (idx != null)
                        sourceEntity.Relationships.Remove(idx);
                    if (!remove)
                        sourceEntity.Relationships.Add(rel);
                }
                if (targetEntity != null)
                {
                    var idx = targetEntity.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == rel.RelationshipTypeKey &&
                        o.SourceEntityKey == rel.SourceEntityKey && o.TargetEntityKey == rel.TargetEntityKey);
                    if (idx != null)
                    {
                        targetEntity.Relationships.Remove(idx);
                        if (!remove)
                            targetEntity.Relationships.Add(rel);
                    }
                }
            }
            else if (e.Object is Act) // We need to remove RCT 
            {
                var act = e.Object as Act;
                var rct = act.Participations.ToArray().FirstOrDefault(x => x.ParticipationRoleKey == ActParticipationKey.RecordTarget || x.ParticipationRole?.Mnemonic == "RecordTarget");
                if (rct != null)
                    MemoryCache.Current.RemoveObject( rct.PlayerEntityKey);
            }

        }

        /// <summary>
        /// Either gets or updates the existing cache item
        /// </summary>
        /// <param name="e"></param>
        private void GetOrUpdateCacheItem(ModelMapEventArgs e)
		{
			var cacheItem = MemoryCache.Current.TryGetEntry(e.Key);
			if (cacheItem == null)
				MemoryCache.Current.AddUpdateEntry(e.ModelObject);
			else
			{
				// Obsolete?
				var cVer = cacheItem as IVersionedEntity;
				var dVer = e.ModelObject as IVersionedEntity;
				if (cVer?.VersionSequence < dVer?.VersionSequence) // Cache is older than this item
					MemoryCache.Current.AddUpdateEntry(dVer);
				e.ModelObject = cacheItem as IdentifiedData;
				e.Cancel = true;
			}
		}

		/// <summary>
		/// Stopping
		/// </summary>
		/// <returns></returns>
		public bool Stop()
		{
			this.Stopping?.Invoke(this, EventArgs.Empty);

			ModelMapper.MappingToModel -= this.m_mappingHandler;
			ModelMapper.MappedToModel -= this.m_mappedHandler;

			this.m_mappingHandler = null;
			this.m_mappedHandler = null;

            MemoryCache.Current.Clear();

			this.Stopped?.Invoke(this, EventArgs.Empty);
			return true;
		}

		/// <summary>
		/// Gets the specified cache item
		/// </summary>
		/// <returns></returns>
		public TData GetCacheItem<TData>(Guid key) where TData : IdentifiedData
		{
			return MemoryCache.Current.TryGetEntry(key) as TData;
		}

		/// <summary>
		/// Gets the specified cache item
		/// </summary>
		/// <returns></returns>
		public Object GetCacheItem(Guid key)
		{
			return MemoryCache.Current.TryGetEntry(key);
		}


        /// <summary>
        /// Add the specified item to the memory cache
        /// </summary>
        public void Add(IdentifiedData data)
        {
            var exist = MemoryCache.Current.TryGetEntry(data.Key);
            MemoryCache.Current.AddUpdateEntry(data);
            if (exist != null)
                this.Updated?.Invoke(this, new DataCacheEventArgs(data));
            else
                this.Added?.Invoke(this, new DataCacheEventArgs(data));
        }

        /// <summary>
        /// Remove the object from the cache
        /// </summary>
        public void Remove(Guid key)
        {
            var exist = MemoryCache.Current.TryGetEntry(key);
            if (exist != null)
            {
                MemoryCache.Current.RemoveObject(key);
                this.Removed?.Invoke(this, new DataCacheEventArgs(exist));
            }
        }
    }
}
