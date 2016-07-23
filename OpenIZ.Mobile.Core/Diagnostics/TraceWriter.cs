﻿/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-14
 */
using System;
using System.Diagnostics.Tracing;

namespace OpenIZ.Mobile.Core.Diagnostics
{
	/// <summary>
	/// Because we're using PCL we have to wrap the TraceWriter interface 
	/// </summary>
	public abstract class TraceWriter
	{

		// Filter
		private EventLevel m_filter;

		public TraceWriter (EventLevel filter, String initializationData)
		{
			this.m_filter = filter;
		}
			
		/// <summary>
		/// Trace information
		/// </summary>
		public void TraceInfo (String source, String format, params Object[] args) {
			this.TraceEvent (EventLevel.Informational, source, format, args);
		}

		/// <summary>
		/// Trace an event to the writer
		/// </summary>
		public virtual void TraceEvent(EventLevel level, String source, String format, params Object[] args){
			if (this.m_filter == EventLevel.LogAlways)
				this.WriteTrace (level, source, format, args);
			else if (this.m_filter >= level)
				this.WriteTrace(level, source, format, args);
		}

		/// <summary>
		/// Write data to the event
		/// </summary>
		protected abstract void WriteTrace(EventLevel level, String source, String format, params Object[] args);


		/// <summary>
		/// Trace an error
		/// </summary>
		public void TraceError(String source, String format, params Object[] args) {
			this.TraceEvent (EventLevel.Error, source, format, args);
		}

		/// <summary>
		/// Trace warning
		/// </summary>
		public void TraceWarning(String source, String format, params Object[] args) {
			this.TraceEvent (EventLevel.Warning, source, format, args);

		}
	}
}

