/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2017-3-31
 */

using Android.Util;
using OpenIZ.Core.Diagnostics;
using System;
using System.Diagnostics.Tracing;

namespace OpenIZ.Mobile.Core.Android.Diagnostics
{
	/// <summary>
	/// Android log trace writer.
	/// </summary>
	public class AndroidLogTraceWriter : TraceWriter
	{
		/// <summary>
		/// constructor for the trace writer
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="initializationData">The initialization data.</param>
		public AndroidLogTraceWriter(EventLevel filter, string initializationData) : base(filter, initializationData)
		{
		}

		/// <summary>
		/// Write a trace to the console
		/// </summary>
		/// <param name="level">The level.</param>
		/// <param name="source">The source.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		protected override void WriteTrace(EventLevel level, string source, string format, params object[] args)
		{
			switch (level)
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
			Console.WriteLine(format, args);
		}
	}
}