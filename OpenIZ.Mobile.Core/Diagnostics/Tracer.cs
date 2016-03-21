using System;
using System.Linq;
using OpenIZ.Mobile.Core.Configuration;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace OpenIZ.Mobile.Core.Diagnostics
{
	/// <summary>
	/// Represents a logger
	/// </summary>
	public sealed class Tracer 
	{

		// The source of the logger
		private String m_source;

		// Writers
		private List<TraceWriter> m_writers = new List<TraceWriter>();

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Diagnostics.Logger"/> class.
		/// </summary>
		/// <param name="source">Source.</param>
		private Tracer (String source)
		{
			this.m_source = source;
		}

		/// <summary>
		/// Creates a logging interface for the specified source
		/// </summary>
		public static Tracer GetTracer (Type sourceType)
		{
			if (ApplicationContext.Current == null)
				return new Tracer (sourceType.FullName);
			else
				return GetTracer (sourceType, ApplicationContext.Current.Configuration);
		}

		/// <summary>
		/// Creates a logging interface for the specified source
		/// </summary>
		public static Tracer GetTracer (Type sourceType, OpenIZConfiguration configuration)
		{
			var retVal = new Tracer (sourceType.FullName);
			retVal.m_writers = new List<TraceWriter> (configuration.GetSection<DiagnosticsConfigurationSection> ()?.TraceWriter.Select (o => o.TraceWriter));
			return retVal;
		}

		/// <summary>
		/// Trace an event 
		/// </summary>
		public void TraceEvent (System.Diagnostics.Tracing.EventLevel level, string format, params Object[] args)
		{
			foreach (var w in this.m_writers) {
				w.TraceEvent (level, this.m_source, format, args);
			}
		}

		/// <summary>
		/// Trace error 
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		public void TraceError(String format, params Object[] args )
		{
			this.TraceEvent(EventLevel.Error, format, args);
		}

		/// <summary>
		/// Trace error 
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		public void TraceWarning(String format, params Object[] args )
		{
			this.TraceEvent(EventLevel.Warning, format, args);
		}

		/// <summary>
		/// Trace error 
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		public void TraceInfo(String format, params Object[] args )
		{
			this.TraceEvent(EventLevel.Informational, format, args);
		}

		/// <summary>
		/// Trace error 
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		public void TraceVerbose(String format, params Object[] args)
		{
			this.TraceEvent(EventLevel.Verbose, format, args);
		}

	}
}

