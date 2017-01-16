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
 * User: fyfej
 * Date: 2016-10-25
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace OpenIZ.Mobile.Core.Configuration.Data
{
	/// <summary>
	/// Data migration log
	/// </summary>
	[XmlType(nameof(DataMigrationLog), Namespace = "http://openiz.org/mobile/configuration")]
	[XmlRoot(nameof(DataMigrationLog), Namespace = "http://openiz.org/mobile/configuration")]
	public class DataMigrationLog
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Configuration.Data.DataMigrationLog"/> class.
		/// </summary>
		public DataMigrationLog ()
		{
			this.Entry = new List<DataMigrationEntry> ();
		}

		/// <summary>
		/// Gets or sets the entry.
		/// </summary>
		[XmlElement("entry")]
		public List<DataMigrationEntry> Entry {
			get;
			set;
		}

		/// <summary>
		/// Data migration entry
		/// </summary>
		[XmlType(nameof(DataMigrationLog), Namespace = "http://openiz.org/mobile/data")]
		public class DataMigrationEntry
		{

			/// <summary>
			/// Initializes a new instance of the
			/// <see cref="OpenIZ.Mobile.Core.Configuration.Data.DataMigrationLog+DataMigrationEntry"/> class.
			/// </summary>
			public DataMigrationEntry ()
			{
				
			}

			/// <summary>
			/// Initializes a new instance of the
			/// <see cref="OpenIZ.Mobile.Core.Configuration.Data.DataMigrationLog+DataMigrationEntry"/> class.
			/// </summary>
			/// <param name="migration">Migration.</param>
			public DataMigrationEntry (IDbMigration migration)
			{
				this.Id = migration.Id;
				this.Date = DateTime.Now;
			}

			/// <summary>
			/// Gets or sets the identifier of the migration
			/// </summary>
			/// <value>The identifier.</value>
			[XmlAttribute("id")]
			public String Id {
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the date when the entry was installed
			/// </summary>
			[XmlAttribute("date")]
			public DateTime Date {
				get;
				set;
			}


		}
	}
}

