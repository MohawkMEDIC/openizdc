using System;
using SQLite;
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

