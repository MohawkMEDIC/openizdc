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

using System.Linq;
using System.Reflection;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model.Map;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Data.Persistence;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model;
using SQLite.Net;
using System.Collections.Generic;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Exceptions;
using System.Linq.Expressions;
using System.Collections;
using OpenIZ.Core.Services;
using System.Diagnostics;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Mobile.Core.Data
{

    
    /// <summary>
    /// Represents a dummy service which just adds the persistence services to the context
    /// </summary>
    public class LocalPersistenceService
    {

        // Cache
        private static Dictionary<Type, ILocalPersistenceService> s_persistenceCache = new Dictionary<Type, ILocalPersistenceService>();


        /// <summary>
        /// Get the specified persister type
        /// </summary>
        public static ILocalPersistenceService GetPersister(Type tDomain)
        {
            ILocalPersistenceService retVal = null;
            if (!s_persistenceCache.TryGetValue(tDomain, out retVal))
            {
                var idpType = typeof(IDataPersistenceService<>).MakeGenericType(tDomain);
                retVal = ApplicationContext.Current.GetService(idpType) as ILocalPersistenceService;
                if (retVal != null)
                    lock (s_persistenceCache)
                        if (!s_persistenceCache.ContainsKey(tDomain))
                            s_persistenceCache.Add(tDomain, retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Generic versioned persister service for any non-customized persister
        /// </summary>
        internal class GenericVersionedPersistenceService<TModel, TDomain> : VersionedDataPersistenceService<TModel, TDomain>
            where TDomain : DbVersionedData, new()
            where TModel : VersionedEntityData<TModel>, new()
        {

            /// <summary>
            /// Ensure exists
            /// </summary>
            protected override TModel InsertInternal(LocalDataContext context, TModel data)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null) continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        instance = ModelExtensions.TryGetExisting(instance as IIdentifiedEntity, context);
                        if (instance != null) rp.SetValue(data, instance);
                        ModelExtensions.UpdateParentKeys(data, rp);
                    }
                }
                return base.InsertInternal(context, data);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            protected override TModel UpdateInternal(LocalDataContext context, TModel data)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null) continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        instance = ModelExtensions.TryGetExisting(instance as IIdentifiedEntity, context);
                        if (instance != null) rp.SetValue(data, instance);
                        ModelExtensions.UpdateParentKeys(data, rp);
                    }

                }
                return base.UpdateInternal(context, data);
            }
        }

        /// <summary>
        /// Generic versioned persister service for any non-customized persister
        /// </summary>
        internal class GenericBasePersistenceService<TModel, TDomain> : BaseDataPersistenceService<TModel, TDomain>
            where TDomain : DbBaseData, new()
            where TModel : BaseEntityData, new()
        {

            /// <summary>
            /// Ensure exists
            /// </summary>
            protected override TModel InsertInternal(LocalDataContext context, TModel data)
            {
                if (data.IsEmpty()) return data;

                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null) continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        instance = ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        if (instance != null) rp.SetValue(data, instance);
                        ModelExtensions.UpdateParentKeys(data, rp);
                    }
                }
                return base.InsertInternal(context, data);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            protected override TModel UpdateInternal(LocalDataContext context, TModel data)
            {
                if (data.IsEmpty()) return data;

                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null) continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        if (instance != null) rp.SetValue(data, instance);
                        ModelExtensions.UpdateParentKeys(data, rp);
                    }

                }
                return base.UpdateInternal(context, data);
            }
        }

        /// <summary>
        /// Generic versioned persister service for any non-customized persister
        /// </summary>
        internal class GenericIdentityPersistenceService<TModel, TDomain> : IdentifiedPersistenceService<TModel, TDomain>
            where TModel : IdentifiedData, new()
            where TDomain : DbIdentified, new()
        {
            /// <summary>
            /// Ensure exists
            /// </summary>
            protected override TModel InsertInternal(LocalDataContext context, TModel data)
            {
                if (data.IsEmpty()) return data;

                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null) continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        instance = ModelExtensions.TryGetExisting(instance as IIdentifiedEntity, context);
                        if (instance != null) rp.SetValue(data, instance);

                        ModelExtensions.UpdateParentKeys(data, rp);
                    }

                }
                return base.InsertInternal(context, data);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            protected override TModel UpdateInternal(LocalDataContext context, TModel data)
            {
                if (data.IsEmpty()) return data;

                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null) continue;

                    var instance = rp.GetValue(data);
                    if (instance != null && rp.Name != "SourceEntity") // HACK: Prevent infinite loops on associtive entities
                    {
                        instance = ModelExtensions.TryGetExisting(instance as IIdentifiedEntity, context);
                        if (instance != null) rp.SetValue(data, instance);
                        ModelExtensions.UpdateParentKeys(data, rp);
                    }

                }
                return base.UpdateInternal(context, data);
            }
        }

        /// <summary>
        /// Generic association persistence service
        /// </summary>
        internal class GenericIdentityAssociationPersistenceService<TModel, TDomain> :
            GenericIdentityPersistenceService<TModel, TDomain>, ILocalAssociativePersistenceService
            where TModel : IdentifiedData, ISimpleAssociation, new()
            where TDomain : DbIdentified, new()
        {
            /// <summary>
            /// Get all the matching TModel object from source
            /// </summary>
            public IEnumerable GetFromSource(LocalDataContext context, Guid sourceId, decimal? versionSequenceId)
            {
                int tr = 0;
                return this.Query(context, o => o.SourceEntityKey == sourceId, 0, 100, out tr, Guid.Empty, false);
            }
        }

        // Mapper
        protected static ModelMapper m_mapper;

        // Static CTOR
        public static ModelMapper Mapper
        {
            get
            {
                if (m_mapper == null)
                {
                    var tracer = Tracer.GetTracer(typeof(LocalPersistenceService));
                    try
                    {
                        m_mapper = new ModelMapper(typeof(LocalPersistenceService).GetTypeInfo().Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Data.Map.ModelMap.xml"));
                    }
                    catch (ModelMapValidationException ex)
                    {
                        tracer.TraceError("Error validating model map: {0}", ex);
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        tracer.TraceError("Error initializing persistence: {0}", ex);
                        throw ex;
                    }
                }
                return m_mapper;
            }
        }

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalPersistenceService));

        // Constructor
        public LocalPersistenceService()
        {
            var appSection = ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>();

            this.m_tracer.TraceInfo("Starting local persistence services...");
            // Iterate the persistence services
            foreach (var t in typeof(LocalPersistenceService).GetTypeInfo().Assembly.ExportedTypes.Where(o => o.Namespace == "OpenIZ.Mobile.Core.Data.Persistence" && !o.GetTypeInfo().IsAbstract && !o.GetTypeInfo().IsGenericTypeDefinition))
            {
                try
                {
                    this.m_tracer.TraceVerbose("Loading {0}...", t.AssemblyQualifiedName);
                    appSection.Services.Add(Activator.CreateInstance(t));
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error adding service {0} : {1}", t.AssemblyQualifiedName, e);
                }
            }

            // Now iterate through the map file and ensure we have all the mappings, if a class does not exist create it
            try
            {
                this.m_tracer.TraceVerbose("Creating secondary model maps...");

                var map = ModelMap.Load(typeof(LocalPersistenceService).GetTypeInfo().Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Data.Map.ModelMap.xml"));
                foreach (var itm in map.Class)
                {
                    // Is there a persistence service?
                    var idpType = typeof(IDataPersistenceService<>);
                    Type modelClassType = Type.GetType(itm.ModelClass),
                        domainClassType = Type.GetType(itm.DomainClass);
                    idpType = idpType.MakeGenericType(modelClassType);


                    // Already created
                    if (appSection.Services.Any(o => idpType.GetTypeInfo().IsAssignableFrom(o.GetType().GetTypeInfo())))
                        continue;

                    this.m_tracer.TraceVerbose("Creating map {0} > {1}", modelClassType, domainClassType);

                    // Is the model class a Versioned entity?
                    if (modelClassType.GetRuntimeProperty("VersionKey") != null &&
                        typeof(DbVersionedData).GetTypeInfo().IsAssignableFrom(domainClassType.GetTypeInfo()))
                    {
                        // Construct a type
                        var pclass = typeof(GenericVersionedPersistenceService<,>);
                        pclass = pclass.MakeGenericType(modelClassType, domainClassType);
                        appSection.Services.Add(Activator.CreateInstance(pclass));
                    }
                    else if (modelClassType.GetRuntimeProperty("CreatedByKey") != null &&
                        typeof(DbBaseData).GetTypeInfo().IsAssignableFrom(domainClassType.GetTypeInfo()))
                    {
                        // Construct a type
                        var pclass = typeof(GenericBasePersistenceService<,>);
                        pclass = pclass.MakeGenericType(modelClassType, domainClassType);
                        appSection.Services.Add(Activator.CreateInstance(pclass));
                    }
                    else
                    {
                        // Construct a type
                        Type pclass = null;
                        if (modelClassType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IVersionedAssociation)))
                            pclass = typeof(GenericIdentityAssociationPersistenceService<,>);
                        else if (modelClassType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ISimpleAssociation)))
                            pclass = typeof(GenericIdentityAssociationPersistenceService<,>);
                        else
                            pclass = typeof(GenericIdentityPersistenceService<,>);
                        pclass = pclass.MakeGenericType(modelClassType, domainClassType);
                        appSection.Services.Add(Activator.CreateInstance(pclass));
                    }

                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error initializing local persistence: {0}", e);
                throw e;
            }
        }
    }
}

