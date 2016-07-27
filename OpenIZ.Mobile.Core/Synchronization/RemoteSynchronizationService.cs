using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Synchronization.Model;
using System.Threading;
using OpenIZ.Mobile.Core.Interop;
using OpenIZ.Mobile.Core.Resources;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Alerting;

namespace OpenIZ.Mobile.Core.Synchronization
{
    /// <summary>
    /// Represents a synchronization service which can query the IMSI and place 
    /// entries onto the inbound queue
    /// </summary>
    public class RemoteSynchronizationService : ISynchronizationService, IDaemonService
    {

        // Lock
        private object m_lock = new object();
        // Get the tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(RemoteSynchronizationService));
        // The configuration for synchronization
        private SynchronizationConfigurationSection m_configuration;
        // Thread pool
        private IThreadPoolService m_threadPool;
        // Network service
        private IIntegrationService m_integrationService;
        // Device principal 
        private IPrincipal m_devicePrincipal;

        /// <summary>
        /// Fired when the service is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the service has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the service is stopping
        /// </summary>
        public event EventHandler Stopping;
        /// <summary>
        /// Fired when the service ahs stopped
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Returns true if the service is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return true;
            }
        }


        /// <summary>
        /// Start the service
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            // Get configuration
            this.m_configuration = ApplicationContext.Current.Configuration.GetSection<SynchronizationConfigurationSection>();
            this.m_threadPool = ApplicationContext.Current.GetService<IThreadPoolService>();
            this.m_integrationService = ApplicationContext.Current.GetService<IIntegrationService>();

            // Pool startup sync if configured..
            this.m_threadPool.QueueUserWorkItem((state) =>
            {
                if (Monitor.IsEntered(this.m_lock))
                    return; // Someone has a lock on us
                try
                {
                    lock (this.m_lock)
                        foreach (var syncResource in this.m_configuration.SynchronizationResources.Where(o => (o.Triggers & SynchronizationPullTriggerType.OnStart) != 0))
                        {

                            foreach (var fltr in syncResource.Filters)
                                this.Pull(syncResource.ResourceType, NameValueCollection.ParseQueryString(fltr));
                            if (syncResource.Filters.Count == 0)
                                this.Pull(syncResource.ResourceType);

                        }
                    var alertService = ApplicationContext.Current.GetService<IAlertService>();
                    alertService?.BroadcastAlert(new AlertMessage(this.m_devicePrincipal.Identity.Name, null, Strings.locale_importDoneSubject, Strings.locale_importDoneBody, AlertMessageFlags.System));
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Cannot process startup command: {0}", e);
                }

            });

            this.Started?.Invoke(this, EventArgs.Empty);

            return true;

        }

        /// <summary>
        /// Perform a fetch operation which performs a head
        /// </summary>
        public bool Fetch(Type modelType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a pull on the root resource
        /// </summary>
        public void Pull(Type modelType)
        {
            this.Pull(modelType, new NameValueCollection());

        }

        /// <summary>
        /// Pull the model according to a filter
        /// </summary>
        public void Pull(Type modelType, NameValueCollection filter)
        {
            try
            {

                ApplicationContext.Current.SetProgress(String.Format(Strings.locale_sync, modelType.Name), 0);
                this.m_tracer.TraceInfo("Start synchronization on {0} (filter:{1})...", modelType, filter);

                // TODO: Clean this up - Login as device account
                if (this.m_devicePrincipal == null ||
                    DateTime.Parse((this.m_devicePrincipal as ClaimsPrincipal)?.FindClaim(ClaimTypes.Expiration)?.Value ?? "0001-01-01") < DateTime.Now)
                    this.m_devicePrincipal = ApplicationContext.Current.GetService<IIdentityProviderService>().Authenticate("Administrator", "Mohawk123");
                var credentials = ApplicationContext.Current.Configuration.GetServiceDescription("imsi").Binding.Security.CredentialProvider.GetCredentials(this.m_devicePrincipal);

                // Get last modified date
                var lastModificationDate = SynchronizationLog.Current.GetLastTime(modelType, filter.ToString());
                this.m_tracer.TraceVerbose("Synchronize all on {0} since {1}", modelType, lastModificationDate);

                var result = new Bundle() { TotalResults = 1 };
                var eTag = String.Empty;
                // Enqueue
                for (int i = result.Count; i < result.TotalResults; i += result.Count)
                {
                    float perc = i / (float)result.TotalResults;
                    ApplicationContext.Current.SetProgress(String.Format(Strings.locale_sync, modelType.Name), perc);
                    result = this.m_integrationService.Find(modelType, new NameValueCollection(), i, 25, new IntegrationQueryOptions() { IfModifiedSince = lastModificationDate, Credentials = credentials });

                    // Queue the act of queueing

                    SynchronizationQueue.Inbound.Enqueue(result, DataOperationType.Sync);

                    if (String.IsNullOrEmpty(eTag))
                        eTag = result.Item.FirstOrDefault()?.Tag;
                }

                // Log that we synchronized successfully
                SynchronizationLog.Current.Save(modelType, filter.ToString(), eTag);

            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error synchronizing {0} : {1} ", modelType, e);
            }
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
