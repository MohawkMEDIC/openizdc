﻿/*
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
using System;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Diagnostics.Tracing;
using System.Diagnostics;
using OpenIZ.Core.Diagnostics;

namespace OpenIZ.Mobile.Core.Xamarin.Diagnostics
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
					Trace.TraceError(msg, args);
					break;
				case EventLevel.Informational:
                    Trace.TraceInformation (msg, args);
					break;
				case EventLevel.Critical:
                    Trace.TraceError (msg, args);
					break;
				case EventLevel.Verbose:
                    Trace.TraceInformation (msg, args);
					break;
				case EventLevel.Warning:
                    Trace.TraceWarning (msg, args);
					break;
			
			}
		}
		#endregion
	}
}

