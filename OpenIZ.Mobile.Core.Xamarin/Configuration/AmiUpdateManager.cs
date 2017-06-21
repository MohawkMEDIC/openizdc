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
 * Date: 2017-3-31
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Applets.Model;
using System.Security.Principal;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Http;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Mobile.Core.Interop;
using System.IO;
using OpenIZ.Mobile.Core.Services;
using System.IO.Compression;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using OpenIZ.Core.Applets.Services;

namespace OpenIZ.Mobile.Core.Xamarin.Configuration
{
    /// <summary>
    /// AMI update manager
    /// </summary>
    public class AmiUpdateManager : IUpdateManager, IDaemonService
    {
        // Cached credential
        private IPrincipal m_cachedCredential = null;

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(AmiUpdateManager));

        // Start events
        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;

        /// <summary>
        /// True if running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets current credentials
        /// </summary>
        private Credentials GetCredentials(IRestClient client)
        {
            var appConfig = ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>();

            AuthenticationContext.Current = new AuthenticationContext(this.m_cachedCredential ?? AuthenticationContext.Current.Principal);

            // TODO: Clean this up - Login as device account
            if (!AuthenticationContext.Current.Principal.Identity.IsAuthenticated ||
                ((AuthenticationContext.Current.Principal as ClaimsPrincipal)?.FindClaim(ClaimTypes.Expiration)?.AsDateTime().ToLocalTime() ?? DateTime.MinValue) < DateTime.Now)
                AuthenticationContext.Current = new AuthenticationContext(ApplicationContext.Current.GetService<IIdentityProviderService>().Authenticate(appConfig.DeviceName, appConfig.DeviceSecret));
            this.m_cachedCredential = AuthenticationContext.Current.Principal;
            return client.Description.Binding.Security.CredentialProvider.GetCredentials(AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Get the server version of a package
        /// </summary>
        public AppletInfo GetServerVersion(string packageId)
        {
            try
            {
                if (ApplicationContext.Current.GetService<INetworkInformationService>().IsNetworkAvailable)
                {
                    var amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));
                    amiClient.Client.Credentials = this.GetCredentials(amiClient.Client);
                    return amiClient.StatUpdate(packageId);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error contacting AMI: {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Install the specified package
        /// </summary>
        public void Install(string packageId)
        {
            try
            {
                if (ApplicationContext.Current.GetService<INetworkInformationService>().IsNetworkAvailable)
                {

                    var amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));
                    amiClient.Client.Credentials = this.GetCredentials(amiClient.Client);
                    amiClient.Client.ProgressChanged += (o, e) => ApplicationContext.Current.SetProgress(String.Format(Strings.locale_downloading, packageId), e.Progress);
                    amiClient.Client.Description.Endpoint[0].Timeout = 1000;
                    // Fetch the applet package
                    using (var ms = amiClient.DownloadApplet(packageId))
                    {
                        var package = AppletPackage.Load(ms);
                        this.m_tracer.TraceInfo("Upgrading {0}...", package.Meta.ToString());
                        ApplicationContext.Current.GetService<IAppletManagerService>().Install(package, true);
                       // ApplicationContext.Current.Exit(); // restart
                    }
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error contacting AMI: {0}", ex.Message);
                throw new InvalidOperationException(Strings.err_updateFailed);
            }
        }

        /// <summary>
        /// Application startup
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            // Check for updates
            ApplicationContext.Current.Started += (o, e) =>
            {
                try
                {
                    if (ApplicationContext.Current.GetService<INetworkInformationService>().IsNetworkAvailable)
                    {
                        ApplicationContext.Current.SetProgress(Strings.locale_updateCheck, 0.5f);

                        // Check for new applications
                        var amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));
                        amiClient.Client.Credentials = this.GetCredentials(amiClient.Client);

                        foreach (var i in amiClient.GetApplets().CollectionItem)
                        {
                            var installed = ApplicationContext.Current.GetService<IAppletManagerService>().GetApplet(i.AppletInfo.Id);
                            if (installed == null || new Version(installed.Info.Version) < new Version(i.AppletInfo.Version) &&
                                ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().AutoUpdateApplets)
                                this.Install(i.AppletInfo.Id);


                        }
                    }
                }
                catch (Exception ex)
                {
                    this.m_tracer.TraceError("Error checking for updates: {0}", ex.Message);
                };
            };

            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            this.Stopped?.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}
