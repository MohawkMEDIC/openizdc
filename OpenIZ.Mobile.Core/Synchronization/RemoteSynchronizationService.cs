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
using System.Diagnostics;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Core.Http;

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
        /// Gets whether the object is synchronizing
        /// </summary>
        public bool IsSynchronizing
        {
            get; private set;
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
                this.m_configuration.PollInterval != default(TimeSpan))
            {
                Action<Object> pollFn = null;
                pollFn = _ =>
                {
                    this.Pull(SynchronizationPullTriggerType.PeriodicPoll);
                    ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(this.m_configuration.PollInterval, pollFn, null);

                };
                ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(this.m_configuration.PollInterval, pollFn, null);
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
                        this.IsSynchronizing = true;

                        DateTime lastSync = DateTime.MinValue;
                        if (SynchronizationLog.Current.GetAll().Count() > 0)
                            lastSync = SynchronizationLog.Current.GetAll().Min(o => o.LastSync);

                        // Trigger
                        if (!this.m_integrationService.IsAvailable())
                        {
                            if (trigger == SynchronizationPullTriggerType.OnStart)
                                ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(new TimeSpan(0, 5, 0), o => this.Pull(SynchronizationPullTriggerType.OnStart), null);
                            return;
                        }

                        int totalResults = 0;
                        var syncTargets = this.m_configuration.SynchronizationResources.Where(o => (o.Triggers & trigger) != 0).ToList();
                        for (var i = 0; i < syncTargets.Count; i++)
                        {

                            var syncResource = syncTargets[i];

                            ApplicationContext.Current.SetProgress(Strings.locale_startingPoll, (float)i / syncTargets.Count);
                            foreach (var fltr in syncResource.Filters)
                                totalResults += this.Pull(syncResource.ResourceType, NameValueCollection.ParseQueryString(fltr), syncResource.Always);
                            if (syncResource.Filters.Count == 0)
                                totalResults += this.Pull(syncResource.ResourceType);

                        }
                        ApplicationContext.Current.SetProgress(String.Empty, 0);

                        // Pull complete?
                        this.IsSynchronizing = false;

                        if (totalResults > 0 && initialSync)
                        {
                            var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
                            alertService?.BroadcastAlert(new AlertMessage(AuthenticationContext.Current.Principal.Identity.Name, "everyone", Strings.locale_importDoneSubject, Strings.locale_importDoneBody, AlertMessageFlags.System));
                            this.PullCompleted?.Invoke(this, new SynchronizationEventArgs(true, totalResults, lastSync));
                        }
                        else if (totalResults > 0)
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
            return this.Pull(modelType, new NameValueCollection(), false);

        }

        /// <summary>
        /// Pull the model according to a filter
        /// </summary>
        public int Pull(Type modelType, NameValueCollection filter)
        {
            return this.Pull(modelType, filter, false);
        }

        /// <summary>
        /// Pull with always filter
        /// </summary>
        public int Pull(Type modelType, NameValueCollection filter, bool always)
        {
            lock (this.m_lock)
            {
                var lastModificationDate = SynchronizationLog.Current.GetLastTime(modelType, filter.ToString());
                if (always)
                    lastModificationDate = null;
                if (lastModificationDate != null)
                    lastModificationDate = lastModificationDate.Value.AddMinutes(-1);

                // Performance timer for more intelligent query control
                Stopwatch perfTimer = new Stopwatch();
                EventHandler<RestResponseEventArgs> respondingHandler = (o, e) => perfTimer.Stop();
                this.m_integrationService.Responding += respondingHandler;

                try
                {

                    this.m_tracer.TraceInfo("Start synchronization on {0} (filter:{1})...", modelType, filter);

                    // Get last modified date
                    this.m_tracer.TraceVerbose("Synchronize all on {0} since {1}", modelType, lastModificationDate);

                    var result = new Bundle() { TotalResults = 1 };
                    var eTag = String.Empty;
                    var retVal = 0;
                    int count = 100;
                    var qid = Guid.NewGuid();

                    // Attempt to find an existing query
                    var existingQuery = SynchronizationLog.Current.FindQueryData(modelType, filter.ToString());
                    if (existingQuery != null && DateTime.Now.Subtract(existingQuery.StartTime).TotalHours <= 1)
                    {
                        qid = new Guid(existingQuery.Uuid);
                        result.Count = existingQuery.LastSuccess;
                        result.TotalResults = result.Count + 1;
                    }
                    else
                    {
                        if (existingQuery != null) SynchronizationLog.Current.CompleteQuery(new Guid(existingQuery.Uuid));
                        SynchronizationLog.Current.SaveQuery(modelType, filter.ToString(), qid, 0);
                    }

                    // Enqueue
                    for (int i = result.Count; i < result.TotalResults; i += result.Count)
                    {
                        float perc = i / (float)result.TotalResults;

                        if (result.TotalResults > result.Offset + result.Count + 1)
                            ApplicationContext.Current.SetProgress(String.Format(Strings.locale_sync, modelType.Name, i, result.TotalResults), perc);
                        NameValueCollection infopt = null;
                        if (filter.Any(o => o.Key.StartsWith("_")))
                        {
                            infopt = new NameValueCollection();
                            foreach (var itm in filter.Where(o => o.Key.StartsWith("_")))
                                infopt.Add(itm.Key, itm.Value);
                        }

                        perfTimer.Reset();
                        perfTimer.Start();
                        result = this.m_integrationService.Find(modelType, i == 0 ? filter : new NameValueCollection(), i, count, new IntegrationQueryOptions() { IfModifiedSince = lastModificationDate, Timeout = 120000, Lean = true, InfrastructureOptions = infopt, QueryId = qid });

                        // Queue the act of queueing
                        if (result != null)
                        {
                            if (count == 2500 && perfTimer.ElapsedMilliseconds < 40000 ||
                                count < 2000 && result.TotalResults > 10000 && perfTimer.ElapsedMilliseconds < 20000)
                                count = 1000;
                            else if (count == 1000 && perfTimer.ElapsedMilliseconds < 20000 ||
                                count < 1000 && result.TotalResults > 5000 && perfTimer.ElapsedMilliseconds < 10000)
                                count = 1000;
                            else if (count == 200 && perfTimer.ElapsedMilliseconds < 15000 ||
                                count < 500 && result.TotalResults > 1000 && perfTimer.ElapsedMilliseconds < 10000)
                                count = 500;
                            else
                                count = 100;

                            this.m_tracer.TraceVerbose("Download {0} ({1}..{2}/{3})", modelType.FullName, i, i + result.Count, result.TotalResults);
                            result.Item.RemoveAll(o => o is SecurityUser || o is SecurityRole || o is SecurityPolicy);

                            SynchronizationQueue.Inbound.Enqueue(result, DataOperationType.Sync);
                            SynchronizationLog.Current.SaveQuery(modelType, filter.ToString(), qid, result.Offset + result.Count);

                            retVal = result.TotalResults;
                        }
                        else
                            break;

                        if (String.IsNullOrEmpty(eTag))
                            eTag = result?.Item.FirstOrDefault()?.Tag;
                    }

                    if (result?.TotalResults > result?.Count)
                        ApplicationContext.Current.SetProgress(String.Empty, 0);

                    // Log that we synchronized successfully
                    SynchronizationLog.Current.Save(modelType, filter.ToString(), eTag);

                    // Clear the query
                    SynchronizationLog.Current.CompleteQuery(qid);

                    // Fire the pull event
                    this.PullCompleted?.Invoke(this, new SynchronizationEventArgs(modelType, filter, lastModificationDate.GetValueOrDefault(), retVal));

                    return retVal;
                }
                catch (TargetInvocationException ex)
                {
                    var e = ex.InnerException;
                    this.m_tracer.TraceError("Error synchronizing {0} : {1} ", modelType, e);
                    var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
                    alertService?.BroadcastAlert(new AlertMessage(AuthenticationContext.Current.Principal.Identity.Name ?? "System", "everyone", Strings.locale_downloadError, String.Format(Strings.locale_downloadErrorBody, e), AlertMessageFlags.Transient));
                    this.PullCompleted?.Invoke(this, new SynchronizationEventArgs(modelType, filter, lastModificationDate.GetValueOrDefault(), 0));

                    return 0;

                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error synchronizing {0} : {1} ", modelType, e);
                    var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
                    alertService?.BroadcastAlert(new AlertMessage(AuthenticationContext.Current.Principal.Identity.Name ?? "System", "everyone", Strings.locale_downloadError, String.Format(Strings.locale_downloadErrorBody, e), AlertMessageFlags.Transient));
                    this.PullCompleted?.Invoke(this, new SynchronizationEventArgs(modelType, filter, lastModificationDate.GetValueOrDefault(), 0));

                    return 0;
                }
                finally
                {
                    this.m_integrationService.Responding -= respondingHandler;
                }
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
