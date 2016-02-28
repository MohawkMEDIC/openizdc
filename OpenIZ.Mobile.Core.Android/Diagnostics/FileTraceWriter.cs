using System;
using OpenIZ.Mobile.Core.Diagnostics;
using System.IO;
using System.Diagnostics.Tracing;

namespace OpenIZ.Mobile.Core.Android.Diagnostics
{
	/// <summary>
	/// Represents a trace listener that appends data to a file
	/// </summary>
	public class FileTraceWriter : TraceWriter
	{
		// The text writer
		private String m_logFile;

		// Sync object
		private Object m_syncObject = new object();

		// Number of logs to keep
		private int m_keepLogs = 4;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Diagnostics.FileTraceListener"/> class.
		/// </summary>
		/// <param name="filter">Filter.</param>
		/// <param name="fileName">File name.</param>
		public FileTraceWriter (EventLevel filter, String fileName) : base(filter, null)
		{
			// First, we want to remove the oldest log
			String logFileBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log", fileName + ".log");
			if (!Directory.Exists (Path.GetDirectoryName (logFileBase)))
				Directory.CreateDirectory (Path.GetDirectoryName (logFileBase));

			for (int i = this.m_keepLogs - 1; i > 0; i--) {
				string oldFile = String.Format ("{0}.{1:000}", logFileBase, i + 1),
					newFile = String.Format ("{0}.{1:000}", logFileBase, i);
				if (File.Exists (newFile)) {
					if (File.Exists (oldFile))
						File.Delete (oldFile);
					File.Move (newFile, oldFile); // Move OpenIZ.log.001 > OpenIZ.log.002 ...
				}
			}
			// Move last recorded log file
			if(File.Exists(logFileBase))
				File.Move(logFileBase, String.Format("{0}.001", logFileBase));

			this.m_logFile = logFileBase;
				
			this.WriteTrace (EventLevel.Informational, "Startup", "OpenIZ.Mobile.Core Version: {0} logging at level [{1}]", typeof(TraceWriter).Assembly.GetName ().Version, filter);
		}

		#region implemented abstract members of TraceWriter
		/// <summary>
		/// Write the trace to the log file
		/// </summary>
		/// <param name="level">Level.</param>
		/// <param name="source">Source.</param>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		protected override void WriteTrace (System.Diagnostics.Tracing.EventLevel level, string source, string format, params object[] args)
		{
			lock (this.m_syncObject)
				using (TextWriter tw = File.AppendText (this.m_logFile)) {
					tw.Write ("{0} [{1}] [{2:o}]:", source, level, DateTime.Now);
					tw.WriteLine (format, args);
				}
		}
		#endregion
	}
}

