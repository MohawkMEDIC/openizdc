using OpenIZ.Core.Diagnostics;
using System;
using System.Diagnostics.Tracing;

namespace Minims
{
    /// <summary>
    /// Tracer writer that writes to the console
    /// </summary>
    public class ConsoleTraceWriter : TraceWriter
    {
        /// <summary>
        /// Console trace writer
        /// </summary>
        public ConsoleTraceWriter(EventLevel filter, string initializationData) : base(filter, initializationData)
        {
        }

        /// <summary>
        /// Write a trace
        /// </summary>
        protected override void WriteTrace(EventLevel level, string source, string format, params object[] args)
        {
            switch(level)
            {
                case EventLevel.Verbose:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case EventLevel.Informational:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case EventLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case EventLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine("[{0} {1:yyyy/MM/dd HH:mm:ss}] {2} : {3}", level, DateTime.Now, source, String.Format(format, args));
        }
    }
}