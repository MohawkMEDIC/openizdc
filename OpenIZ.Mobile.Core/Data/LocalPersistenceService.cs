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
using SQLite;
using System.Collections.Generic;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Attributes;

namespace OpenIZ.Mobile.Core.Data
{

    /// <summary>
    /// Ensure the data exists
    /// </summary>
    public static class ModelExtensions
    {

        // Guid of stuff that exists and the version
        private static Dictionary<String, Guid?> s_exists = new Dictionary<String, Guid?>();

        // Lock 
        private static Object s_lock = new object();

        /// <summary>
        /// Ensure the specified object exists, insert it if it doesnt
        /// </summary>
        public static void EnsureExists(this IIdentifiedEntity me, SQLiteConnection context) 
        {

            // Me
            var vMe = me as IVersionedEntity;
            String dkey = String.Format("{0}.{1}", me.GetType().FullName, me.Key);
            // Does it exist in our cache?
            Guid? existingGuidVer = Guid.Empty;
            if (s_exists.TryGetValue(dkey, out existingGuidVer))
            {
                if (vMe?.VersionKey == existingGuidVer || vMe == null)
                    return; // Exists already we know about it
            }

            // We have to find it
            var idpType = typeof(IDataPersistenceService<>).MakeGenericType(me.GetType());
            var idpInstance = ApplicationContext.Current.GetService(idpType);
            var getMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o=>o.Name == "Get" && o.GetParameters().Length == 2);
            if (getMethod == null) return;
            var existing = getMethod.Invoke(idpInstance, new object[] { context, me.Key }) as IIdentifiedEntity;

            // Existing exists?
            if (existing != null)
            {
                // Exists but is an old version
                if ((existing as IVersionedEntity)?.VersionKey != vMe?.VersionKey)
                {
                    // Update method
                    var updateMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Update" && o.GetParameters().Length == 2);
                    if (updateMethod != null)
                    {
                        IVersionedEntity updated = updateMethod.Invoke(idpInstance, new object[] { context, me }) as IVersionedEntity;
                        me.Key = updated.Key;
                        if (vMe != null)
                            vMe.VersionKey = (updated as IVersionedEntity).VersionKey;
                    }
                }

                // Add
                dkey = String.Format("{0}.{1}", me.GetType().FullName, existing.Key);

                lock (s_lock)
                    if (existingGuidVer.HasValue)
                        s_exists[dkey] = vMe?.VersionKey;
                    else
                        s_exists.Add(dkey, vMe?.VersionKey);
            }
            else // Insert
            {
                var insertMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Insert" && o.GetParameters().Length == 2);
                if (insertMethod != null)
                {
                    IIdentifiedEntity inserted = insertMethod.Invoke(idpInstance, new object[] { context, me }) as IIdentifiedEntity;
                    me.Key = inserted.Key;

                    if(vMe != null)
                        vMe.VersionKey = (inserted as IVersionedEntity).VersionKey;
                }
                dkey = String.Format("{0}.{1}", me.GetType().FullName, me.Key);

                lock (s_lock)
                    if(me.Key != Guid.Empty)
                        s_exists.Add(dkey, null);

            }
        }

        /// <summary>
        /// Updates a keyed delay load field if needed
        /// </summary>
        public static void UpdateParentKeys(this IIdentifiedEntity instance, PropertyInfo field)
        {
            var delayLoadProperty = field.GetCustomAttribute<DelayLoadAttribute>();
            if (delayLoadProperty == null || String.IsNullOrEmpty(delayLoadProperty.KeyPropertyName))
                return;
            var value = field.GetValue(instance) as IIdentifiedEntity;
            if (value == null)
                return;
            // Get the delay load key property!
            var keyField = instance.GetType().GetRuntimeProperty(delayLoadProperty.KeyPropertyName);
            keyField.SetValue(instance, value.Key);
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
            public override TModel Insert(SQLiteConnection context, TModel data)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        data.UpdateParentKeys(rp);
                    }
                }
                    return base.Insert(context, data);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            public override TModel Update(SQLiteConnection context, TModel data)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        data.UpdateParentKeys(rp);
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
            where TModel : BaseEntityData , new()
        {

            /// <summary>
            /// Ensure exists
            /// </summary>
            public override TModel Insert(SQLiteConnection context, TModel data)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        data.UpdateParentKeys(rp);
                    }
                }
                return base.Insert(context, data);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            public override TModel Update(SQLiteConnection context, TModel data)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        data.UpdateParentKeys(rp);
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
            public override TModel Insert(SQLiteConnection context, TModel data)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    var instance = rp.GetValue(data);
                    if (instance != null)
                    {
                        ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        data.UpdateParentKeys(rp);
                    }
                }
                return base.Insert(context, data);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            public override TModel Update(SQLiteConnection context, TModel data)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    var instance = rp.GetValue(data);
                    if (instance != null)
                    { 
                        ModelExtensions.EnsureExists(instance as IIdentifiedEntity, context);
                        data.UpdateParentKeys(rp);
                    }
                }
                return base.Update(context, data);
            }
        }

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalPersistenceService));

        // Constructor
		public LocalPersistenceService ()
		{
			var appSection = ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection> ();

			// Iterate the persistence services
			foreach(var t in typeof(LocalPersistenceService).GetTypeInfo().Assembly.ExportedTypes.Where(o=>o.Namespace == "OpenIZ.Mobile.Core.Data.Persistence" && !o.GetTypeInfo().IsAbstract))
			{
				try
				{
					this.m_tracer.TraceVerbose ("Loading {0}...", t.AssemblyQualifiedName);
					appSection.Services.Add (Activator.CreateInstance (t));
				}
				catch(Exception e) {
					this.m_tracer.TraceError ("Error adding service {0} : {1}", t.AssemblyQualifiedName, e); 
				}
			}

            // Now iterate through the map file and ensure we have all the mappings, if a class does not exist create it
            try
            {
                this.m_tracer.TraceVerbose("Creating secondary model maps...");
                
                var map = ModelMap.Load(typeof(LocalPersistenceService).GetTypeInfo().Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Data.Map.ModelMap.xml"));
                foreach(var itm in map.Class)
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
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error initializing local persistence: {0}", e);
                throw e;
            }
        }
    }
}

