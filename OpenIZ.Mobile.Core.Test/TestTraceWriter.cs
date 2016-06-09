using System;
using System.Diagnostics.Tracing;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Diagnostics;

namespace OpenIZ.Mobile.Core.Test
{
    /// <summary>
    /// Test trace writer
    /// </summary>
    internal class TestTraceWriter : TraceWriter
    {
        public TestTraceWriter(EventLevel filter, string initializationData) : base(filter, initializationData)
        {
        }

        /// <summary>
        /// Write a trace
        /// </summary>
        protected override void WriteTrace(EventLevel level, string source, string format, params object[] args)
        {
            Trace.WriteLine(String.Format("{0}/{1} : {2}", level, source, String.Format(format, args)));
        }
    }
}