using System;
using OpenIZ.Mobile.Core.Android.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Linq;
using System.Collections.Generic;


namespace OpenIZ.Mobile.Core.Diagnostics
{
	/// <summary>
	/// Tracer extensions
	/// </summary>
	public static class TracerHelper
	{
		/// <summary>
		/// Get the tracer
		/// </summary>
		/// <returns>The tracer.</returns>
		/// <param name="">.</param>
		/// <param name="type">Type.</param>
		public static Tracer GetTracer(Type type)
		{
			return Tracer.CreateTracer(type, ConfigurationManager.Current.Configuration);
		}
	}
}

