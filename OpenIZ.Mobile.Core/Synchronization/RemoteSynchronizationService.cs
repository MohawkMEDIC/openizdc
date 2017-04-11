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
 * Date: 2016-7-30
 */
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
using OpenIZ.Core.Services;
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using System.Reflection;

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
        private IClinicalIntegrationService m_integrationService;
        // Network information service
        private INetworkInformationService m_networkInfoService;

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
        /// Pull has completed
        /// </summary>
        public event EventHandler<SynchronizationEventArgs> PullCompleted;

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
            this.m_integrationService = ApplicationContext.Current.GetService<IClinicalIntegrationService>();
            this.m_networkInfoService = ApplicationContext.Current.GetService<INetworkInformationService>();

            this.m_networkInfoService.NetworkStatusChanged += (o, e) => this.Pull(SynchronizationPullTriggerType.OnNetworkChange);

            this.Pull(SynchronizationPullTriggerType.OnStart);

            // Polling
            if (this.m_configuration.SynchronizationResources.Any(o => (o.Triggers & SynchronizationPullTriggerType.PeriodicPoll) != 0) &&
                this.m_configuration.PollInterval.HasValue &&
                this.m_configuration.PollInterval.Value != default(TimeSpan))
            {
                Action<Object> pollFn = null;
                pollFn = _ =>
                {
                    ApplicationContext.Current.SetProgress(Strings.locale_startingPoll, 0.5f);
                    this.Pull(SynchronizationPullTriggerType.PeriodicPoll);
                    ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(this.m_configuration.PollInterval.Value, pollFn, null);

                };
                ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(this.m_configuration.PollInterval.Value, pollFn, null);
            }
            this.Started?.Invoke(this, EventArgs.Empty);

            return true;

        }

        /// <summary>
        /// Pull from remote
        /// </summary>
        private void Pull(SynchronizationPullTriggerType trigger)
        {
            // Pool startup sync if configured..
            this.m_threadPool.QueueUserWorkItem((state) =>
            {

                bool initialSync = !SynchronizationLog.Current.GetAll().Any();

                if (Monitor.TryEnter(this.m_lock, 100)) // Do we have a lock?
                {
                    try
                    {

                        DateTime lastSync = DateTime.MinValue;
                        if(SynchronizationLog.Current.GetAll().Count() > 0)
                            lastSync = SynchronizationLog.Current.GetAll().Min(o => o.LastSync);

                        if (!this.m_integrationService.IsAvailable()) return;

                        int totalResults = 0;
                        foreach (var syncResource in this.m_configuration.SynchronizationResources.Where(o => (o.Triggers & trigger) != 0))
                        {

                            foreach (var fltr in syncResource.Filters)
                                totalResults += this.Pull(syncResource.ResourceType, NameValueCollection.ParseQueryString(fltr));
                            if (syncResource.Filters.Count == 0)
                                totalResults += this.Pull(syncResource.ResourceType);

                        }

                        // Pull complete?
                        if (totalResults > 0 && initialSync)
                        {
                            var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
                            alertService?.BroadcastAlert(new AlertMessage(AuthenticationContext.Current.Principal.Identity.Name, "everyone", Strings.locale_importDoneSubject, Strings.locale_importDoneBody, AlertMessageFlags.System));
                            this.PullCompleted?.Invoke(this, new SynchronizationEventArgs(true, totalResults, lastSync));
                        }
                        else if(totalResults > 0)
                            this.PullCompleted?.Invoke(this, new SynchronizationEventArgs(totalResults, lastSync));


                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Cannot process startup command: {0}", e);
                    }
                    finally
                    {
                        Monitor.Exit(this.m_lock);
                    }
                }
            });

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
        public int Pull(Type modelType)
        {
            return this.Pull(modelType, new NameValueCollection());

        }

        /// <summary>
        /// Pull the model according to a filter
        /// </summary>
        public int Pull(Type modelType, NameValueCollection filter)
        {
            var lastModificationDate = SynchronizationLog.Current.GetLastTime(modelType, filter.ToString());

            try
            {

                ApplicationContext.Current.SetProgress(String.Format(Strings.locale_sync, modelType.Name), 0);
                this.m_tracer.TraceInfo("Start synchronization on {0} (filter:{1})...", modelType, filter);

                // Get last modified date
                this.m_tracer.TraceVerbose("Synchronize all on {0} since {1}", modelType, lastModificationDate);

                var result = new Bundle() { TotalResults = 1 };
                var eTag = String.Empty;
                var retVal = 0;
                int count = 75;

                var qid = Guid.NewGuid();

                // Enqueue
                for (int i = result.Count; i < result.TotalResults; i += result.Count)
                {
                    float perc = i / (float)result.TotalResults;

                    ApplicationContext.Current.SetProgress(String.Format(Strings.locale_sync, modelType.Name), perc);
                    NameValueCollection infopt = null;
                    if (filter.Any(o => o.Key.StartsWith("_")))
                    {
                        infopt = new NameValueCollection();
                        foreach (var itm in filter.Where(o => o.Key.StartsWith("_")))
                            infopt.Add(itm.Key, itm.Value);
                    }
                    result = this.m_integrationService.Find(modelType, filter, i, count, new IntegrationQueryOptions() { IfModifiedSince = lastModificationDate, Timeout = 30000, Lean = true, InfrastructureOptions = infopt, QueryId = qid });

                    // Queue the act of queueing
                    if (result != null)
                    {
                        if (result.TotalResults > 5000)
                            count = 1000;
                        else if (result.TotalResults > 1000)
                            count = 200;
                        this.m_tracer.TraceVerbose("Download {0} ({1}..{2}/{3})", modelType.FullName, i, i + result.Count, result.TotalResults);
                        result.Item.RemoveAll(o => o is SecurityUser || o is SecurityRole || o is SecurityPolicy);

                        ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(o => { SynchronizationQueue.Inbound.Enqueue(o as IdentifiedData, DataOperationType.Sync); }, result);
                        
                        retVal = result.TotalResults;
                    }
                    else
                        break;

                    if (String.IsNullOrEmpty(eTag))
                        eTag = result?.Item.FirstOrDefault()?.Tag;
                }

                // Log that we synchronized successfully
                SynchronizationLog.Current.Save(modelType, filter.ToString(), eTag);

                // Fire the pull event
                this.PullCompleted?.Invoke(this, new SynchronizationEventArgs(modelType, filter, lastModificationDate.GetValueOrDefault(), retVal));

                return retVal;
            }
            catch(TargetInvocationException ex)
            {
                var e = ex.InnerException;
                this.m_tracer.TraceError("Error synchronizing {0} : {1} ", modelType, e);
                var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
                alertService?.BroadcastAlert(new AlertMessage(AuthenticationContext.Current.Principal.Identity.Name ?? "System", "everyone", Strings.locale_downloadError, String.Format(Strings.locale_downloadErrorBody, e), AlertMessageFlags.System));
                this.PullCompleted?.Invoke(this, new SynchronizationEventArgs(modelType, filter, lastModificationDate.GetValueOrDefault(), 0));

                return 0;

            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error synchronizing {0} : {1} ", modelType, e);
                var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
                alertService?.BroadcastAlert(new AlertMessage(AuthenticationContext.Current.Principal.Identity.Name ?? "System", "everyone", Strings.locale_downloadError, String.Format(Strings.locale_downloadErrorBody, e), AlertMessageFlags.System));
                this.PullCompleted?.Invoke(this, new SynchronizationEventArgs(modelType, filter, lastModificationDate.GetValueOrDefault(), 0));

                return 0;
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
