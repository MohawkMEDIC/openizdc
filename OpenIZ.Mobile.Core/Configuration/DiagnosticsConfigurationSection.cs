/*
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
using System.Reflection;
using SQLite;
using System.Xml.Serialization;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Configuration.Data;
using System.IO;
using System.Diagnostics.Tracing;
using OpenIZ.Mobile.Core.Diagnostics;
using Newtonsoft.Json;

namespace OpenIZ.Mobile.Core.Configuration
{

	/// <summary>
	/// Diagnostics configuration
	/// </summary>
	[XmlType(nameof(DiagnosticsConfigurationSection), Namespace ="http://openiz.org/mobile/configuration")]
	public class DiagnosticsConfigurationSection :IConfigurationSection
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Configuration.DiagnosticsConfigurationSection"/> class.
		/// </summary>
		public DiagnosticsConfigurationSection ()
		{
			this.TraceWriter = new List<TraceWriterConfiguration> ();
		}

		/// <summary>
		/// Trace writers
		/// </summary>
		[XmlElement("trace"), JsonProperty("trace")]
		public List<TraceWriterConfiguration> TraceWriter {
			get;
			set;
		}

	}

	/// <summary>
	/// Trace writer configuration
	/// </summary>
	[XmlType(nameof(TraceWriterConfiguration), Namespace = "http://openiz.org/mobile/configuration")]
	public class TraceWriterConfiguration
	{

		/// <summary>
		/// Trace writer
		/// </summary>
		/// <value>The trace writer.</value>
		[XmlIgnore]
		public TraceWriter TraceWriter {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the initialization data.
		/// </summary>
		/// <value>The initialization data.</value>
		[XmlAttribute("initializationData"), JsonProperty("initializationData")]
		public String InitializationData {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the writer implementation
		/// </summary>
		[XmlElement("writer")]
		public String TraceWriterClassXml {
			get { return this.TraceWriter.GetType ().AssemblyQualifiedName; }
			set { 
				this.TraceWriter = Activator.CreateInstance (Type.GetType (value), this.Filter, this.InitializationData) as TraceWriter; 
			}
		}

		/// <summary>
		/// Gets or sets the filter of the trace writer
		/// </summary>
		/// <value>The filter.</value>
		[XmlAttribute("filter"), JsonProperty("filter")]
		public EventLevel Filter {
			get;
			set;
		}

	}

}

