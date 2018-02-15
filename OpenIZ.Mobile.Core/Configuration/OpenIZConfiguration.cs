/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */
using System;
using System.Reflection;
using SQLite.Net;
using System.Xml.Serialization;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Configuration.Data;
using System.IO;
using OpenIZ.Mobile.Core.Exceptions;

namespace OpenIZ.Mobile.Core.Configuration
{


	/// <summary>
	/// Configuration table object
	/// </summary>
	[XmlRoot(nameof(OpenIZConfiguration), Namespace = "http://openiz.org/mobile/configuration")]
	[XmlType(nameof(OpenIZConfiguration), Namespace = "http://openiz.org/mobile/configuration")]
	[XmlInclude(typeof(SecurityConfigurationSection))]
	[XmlInclude(typeof(DataConfigurationSection))]
	[XmlInclude(typeof(AppletConfigurationSection))]
	[XmlInclude(typeof(ServiceClientConfigurationSection))]
	[XmlInclude(typeof(ApplicationConfigurationSection))]
	[XmlInclude(typeof(DiagnosticsConfigurationSection))]
    [XmlInclude(typeof(SynchronizationConfigurationSection))]
    public class OpenIZConfiguration
	{

	    private static XmlSerializer s_xsz = new XmlSerializer(typeof(OpenIZConfiguration));
        
        /// <summary>
        /// OpenIZ configuration
        /// </summary>
        public OpenIZConfiguration ()
		{
			this.Sections = new List<Object> ();
			this.Version = typeof(OpenIZConfiguration).GetTypeInfo ().Assembly.GetName ().Version.ToString ();
		}

        /// <summary>
        /// Get app setting
        /// </summary>
        public string GetAppSetting(string key)
        {
            return this.GetSection<ApplicationConfigurationSection>()?.AppSettings?.Find(o => o.Key == key)?.Value;
        }

        /// <summary>
        /// Gets or sets the version of the configuration
        /// </summary>
        /// <value>The version.</value>
        [XmlAttribute("version")]
		public String Version {
			get { return typeof(OpenIZConfiguration).GetTypeInfo ().Assembly.GetName ().Version.ToString (); }
			set {

				Version v = new Version (value),
					myVersion = typeof(OpenIZConfiguration).GetTypeInfo ().Assembly.GetName ().Version;
				if(v.Major > myVersion.Major)
					throw new ConfigurationException(String.Format("Configuration file version {0} is newer than OpenIZ version {1}", v, myVersion));
			}
		}

		/// <summary>
		/// Load the specified dataStream.
		/// </summary>
		/// <param name="dataStream">Data stream.</param>
		public static OpenIZConfiguration Load(Stream dataStream)
		{
			return s_xsz.Deserialize (dataStream) as OpenIZConfiguration;
		}

		/// <summary>
		/// Save the configuration to the specified data stream
		/// </summary>
		/// <param name="dataStream">Data stream.</param>
		public void Save(Stream dataStream)
		{
            s_xsz.Serialize (dataStream, this);
		}


		/// <summary>
		/// Configuration sections
		/// </summary>
		/// <value>The sections.</value>
		[XmlElement("section")]
		public List<Object> Sections {
			get;
			set;
		}

		/// <summary>
		/// Get the specified section
		/// </summary>
		/// <returns>The section.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetSection<T>() where T : IConfigurationSection
		{
			return (T)this.GetSection (typeof(T));
		}

		/// <summary>
		/// Gets the section of specified type.
		/// </summary>
		/// <returns>The section.</returns>
		/// <param name="t">T.</param>
		public object GetSection(Type t)
		{
			return this.Sections.Find (o => o.GetType ().Equals (t));
		}

		/// <summary>
		/// Get connection string
		/// </summary>
		/// <returns>The connection string.</returns>
		/// <param name="name">Name.</param>
		public ConnectionString GetConnectionString(String name)
		{
			return this.GetSection<DataConfigurationSection> ()?.ConnectionString.Find (o => o.Name == name);
		}

        /// <summary>
        /// Set application setting
        /// </summary>
        internal void SetAppSetting(string key, Object value)
        {
            var setting = this.GetSection<ApplicationConfigurationSection>()?.AppSettings?.Find(o => o.Key == key);
            if (setting == null)
                this.GetSection<ApplicationConfigurationSection>()?.AppSettings.Add(new AppSettingKeyValuePair() { Key = key, Value = value.ToString() });
            else
                setting.Value = value.ToString();
            ApplicationContext.Current.SaveConfiguration();
        }
    }
}

