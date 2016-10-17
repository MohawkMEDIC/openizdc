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

namespace OpenIZ.Mobile.Core.Xamarin
{
	/// <summary>
	/// Represents an application context for Xamarin Android
	/// </summary>
	public abstract class XamarinApplicationContext : ApplicationContext
	{

        // Tracer
        protected Tracer m_tracer;

		// Applets
		private AppletCollection m_applets = new AppletCollection();

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
                this.GetService<IClinicalProtocolRepositoryService>().InsertProtocol(pdf.GetProtcolData());
            }
            catch(Exception e)
            {
                this.m_tracer?.TraceError("Error installing protocol {0}: {1}", pdf.Id, e);
                throw;
            }
        }

        /// <summary>
        /// Get applet by id
        /// </summary>
        /// <returns>The applet.</returns>
        /// <param name="id">Identifier.</param>
        public AppletManifest GetApplet (String id)
		{
			return this.m_applets.FirstOrDefault (o => o.Info.Id == id);
		}

		/// <summary>
		/// Register applet
		/// </summary>
		/// <param name="applet">Applet.</param>
		public virtual void LoadApplet (AppletManifest applet)
		{
            if (applet.Info.Id == (this.Configuration.GetSection<AppletConfigurationSection>().StartupAsset ?? "org.openiz.core"))
                this.m_applets.DefaultApplet = applet;
			this.m_applets.Add (applet);
		}

        /// <summary>
        /// Perform platform specific installation
        /// </summary>
        public abstract void InstallApplet(AppletPackage package, bool isUpgrade = false);

        /// <summary>
        /// Get the registered applets
        /// </summary>
        /// <value>The registered applets.</value>
        public AppletCollection LoadedApplets {
			get { 
				return this.m_applets;
			}
		}

	
		/// <summary>
		/// Verifies the manifest against it's recorded signature
		/// </summary>
		/// <returns><c>true</c>, if manifest was verifyed, <c>false</c> otherwise.</returns>
		/// <param name="manifest">Manifest.</param>
		public bool VerifyManifest (AppletManifest manifest, AppletName configuredInfo)
		{
			return true;
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
        /// Get applet asset
        /// </summary>
        public abstract object ResolveAppletAsset(AppletAsset navigateAsset);
        
        #endregion
    }
}

