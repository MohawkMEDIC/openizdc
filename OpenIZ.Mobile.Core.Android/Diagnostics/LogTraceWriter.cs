using System;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Diagnostics.Tracing;
using Android.Util;

namespace OpenIZ.Mobile.Core.Android.Diagnostics
{
	/// <summary>
	/// Represents a trace writer which writes to the android log
	/// </summary>
	public class LogTraceWriter : TraceWriter
	{
		// Source name
		private String m_sourceName;

		/// <summary>
		/// Initialize the trace writer
		/// </summary>
		public LogTraceWriter (EventLevel filter, String initializationData) : base(filter, null)
		{
			this.m_sourceName = initializationData;
		}


		#region implemented abstract members of TraceWriter
		/// <summary>
		/// Write trace
		/// </summary>
		protected override void WriteTrace (EventLevel level, string source, string format, params object[] args)
		{
			String msg = String.Format ("{0}:{1}", source, format);
			switch (level) {
				case EventLevel.Error:
					Log.Error (this.m_sourceName, msg, args);
					break;
				case EventLevel.Informational:
					Log.Info (this.m_sourceName, msg, args);
					break;
				case EventLevel.Critical:
					Log.Wtf (this.m_sourceName, msg, args);
					break;
				case EventLevel.Verbose:
					Log.Verbose (this.m_sourceName, msg, args);
					break;
				case EventLevel.Warning:
					Log.Warn (this.m_sourceName, msg, args);
					break;
			
			}
		}
		#endregion
	}
}

