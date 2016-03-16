using System;
using OpenIZ.Core.Model;
using Newtonsoft.Json;
using System.IO;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	/// <summary>
	/// Jni util.
	/// </summary>
	public static class JniUtil
	{

		/// <summary>
		/// Convert object to JSON.
		/// </summary>
		public static String ToJson<T>(T obj) where T : IdentifiedData
		{

			JsonSerializer jsz = new JsonSerializer ();
			using(StringWriter sw = new StringWriter())
			{
				jsz.Serialize(sw, obj);
				return sw.ToString();
			}

		}

		/// <summary>
		/// Convert object from JSON
		/// </summary>
		public static T FromJson<T>(String json) where T : IdentifiedData
		{
			JsonSerializer jsz = new JsonSerializer ();
			using (StringReader sr = new StringReader (json)) {
				return (T)jsz.Deserialize (sr, typeof(T));
			}
		}
	}
}

