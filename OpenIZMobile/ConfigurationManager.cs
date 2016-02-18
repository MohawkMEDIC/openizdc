using System;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using OpenIZ.Mobile.Core.Configuration;
using Android.Content.Res;

namespace OpenIZMobile
{
	/// <summary>
	/// Configuration manager for the application
	/// </summary>
	public class ConfigurationManager
	{

		// Configuration
		private OpenIZConfiguration m_configuration;

		// Configuration manager
		private static ConfigurationManager s_configManager;

		/// <summary>
		/// Creates a new instance of the configuration manager with the specified configuration file
		/// </summary>
		private ConfigurationManager ()
		{

			String configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenIZ.config");
			XmlSerializer xsz = new XmlSerializer (typeof(OpenIZConfiguration));

			// Configuration exists?
			if (File.Exists (configPath))
				using (var fs = File.OpenRead (configPath)) {
					this.m_configuration = xsz.Deserialize (fs) as OpenIZConfiguration;
				}
			else {
				// TODO: Bring up initial settings dialog and utility
				this.m_configuration = new OpenIZConfiguration();
				this.m_configuration.AppDataFile = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "OpenIZ.sqlite");
				this.m_configuration.AppletDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "applets");

				if (!Directory.Exists (Path.GetDirectoryName (configPath)))
					Directory.CreateDirectory (Path.GetDirectoryName (configPath));
				if (!Directory.Exists (this.m_configuration.AppletDir))
					Directory.CreateDirectory (this.m_configuration.AppletDir);
				
				using (var fs = File.Create (configPath))
					xsz.Serialize (fs, this.m_configuration);
			}
		}

		/// <summary>
		/// Get the current configuration manager singleton
		/// </summary>
		public static ConfigurationManager Current {
			get {
				if (s_configManager == null)
					s_configManager = new ConfigurationManager ();
				return s_configManager;
			}
		}

		/// <summary>
		/// Get the configuration
		/// </summary>
		public OpenIZConfiguration Configuration {
			get {
				return this.m_configuration;
			}
		}

	}
}

