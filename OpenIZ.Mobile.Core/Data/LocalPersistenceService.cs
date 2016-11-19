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
    /// Ensure the data exists
    /// </summary>
    public static class ModelExtensions
    {
        // Lock 
        private static Object s_lock = new object();

        private static Tracer s_tracer = Tracer.GetTracer(typeof(ModelExtensions));
        private static Dictionary<Type, Object> s_idpInstances = new Dictionary<Type, object>(20);
        //private static Dictionary<String, Guid> s_conceptDictionary = new Dictionary<string, Guid>(20);

        /// <summary>
        /// Load specified associations
        /// </summary>
        public static void LoadAssociations<TModel>(this TModel me, SQLiteConnectionWithLock context) where TModel : IIdentifiedEntity
        {

            if (me == null)
                throw new ArgumentNullException(nameof(me));
            else if (context.IsInTransaction) return;

#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            // Load associations
            foreach (var pi in me.GetType().GetRuntimeProperties())
            {
                if (pi.GetCustomAttribute<DataIgnoreAttribute>() != null ||
                    pi.GetCustomAttribute<AutoLoadAttribute>() == null)
                    continue;

                var value = pi.GetValue(me);
                if (value == null)
                {
                    var keyProperty = pi.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty;
                    if (keyProperty == null) continue;
                    var keyValue = me.GetType().GetRuntimeProperty(keyProperty)?.GetValue(me);
                    if (keyValue == null) continue; // no key


                    var idpType = typeof(IDataPersistenceService<>).MakeGenericType(pi.PropertyType);
                    var idpInstance = ApplicationContext.Current.GetService(idpType);
                    var getMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Get" && o.GetParameters().Length == 2 && o.GetParameters()[0].ParameterType == typeof(SQLiteConnectionWithLock));
                    if (getMethod != null) 
                        pi.SetValue(me, getMethod.Invoke(idpInstance, new object[] { context, keyValue }) as IIdentifiedEntity);
                }
                else if (value is IdentifiedData)
                    pi.SetValue(me, TryGetExisting(value as IIdentifiedEntity, context));
                else if (value is IList)
                {
                    if ((value as IList).Count > 0) continue;

                    Type entryType = pi.PropertyType.GetTypeInfo().GenericTypeArguments[0],
                        predicateType = typeof(Func<,>).MakeGenericType(entryType, typeof(bool));

                    if (!typeof(ISimpleAssociation).GetTypeInfo().IsAssignableFrom(entryType.GetTypeInfo()))
                        continue;

                    ParameterExpression parameterExpr = Expression.Parameter(entryType, "o");
                    var memberExpr = Expression.MakeMemberAccess(parameterExpr, entryType.GetRuntimeProperty("SourceEntityKey"));
                    var lambda = Expression.Lambda(predicateType, Expression.MakeBinary(ExpressionType.Equal, memberExpr, Expression.Convert(Expression.Constant(me.Key), typeof(Guid?))), parameterExpr);

                    // Get the query method
                    var idpType = typeof(IDataPersistenceService<>).MakeGenericType(entryType);
                    var idpInstance = ApplicationContext.Current.GetService(idpType);
                    if (idpInstance == null) continue;
                    var queryMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Query" && o.GetParameters().Length == 2 && o.GetParameters()[0].ParameterType == typeof(SQLiteConnectionWithLock));

                    // Execute query
                    if (queryMethod == null) continue;
                    else
                    {
                        var results = queryMethod.Invoke(idpInstance, new object[] { context, lambda }) as IEnumerable;
                        var lValue = Activator.CreateInstance(pi.PropertyType) as IList;
                        pi.SetValue(me, lValue);
                        foreach (var itm in results)
                        {
                            (itm as IIdentifiedEntity).LoadAssociations(context);
                            lValue.Add(itm);
                        }
                    }
                }
            }

#if PERFMON
            sw.Stop();
            s_tracer.TraceVerbose("PERF: LoadAssociations ({0} ms)", sw.ElapsedMilliseconds);
#endif
        }

        /// <summary>
        /// Try get by classifier
        /// </summary>
        public static IIdentifiedEntity TryGetExisting(this IIdentifiedEntity me, SQLiteConnectionWithLock context)
        {

#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            IIdentifiedEntity existing = null;

            // First, is the object a concept?
            Guid meKey = Guid.Empty;
            //Concept conceptMe = me as Concept;
            //if (conceptMe != null && s_conceptDictionary.TryGetValue(conceptMe.Mnemonic, out meKey))
            //    me.Key = meKey;

            // Have we already loaded the data provider?
            object idpInstance = null;
            if (!s_idpInstances.TryGetValue(me.GetType(), out idpInstance))
            {
                lock (s_lock)
                {
                    var idpType = typeof(IDataPersistenceService<>).MakeGenericType(me.GetType());
                    idpInstance = ApplicationContext.Current.GetService(idpType);
                    if(!s_idpInstances.ContainsKey(me.GetType()))
                        s_idpInstances.Add(me.GetType(), idpInstance);
                }
            }
            if (idpInstance == null) return null;


            // Is the key not null?
            if (me.Key != Guid.Empty && me.Key != null)
            {
#if PERFMON
                sw.Stop();
                s_tracer.TraceVerbose("Using FAST GET method on ID {0} ({1} ms)", me, sw.ElapsedMilliseconds);
                sw.Start();
#endif              
                //existing = idpInstance.Get(me.Key.Value) as IIdentifiedEntity;
                //// We have to find it
                var getMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Get" && o.GetParameters().Length == 2 && o.GetParameters()[0].ParameterType == typeof(SQLiteConnectionWithLock) );
                if (getMethod == null) return null;
                existing = getMethod.Invoke(idpInstance, new object[] { context, me.Key }) as IIdentifiedEntity;
            }

            var classAtt = me.GetType().GetTypeInfo().GetCustomAttribute<KeyLookupAttribute>();
            if (classAtt != null && existing == null)
            {
#if PERFMON
                sw.Stop();
                s_tracer.TraceVerbose("Using SLOW QUERY method on ID {0} ({1} ms)", me, sw.ElapsedMilliseconds);
                sw.Start();
#endif        
                object classifierValue = me;// me.GetType().GetProperty(classAtt.ClassifierProperty).GetValue(me);
                                            // Follow the classifier
                Type predicateType = typeof(Func<,>).MakeGenericType(me.GetType(), typeof(bool));
                ParameterExpression parameterExpr = Expression.Parameter(me.GetType(), "o");
                Expression accessExpr = parameterExpr;
                while (classAtt != null)
                {
                    var property = accessExpr.Type.GetRuntimeProperty(classAtt.UniqueProperty);
                    accessExpr = Expression.MakeMemberAccess(accessExpr, property);
                    classifierValue = property.GetValue(classifierValue);

                    classAtt = accessExpr.Type.GetTypeInfo().GetCustomAttribute<KeyLookupAttribute>();

                }

                // public abstract IQueryable<TData> Query(ModelDataContext context, Expression<Func<TData, bool>> query, IPrincipal principal);
                var queryMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Query" && o.GetParameters().Length == 2 && o.GetParameters()[0].ParameterType == typeof(SQLiteConnectionWithLock) );
                var expression = Expression.Lambda(predicateType, Expression.MakeBinary(ExpressionType.Equal, accessExpr, Expression.Constant(classifierValue)), new ParameterExpression[] { parameterExpr });

                if (queryMethod == null) return null;
                var iq = queryMethod.Invoke(idpInstance, new object[] { context, expression }) as IEnumerable;
                foreach (var i in iq)
                {
                    existing = i as IIdentifiedEntity;
                    me.Key = existing.Key;
                    if (me is IVersionedEntity)
                        (me as IVersionedEntity).VersionKey = (existing as IVersionedEntity)?.VersionKey ?? Guid.Empty;
                    break;
                }
            }

            // Add to concept dictionary
            //if (existing != null && conceptMe != null && !s_conceptDictionary.ContainsKey(conceptMe.Mnemonic))
            //    lock (s_lock)
            //        s_conceptDictionary.Add(conceptMe.Mnemonic, conceptMe.Key.Value);

#if PERFMON
            sw.Stop();
            s_tracer.TraceVerbose("PERF: TryGetExisting {0} ({1} ms)", me, sw.ElapsedMilliseconds);
#endif

            return existing;

        }

        /// <summary>
        /// Ensure the specified object exists
        /// </summary>
        public static TModel EnsureExists<TModel>(this TModel me, SQLiteConnectionWithLock context) where TModel : IIdentifiedEntity
        {

            // Me
            var vMe = me as IVersionedEntity;
                                   
            // We have to find it
            object idpInstance = null;
            if (!s_idpInstances.TryGetValue(me.GetType(), out idpInstance))
            {
                lock (s_lock)
                {
                    var idpType = typeof(IDataPersistenceService<>).MakeGenericType(me.GetType());
                    idpInstance = ApplicationContext.Current.GetService(idpType);
                    s_idpInstances.Add(me.GetType(), idpInstance);
                }
            }
            
            var existing = me.TryGetExisting(context);

#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            // Existing exists?
            if (existing == null)
            {
                var insertMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Insert" && o.GetParameters().Length == 2);
                if (insertMethod != null)
                {
                    IIdentifiedEntity inserted = insertMethod.Invoke(idpInstance, new object[] { context, me }) as IIdentifiedEntity;
                    me.Key = inserted.Key;

                    if (vMe != null)
                        vMe.VersionKey = (inserted as IVersionedEntity).VersionKey;
                    existing = inserted;
                }

            }

#if PERFMON
            sw.Stop();
            s_tracer.TraceVerbose("PERF: EnsureExists {0} ({1} ms)", me, sw.ElapsedMilliseconds);
#endif
            return existing == null ? me : (TModel)existing;

        }

        ///// <summary>
        ///// Updates a keyed delay load field if needed
        ///// </summary>
        public static void UpdateParentKeys(this IIdentifiedEntity instance, PropertyInfo field)
        {
#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            var delayLoadProperty = field.GetCustomAttribute<SerializationReferenceAttribute>();
            if (delayLoadProperty == null || String.IsNullOrEmpty(delayLoadProperty.RedirectProperty))
                return;
            var value = field.GetValue(instance) as IIdentifiedEntity;
            if (value == null)
                return;
            // Get the delay load key property!
            var keyField = instance.GetType().GetRuntimeProperty(delayLoadProperty.RedirectProperty);
            keyField.SetValue(instance, value.Key);

#if PERFMON
            sw.Stop();
            s_tracer.TraceVerbose("PERF: UpdateParentKeys {0} ({1} ms)", instance, sw.ElapsedMilliseconds);
#endif
        }
    }

    /// <summary>
    /// Represents a dummy service which just adds the persistence services to the context
    /// </summary>
    public class LocalPersistenceService
    {

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
            public override TModel Insert(SQLiteConnectionWithLock context, TModel data)
            {
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
                return base.Insert(context, data);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            public override TModel Update(SQLiteConnectionWithLock context, TModel data)
            {
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
                return base.Update(context, data);
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
            public override TModel Insert(SQLiteConnectionWithLock context, TModel data)
            {
                if (data.IsEmpty()) return data;

                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null) continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        instance = ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        if(instance != null) rp.SetValue(data, instance);
                        ModelExtensions.UpdateParentKeys(data, rp);
                    }
                }
                return base.Insert(context, data);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            public override TModel Update(SQLiteConnectionWithLock context, TModel data)
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
                return base.Update(context, data);
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
            public override TModel Insert(SQLiteConnectionWithLock context, TModel data)
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
                return base.Insert(context, data);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            public override TModel Update(SQLiteConnectionWithLock context, TModel data)
            {
                if (data.IsEmpty()) return data;

                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null) continue;

                    var instance = rp.GetValue(data);
                    if (instance != null && rp.Name != "SourceEntity") // HACK: Prevent infinite loops on associtive entities
                    {
                        ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        if (instance != null) rp.SetValue(data, instance);
                        ModelExtensions.UpdateParentKeys(data, rp);
                    }

                }
                return base.Update(context, data);
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
                        var pclass = typeof(GenericIdentityPersistenceService<,>);
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

