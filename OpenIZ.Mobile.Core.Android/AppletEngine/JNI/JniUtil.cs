using System;
using OpenIZ.Core.Model;
using Newtonsoft.Json;
using System.IO;
using OpenIZ.Core.Model.Query;
using System.Collections;
using System.Reflection;
using System.Linq;
using OpenIZ.Core.Model.Attributes;
using System.Xml.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using OpenIZ.Core.Model.Serialization;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	/// <summary>
	/// Jni util.
	/// </summary>
	public static class JniUtil
	{

        // Lock object
        private static object m_lockObject = new object();

        /// <summary>
        /// Load cache of objects already loaded
        /// </summary>
        private static Dictionary<Object, Object> s_loadCache = new Dictionary<Object, Object>();

        /// <summary>
        /// Convert object to JSON.
        /// </summary>
        public static String ToJson<T>(T obj) 
		{

            JsonSerializer jsz = new JsonSerializer();
            jsz.Converters.Add(new StringEnumConverter());
            jsz.NullValueHandling = NullValueHandling.Ignore;

            using (StringWriter sw = new StringWriter())
			{
				jsz.Serialize(sw, obj);
				return sw.ToString();
			}

		}

		/// <summary>
		/// Convert object from JSON
		/// </summary>
		public static T FromJson<T>(String json) 
		{
			JsonSerializer jsz = new JsonSerializer()
            {
                Binder = new ModelSerializationBinder(),
                TypeNameAssemblyFormat = 0,
                TypeNameHandling = TypeNameHandling.All
            };
            jsz.Converters.Add(new StringEnumConverter());
			using (StringReader sr = new StringReader (json)) {
				return (T)jsz.Deserialize (sr, typeof(T));
			}
		}

        /// <summary>
        /// Expand properties
        /// </summary>
        public static void ExpandProperties(IdentifiedData returnValue, NameValueCollection qp)
        {

            // Make sure cache doesn't get too large
            if (s_loadCache.Count > 500)
                lock(m_lockObject)
                    s_loadCache.Clear();

            // Expand property?
            if (!qp.ContainsKey("_expand") && !qp.ContainsKey("_all"))
                return;

            if(qp.ContainsKey("_all"))
            {
                foreach (var pi in returnValue.GetType().GetRuntimeProperties())
                {
                    var scope = pi.GetValue(returnValue);
                    if (scope is IdentifiedData) // Two levels deep
                        foreach (var pi2 in scope.GetType().GetRuntimeProperties())
                            pi2.GetValue(scope);
                    else if(scope is IList)
                        foreach(var itm in scope as IList)
                            if(itm is IdentifiedData)
                                foreach (var pi2 in itm.GetType().GetRuntimeProperties())
                                    pi2.GetValue(itm);
                }
            }
            else
                foreach (var nvs in qp["_expand"])
                {
                    // Get the property the user wants to expand
                    object scope = returnValue;
                    foreach (var property in nvs.Split('.'))
                    {
                        if (scope is IList)
                        {
                            foreach (var sc in scope as IList)
                            {
                                PropertyInfo keyPi = sc.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<XmlElementAttribute>()?.ElementName == property);
                                if (keyPi == null)
                                    continue;
                                // Get the backing property
                                PropertyInfo expandProp = sc.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<DelayLoadAttribute>()?.KeyPropertyName == keyPi.Name);
                                if (expandProp != null)
                                    scope = expandProp.GetValue(sc);
                                else
                                    scope = keyPi.GetValue(sc);

                            }
                        }
                        else
                        {
                            PropertyInfo keyPi = scope.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<XmlElementAttribute>()?.ElementName == property);
                            if (keyPi == null)
                                continue;
                            // Get the backing property
                            PropertyInfo expandProp = scope.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<DelayLoadAttribute>()?.KeyPropertyName == keyPi.Name);

                            Object existing = null;
                            Object keyValue = keyPi.GetValue(scope);

                            if (expandProp != null && expandProp.CanWrite && s_loadCache.TryGetValue(keyValue, out existing))
                            {
                                expandProp.SetValue(scope, existing);
                                scope = existing;
                            }
                            else
                            {
                                if (expandProp != null)
                                {
                                        if (!s_loadCache.ContainsKey(keyValue))
                                        {
                                            scope = expandProp.GetValue(scope);
                                            s_loadCache.Add(keyValue, scope);
                                        }
                                }
                                else
                                    scope = keyValue;
                            }
                        }
                    }
                }
        }
    }
}

