﻿/*
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
using System.Linq;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using OpenIZ.Mobile.Core.Configuration.Data;
using System.Xml.Serialization;
using System.Security.Cryptography;
using OpenIZ.Mobile.Core.Security;
using System.Reflection;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Services;
using System.Security;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Applets;
using OpenIZ.Core.Applets.Model;
using System.Security.Principal;
using OpenIZ.Core.Services;
using OpenIZ.Protocol.Xml.Model;
using OpenIZ.Core.Protocol;
using OpenIZ.Core;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using OpenIZ.Mobile.Core.Xamarin.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace OpenIZ.Mobile.Core.Xamarin
{
	/// <summary>
	/// Represents an application context for Xamarin Android
	/// </summary>
	public abstract class XamarinApplicationContext : ApplicationContext
	{

        // Tracer
        protected Tracer m_tracer;

		/// <summary>
		/// Gets the current application context
		/// </summary>
		/// <value>The current.</value>
		public static XamarinApplicationContext Current { get { return ApplicationContext.Current as XamarinApplicationContext; } }

        /// <summary>
        /// Gets the configuration manager
        /// </summary>
        public abstract IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// Install protocol
        /// </summary>
        public void InstallProtocol(IClinicalProtocol pdf)
        {
            try
            {
                this.GetService<IClinicalProtocolRepositoryService>().InsertProtocol(pdf.GetProtocolData());
            }
            catch(Exception e)
            {
                this.m_tracer?.TraceError("Error installing protocol {0}: {1}", pdf.Id, e);
                throw;
            }
        }

        /// <summary>
        /// Explicitly authenticate the specified user as the domain context
        /// </summary>
        public void Authenticate(String userName, String password)
		{
			var identityService = this.GetService<IIdentityProviderService>();
            var principal = identityService.Authenticate(userName, password);
            if (principal == null)
                throw new SecurityException(Strings.err_login_invalidusername);
            AuthenticationContext.Current = new AuthenticationContext(principal);
		}

		#region implemented abstract members of ApplicationContext

	    /// <summary>
        /// Gets the device information for the currently running device
        /// </summary>
        /// <value>The device.</value>
        public override OpenIZ.Core.Model.Security.SecurityDevice Device {
			get {
				// TODO: Load this from configuration
				return new OpenIZ.Core.Model.Security.SecurityDevice () {
					Name = this.Configuration.GetSection<SecurityConfigurationSection>().DeviceName,
					DeviceSecret = this.Configuration.GetSection<SecurityConfigurationSection>().DeviceSecret
				};
			}
		}

        /// <summary>
        /// Loads the user configuration for the specified user key
        /// </summary>
        public override OpenIZConfiguration GetUserConfiguration(string userId)
        {
            try
            {
                var userPrefDir = this.Configuration.GetSection<ApplicationConfigurationSection>().UserPrefDir;
                if (!Directory.Exists(userPrefDir))
                    Directory.CreateDirectory(userPrefDir);

                // Now we want to load
                String configFile = Path.ChangeExtension(Path.Combine(userPrefDir, userId), "userpref");
                if (!File.Exists(configFile))
                    return new OpenIZConfiguration()
                    {
                        Sections = new System.Collections.Generic.List<object>()
                        {
                            new AppletConfigurationSection(),
                            new ApplicationConfigurationSection()
                        }
                    };
                else
                    using (var fs = File.OpenRead(configFile))
                        return OpenIZConfiguration.Load(fs);
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error getting user configuration data {0}: {1}", userId, ex);
                throw;
            }
        }

        /// <summary>
        /// Save user configuration
        /// </summary>
        public override void SaveUserConfiguration(string userId, OpenIZConfiguration config)
        {
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            else if (config == null) throw new ArgumentNullException(nameof(config));

            // Try-catch for save
            try
            {
                var userPrefDir = this.Configuration.GetSection<ApplicationConfigurationSection>().UserPrefDir;
                if (!Directory.Exists(userPrefDir))
                    Directory.CreateDirectory(userPrefDir);

                // Now we want to load
                String configFile = Path.ChangeExtension(Path.Combine(userPrefDir, userId), "userpref");
                using (var fs = File.Create(configFile))
                    config.Save(fs);
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error saving user configuration data {0}: {1}", userId, ex);
                throw;
            }
        }

        #endregion
    }
}

