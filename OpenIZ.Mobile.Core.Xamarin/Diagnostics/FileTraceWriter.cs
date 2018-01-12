/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using System;
using OpenIZ.Mobile.Core.Diagnostics;
using System.IO;
using System.Diagnostics.Tracing;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Services;
using System.Collections.Generic;
using System.Threading;
using OpenIZ.Mobile.Core.Xamarin.Resources;

namespace OpenIZ.Mobile.Core.Xamarin.Diagnostics
{
    /// <summary>
    /// Represents a trace listener that appends data to a file
    /// </summary>
    public class FileTraceWriter : TraceWriter, IDisposable
    {

        // Dispatch thread
        private Thread m_dispatchThread = null;

        // True when disposing
        private bool m_disposing = false;

        // The text writer
        private String m_logFile;

        // The log backlog
        private Queue<String> m_logBacklog = new Queue<string>();

        // Sync object
        private static Object s_syncObject = new object();

        // Number of logs to keep
        private int m_keepLogs = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Xamarin.Diagnostics.FileTraceListener"/> class.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <param name="fileName">File name.</param>
        public FileTraceWriter(EventLevel filter, String fileName) : base(filter, null)
        {
            // First, we want to remove the oldest log
            String logFileBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log", fileName + ".log");
            if (!Directory.Exists(Path.GetDirectoryName(logFileBase)))
                Directory.CreateDirectory(Path.GetDirectoryName(logFileBase));

            for (int i = this.m_keepLogs - 1; i > 0; i--)
            {
                string oldFile = String.Format("{0}.{1:000}", logFileBase, i + 1),
                    newFile = String.Format("{0}.{1:000}", logFileBase, i);
                if (File.Exists(newFile))
                {
                    if (File.Exists(oldFile))
                        File.Delete(oldFile);
                    File.Move(newFile, oldFile); // Move OpenIZ.log.001 > OpenIZ.log.002 ...
                }
            }
            // Move last recorded log file
            if (File.Exists(logFileBase))
                File.Move(logFileBase, String.Format("{0}.001", logFileBase));

            this.m_logFile = logFileBase;

            // Start log dispatch
            this.m_dispatchThread = new Thread(this.LogDispatcherLoop);
            this.m_dispatchThread.IsBackground = true;
            this.m_dispatchThread.Start();

            this.WriteTrace(EventLevel.Informational, "Startup", "OpenIZ.Mobile.Core Version: {0} logging at level [{1}]", typeof(ApplicationContext).Assembly.GetName().Version, filter);
        }

        #region implemented abstract members of TraceWriter
        /// <summary>
        /// Write the trace to the log file
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="source">Source.</param>
        /// <param name="format">Format.</param>
        /// <param name="args">Arguments.</param>
        protected override void WriteTrace(System.Diagnostics.Tracing.EventLevel level, string source, string format, params object[] args)
        {
            lock (this.m_logBacklog)
            {
                this.m_logBacklog.Enqueue(String.Format("{0}@{1} <{2}> [{3:o}]: {4}", source, Thread.CurrentThread.Name, level, DateTime.Now, String.Format(format, args)));
                //string dq = String.Format("{0}@{1} <{2}> [{3:o}]: {4}", source, Thread.CurrentThread.Name, level, DateTime.Now, String.Format(format, args));
                //using (TextWriter tw = File.AppendText(this.m_logFile))
                //    tw.WriteLine(dq); // This allows other threads to add to the write queue

                Monitor.Pulse(this.m_logBacklog);
            }
        }
        #endregion

        /// <summary>
        /// Log dispatcher loop.
        /// </summary>
        private void LogDispatcherLoop()
        {
            while (true)
            {
                while (true)
                {
                    try
                    {
                        Monitor.Enter(this.m_logBacklog);
                        if (this.m_disposing) return; // shutdown dispatch
                        while (this.m_logBacklog.Count == 0)
                            Monitor.Wait(this.m_logBacklog);
                        if (this.m_disposing) return;

                        using (TextWriter tw = File.AppendText(this.m_logFile))
                            while (this.m_logBacklog.Count > 0)
                            {
                                var dq = this.m_logBacklog.Dequeue();
                                Monitor.Exit(this.m_logBacklog);
                                tw.WriteLine(dq); // This allows other threads to add to the write queue
                                Monitor.Enter(this.m_logBacklog);
                            }
                    }
                    catch
                    {
                        ;
                    }
                    finally
                    {
                        if (Monitor.IsEntered(this.m_logBacklog))
                            Monitor.Exit(this.m_logBacklog);
                    }
                }

            }
        }

        /// <summary>
        /// Dispose of the object
        /// </summary>
        public void Dispose()
        {
            if(this.m_dispatchThread != null)
            {
                this.m_disposing = true;
                lock (this.m_logBacklog)
                    Monitor.PulseAll(this.m_logBacklog);
                this.m_dispatchThread.Join(); // Abort thread
                this.m_dispatchThread = null;
            }
        }
    }
}

