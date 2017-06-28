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
 * Date: 2016-6-14
 */
using System;
using SQLite.Net;
using System.Xml.Serialization;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Configuration.Data;
using System.IO;

namespace OpenIZ.Mobile.Core.Configuration
{
	/// <summary>
	/// Data configuration section
	/// </summary>
	[XmlType(nameof(DataConfigurationSection), Namespace = "http://openiz.org/mobile/configuration")]
	public class DataConfigurationSection : IConfigurationSection
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Configuration.DataConfigurationSection"/> class.
		/// </summary>
		public DataConfigurationSection ()
		{
			this.MigrationLog = new DataMigrationLog();
			this.ConnectionString = new List<OpenIZ.Mobile.Core.Configuration.ConnectionString> ();
		}

        /// <summary>
        /// Gets or sets connection strings
        /// </summary>
        /// <value>My property.</value>
        [XmlElement("connectionString")]
		public List<ConnectionString> ConnectionString {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the main data source connection string.
		/// </summary>
		/// <value>The name of the main data source connection string.</value>
		[XmlAttribute("clinicalDataStore")]
		public String MainDataSourceConnectionStringName {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the message queue connection string.
		/// </summary>
		/// <value>The name of the message queue connection string.</value>
		[XmlAttribute("messageQueue")]
		public String MessageQueueConnectionStringName {
			get;
			set;
		}


		/// <summary>
		/// Migration log 
		/// </summary>
		/// <value>The migration log.</value>
		[XmlElement("migration")]
		public DataMigrationLog MigrationLog {
			get;
			set;
		}
	}

	/// <summary>
	/// Represents a single connection string
	/// </summary>
	[XmlType(nameof(ConnectionString), Namespace = "http://openiz.org/mobile/configuration")]
	public class ConnectionString
	{

		/// <summary>
		/// Gets or sets the name
		/// </summary>
		[XmlAttribute("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the connection string
		/// </summary>
		[XmlAttribute("value")]
		public String Value{
			get;
			set;
		}
	}

}

