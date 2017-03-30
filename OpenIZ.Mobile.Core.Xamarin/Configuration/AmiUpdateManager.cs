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
                this.m_tracer.TraceError("Error contacting AMI: {0}", ex);
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
                    amiClient.Client.ProgressChanged += (o, e) => ApplicationContext.Current.SetProgress(String.Format(Strings.locale_updating, packageId), e.Progress);
                    // Fetch the applet package
                    var applet = amiClient.Client.Get($"/pak/{packageId}");
                    using (var ms = new MemoryStream(applet))
                    using (var gzs = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        var package = AppletPackage.Load(gzs);
                        this.m_tracer.TraceInfo("Upgrading {0}...", package.Meta.ToString());
                        ApplicationContext.Current.InstallApplet(package);
                        //ApplicationContext.Current.Exit(); // restart
                    }
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error contacting AMI: {0}", ex);
                throw;
            }
        }

        /// <summary>
        /// Application startup
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            // Check for updates
            try
            {
                if (ApplicationContext.Current.GetService<INetworkInformationService>().IsNetworkAvailable)
                {
                    ApplicationContext.Current.SetProgress(Strings.locale_updateCheck, 0.5f);

                    // Check for new applications
                    var amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));
                    amiClient.Client.Credentials = this.GetCredentials(amiClient.Client);

                    foreach (var i in amiClient.GetApplets().CollectionItem) {
                        var installed = XamarinApplicationContext.Current.GetApplet(i.AppletManifest.Info.Id);
                        if (installed == null || new Version(installed.Info.Version) < new Version(i.AppletManifest.Info.Version) &&
                            ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().AutoUpdateApplets)
                            this.Install(i.AppletManifest.Info.Id);


                    }
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error checking for updates: {0}", e);
            }

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
