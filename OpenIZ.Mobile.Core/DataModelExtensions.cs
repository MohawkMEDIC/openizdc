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
 * Date: 2017-3-31
 */
using Jint.Parser.Ast;
using OpenIZ.Core.Data.QueryBuilder;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Data;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core
{
    /// <summary>
    /// Ensure the data exists
    /// </summary>
    public static class ModelExtensions
    {
        // Lock 
        private static Object s_lock = new object();

        private static Tracer s_tracer = Tracer.GetTracer(typeof(ModelExtensions));
        //private static Dictionary<String, Guid> s_conceptDictionary = new Dictionary<string, Guid>(20);
        // Lock object
        private static Object s_lockObject = new object();

        // Classification properties for autoload
        private static Dictionary<Type, PropertyInfo> s_classificationProperties = new Dictionary<Type, PropertyInfo>();

        // Runtime properties
        private static Dictionary<String, IEnumerable<PropertyInfo>> s_runtimeProperties = new Dictionary<string, IEnumerable<PropertyInfo>>();

        // Readonly types
        private static readonly List<Type> m_readonlyTypes = new List<Type>()
        {
            typeof(Concept),
            typeof(AssigningAuthority),
            typeof(IdentifierType),
            typeof(ConceptName),
            typeof(Material),
            typeof(ManufacturedMaterial),
            typeof(Place)
        };

        // Get instance of cache
        private static Dictionary<Type, Dictionary<Type, Dictionary<PropertyInfo, PropertyInfo>>> s_getInstanceOfCache = new Dictionary<Type, Dictionary<Type, Dictionary<PropertyInfo, PropertyInfo>>>();


        /// <summary>
        /// Gets an instance of TDomain from me
        /// </summary>
        public static TDomain GetInstanceOf<TDomain>(this Object me) where TDomain : new()
        {
            if (me.GetType() == typeof(TDomain)) return (TDomain)me;

            TDomain retVal = new TDomain();

            // First get the runtime properties from cache?
            Dictionary<Type, Dictionary<PropertyInfo, PropertyInfo>> meKnownMaps = null;
            if (!s_getInstanceOfCache.TryGetValue(me.GetType(), out meKnownMaps))
            {
                meKnownMaps = new Dictionary<Type, Dictionary<PropertyInfo, PropertyInfo>>();
                lock (s_getInstanceOfCache)
                    if (!s_getInstanceOfCache.ContainsKey(me.GetType()))
                        s_getInstanceOfCache.Add(me.GetType(), meKnownMaps);
            }

            // Do we have a map for the destination?
            Dictionary<PropertyInfo, PropertyInfo> mapProperties = null;
            if (!meKnownMaps.TryGetValue(typeof(TDomain), out mapProperties))
            {
                mapProperties = new Dictionary<PropertyInfo, PropertyInfo>();
                foreach (var pi in typeof(TDomain).GetRuntimeProperties())
                    mapProperties.Add(pi, me.GetType().GetRuntimeProperty(pi.Name));
                lock (meKnownMaps)
                    if (meKnownMaps.ContainsKey(typeof(TDomain)))
                        meKnownMaps.Add(typeof(TDomain), mapProperties);
            }

            // Now map
            foreach (var prop in mapProperties)
            {
                if (prop.Value != null)
                    prop.Key.SetValue(retVal, prop.Value.GetValue(me));
                else
                    return default(TDomain);
            }
            return retVal;

        }

        /// <summary>
        /// Load specified associations
        /// </summary>
        public static void LoadAssociations<TModel>(this TModel me, LocalDataContext context, params string[] excludeProperties) where TModel : IIdentifiedEntity
        {
            //using (context.LockConnection())
            //{
                if (me == null)
                    throw new ArgumentNullException(nameof(me));
                else if (me.LoadState == LoadState.FullLoad ||
                    context.DelayLoadMode == LoadState.PartialLoad && context.LoadAssociations == null)
                    return;
                else if (context.Connection.IsInTransaction) return;

#if DEBUG
                /*
                 * Me neez all the timez

                   /\_/\
                   >^.^<.---.
                  _'-`-'     )\
                 (6--\ |--\ (`.`-.
                     --'  --'  ``-'
                */
                Stopwatch sw = new Stopwatch();
                sw.Start();
#endif

                // Cache get classification property - thiz makez us fasters
                PropertyInfo classProperty = null;
                if (!s_classificationProperties.TryGetValue(typeof(TModel), out classProperty))
                {
                    classProperty = typeof(TModel).GetRuntimeProperty(typeof(TModel).GetTypeInfo().GetCustomAttribute<ClassifierAttribute>()?.ClassifierProperty ?? "____XXX");
                    if (classProperty != null)
                        classProperty = typeof(TModel).GetRuntimeProperty(classProperty.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty ?? classProperty.Name);
                    lock (s_lockObject)
                        if (!s_classificationProperties.ContainsKey(typeof(TModel)))
                            s_classificationProperties.Add(typeof(TModel), classProperty);
                }

                // Classification property?
                String classValue = classProperty?.GetValue(me)?.ToString();

                // Cache the props so future kitties can call it
                IEnumerable<PropertyInfo> properties = null;
                var propertyCacheKey = $"{me.GetType()}.FullName[{classValue}]";
                if (!s_runtimeProperties.TryGetValue(propertyCacheKey, out properties))
                    lock (s_runtimeProperties)
                    {
                        properties = me.GetType().GetRuntimeProperties().Where(o => o.GetCustomAttribute<DataIgnoreAttribute>() == null && o.GetCustomAttributes<AutoLoadAttribute>().Any(p => p.ClassCode == classValue || p.ClassCode == null) && typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.StripGeneric().GetTypeInfo()));

                        if (!s_runtimeProperties.ContainsKey(propertyCacheKey))
                        {
                            s_runtimeProperties.Add(propertyCacheKey, properties);
                        }
                    }

                // Load fast or lean mode only root associations which will appear on the wire
                if (context.LoadAssociations != null)
                {
                    var loadAssociations = context.LoadAssociations.Where(o => o.StartsWith(me.GetType().GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName))
                        .Select(la => la.Contains(".") ? la.Substring(la.IndexOf(".") + 1) : la).ToArray();
                    if (loadAssociations.Length == 0) return;
                    properties = properties.Where(p => loadAssociations.Any(la => la == p.Name));

                }

                // Iterate over the properties and load the properties
                foreach (var pi in properties)
                {

                    if (excludeProperties?.Contains(pi.Name) == true)
                        continue;
                    // Map model type to domain
                    var adoPersister = LocalPersistenceService.GetPersister(pi.PropertyType.StripGeneric());

                    // Loading associations, so what is the associated type?
                    if (typeof(IList).GetTypeInfo().IsAssignableFrom(pi.PropertyType.GetTypeInfo()) &&
                        adoPersister is ILocalAssociativePersistenceService &&
                        me.Key.HasValue) // List so we select from the assoc table where we are the master table
                    {
                        // Is there not a value?
                        var assocPersister = adoPersister as ILocalAssociativePersistenceService;

                        // We want to que   ry based on our PK and version if applicable
                        decimal? versionSequence = (me as IVersionedEntity)?.VersionSequence;
                        var assoc = assocPersister.GetFromSource(context, me.Key.Value, versionSequence);
                        var listValue = Activator.CreateInstance(pi.PropertyType, assoc);
                        pi.SetValue(me, listValue);
                    }
                    else if (typeof(IIdentifiedEntity).GetTypeInfo().IsAssignableFrom(pi.PropertyType.GetTypeInfo())) // Single
                    {
                        // Single property, we want to execute a get on the key property
                        var redirectAtt = pi.GetCustomAttribute<SerializationReferenceAttribute>();
                        if (redirectAtt == null)
                            continue; // cannot get key property

                        // We want to issue a query
                        var keyProperty = pi.DeclaringType.GetRuntimeProperty(redirectAtt.RedirectProperty);
                        var keyValue = keyProperty?.GetValue(me);
                        if (keyValue == null ||
                            Guid.Empty.Equals(keyValue))
                            continue; // No key specified

                        // This is kinda messy.. maybe iz to be changez
                        object value = null;
                        if (!context.Data.TryGetValue(keyValue.ToString(), out value))
                        {
                            value = adoPersister.Get(context, (Guid)keyValue);
                            context.AddData(keyValue.ToString(), value);
                        }
                        pi.SetValue(me, value);
                    }

                }
#if DEBUG
                sw.Stop();
                s_tracer.TraceVerbose("Load associations for {0} took {1} ms", me, sw.ElapsedMilliseconds);
#endif

                me.LoadState = LoadState.FullLoad;

                ApplicationContext.Current.GetService<IDataCachingService>()?.Add(me as IdentifiedData);
            //}
        }

        /// <summary>
        /// Try get by classifier
        /// </summary>
        public static IIdentifiedEntity TryGetExisting(this IIdentifiedEntity me, LocalDataContext context)
        {

            // Is there a classifier?
            var idpInstance = LocalPersistenceService.GetPersister(me.GetType()) as ILocalPersistenceService;
            if (idpInstance == null) return null;
            //if (me.Key?.ToString() == "e4d3350b-b0f5-45c1-80ba-49e3844cbcc8")
            //    System.Diagnostics.Debugger.Break();

            IIdentifiedEntity existing = null;
            if (me.Key != null)
                existing = context.TryGetCacheItem(me.Key.Value);
            if (existing != null) return existing;
            else if (!context.Connection.IsInTransaction)
                existing = context.TryGetData(me.Key.Value.ToString()) as IdentifiedData;

            // Is the key not null?
            if (me.Key != Guid.Empty && me.Key != null)
            {
                existing = idpInstance.Get(context, me.Key.Value) as IIdentifiedEntity;
            }

            var classAtt = me.GetType().GetTypeInfo().GetCustomAttribute<KeyLookupAttribute>();
            if (classAtt != null && existing == null)
            {

                // Get the domain type
                var dataType = LocalPersistenceService.Mapper.MapModelType(me.GetType());
                var tableMap = TableMapping.Get(dataType);

                // Get the classifier attribute value
                var classProperty = me.GetType().GetRuntimeProperty(classAtt.UniqueProperty);
                object classifierValue = classProperty.GetValue(me); // Get the classifier

                // Is the classifier a UUID'd item?
                if (classifierValue is IIdentifiedEntity)
                {
                    classifierValue = (classifierValue as IIdentifiedEntity).Key.Value;
                    classProperty = me.GetType().GetRuntimeProperty(classProperty.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty ?? classProperty.Name);
                }

                // Column 
                var column = tableMap.GetColumn(LocalPersistenceService.Mapper.MapModelProperty(me.GetType(), dataType, classProperty));
                // Now we want to query 
                SqlStatement stmt = new SqlStatement().SelectFrom(dataType)
                    .Where($"{column.Name} = ?", classifierValue).Build();

                var mapping = context.Connection.GetMapping(dataType);
                var dataObject = context.Connection.Query(mapping, stmt.SQL, stmt.Arguments.ToArray()).FirstOrDefault();
                if (dataObject != null)
                    existing = idpInstance.ToModelInstance(dataObject, context) as IIdentifiedEntity;
            }

            if (existing != null && me.Key.HasValue)
                context.AddData(me.Key.Value.ToString(), existing);

            return existing;

        }

        /// <summary>
        /// Ensure the specified object exists
        /// </summary>
        public static TModel EnsureExists<TModel>(this TModel me, LocalDataContext context) where TModel : IIdentifiedEntity
        {

            // Me
            var vMe = me as IVersionedEntity;

            // We have to find it
            var idpInstance = LocalPersistenceService.GetPersister(me.GetType());

            var existing = me.TryGetExisting(context);


            // Existing exists?
            if (existing == null && !m_readonlyTypes.Contains(typeof(TModel)))
            {
                IIdentifiedEntity inserted = ((TModel)idpInstance.Insert(context, me)) as IIdentifiedEntity;
                me.Key = inserted.Key;

                if (vMe != null)
                    vMe.VersionKey = (inserted as IVersionedEntity).VersionKey;
                existing = inserted;

            }
            else if (existing == null)
                throw new KeyNotFoundException($"Object {me} not found in database and is restricted for creation");

            return existing == null ? me : (TModel)existing;

        }

        ///// <summary>
        ///// Updates a keyed delay load field if needed
        ///// </summary>
        public static void UpdateParentKeys(this IIdentifiedEntity instance, PropertyInfo field)
        {

            var delayLoadProperty = field.GetCustomAttribute<SerializationReferenceAttribute>();
            if (delayLoadProperty == null || String.IsNullOrEmpty(delayLoadProperty.RedirectProperty))
                return;
            var value = field.GetValue(instance) as IIdentifiedEntity;
            if (value == null)
                return;
            // Get the delay load key property!
            var keyField = instance.GetType().GetRuntimeProperty(delayLoadProperty.RedirectProperty);
            keyField.SetValue(instance, value.Key);

        }
    }
}
