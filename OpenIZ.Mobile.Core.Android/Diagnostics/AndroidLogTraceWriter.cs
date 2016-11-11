using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Core.Diagnostics;
using Android.Util;

namespace OpenIZ.Mobile.Core.Android.Diagnostics
{
    /// <summary>
    /// Android log trace writer
    /// </summary>
    public class AndroidLogTraceWriter : TraceWriter
    {
        /// <summary>
        /// constructor for the trace writer
        /// </summary>
        public AndroidLogTraceWriter(EventLevel filter, string initializationData) : base(filter, initializationData)
        {
        }

        /// <summary>
        /// Write a trace to the console
        /// </summary>
        protected override void WriteTrace(EventLevel level, string source, string format, params object[] args)
        {
            switch(level)
            {
                case EventLevel.Critical:
                    Log.Wtf(source, String.Format(format, args));
                    break;
                case EventLevel.Error:
                    Log.Error(source, String.Format(format, args));
                    break;
                case EventLevel.Warning:
                    Log.Warn(source, String.Format(format, args));
                    break;
                case EventLevel.Informational:
                    Log.Info(source, String.Format(format, args));
                    break;
                case EventLevel.Verbose:
                    Log.Verbose(source, String.Format(format, args));
                    break;
            }
        }
    }
}