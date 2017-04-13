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
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Patch;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Alerting;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Resources;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Synchronization.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Synchronization
{

    /// <summary>
    /// Queue manager daemon
    /// </summary>
    public class QueueManagerService : IDaemonService
    {

        private Object m_inboundLock = new object();
        private Object m_outboundLock = new object();
        private Object m_adminLock = new object();

        private IThreadPoolService m_threadPool = null;

        /// <summary>
        /// Queue has been exhuasted
        /// </summary>
        public event EventHandler<QueueExhaustedEventArgs> QueueExhausted;

        // Queue manager 
        private Tracer m_tracer = Tracer.GetTracer(typeof(QueueManagerService));
        public event EventHandler Started;

        /// <summary>
        /// Events surrounding the daemon
        /// </summary>
        public event EventHandler Starting;
        public event EventHandler Stopped;

        public event EventHandler Stopping;
        /// <summary>
        /// Returns true if the service is running
        /// </summary>
        public bool IsRunning => true;

        /// <summary>
        /// True if synchronization is occurring
        /// </summary>
        public bool IsSynchronizing { get
            {
                return Monitor.IsEntered(this.m_inboundLock) || Monitor.IsEntered(this.m_outboundLock);
            }
        }

        /// <summary>
        /// Exhausts the inbound queue
        /// </summary>
        public void ExhaustInboundQueue()
        {
            bool locked = false;
            try
            {
                locked = Monitor.TryEnter(this.m_inboundLock, 100);
                if (!locked) return;

                // Exhaust the queue
                int remain = SynchronizationQueue.Inbound.Count();
                int maxTotal = 0;
                InboundQueueEntry nextPeek = null;
                IdentifiedData nextDpe = null;

                while (remain > 0)
                {
                    InboundQueueEntry queueEntry = null;

                    try
                    {
                        if (remain > maxTotal)
                            maxTotal = remain;

                        if(maxTotal > 5)
                            ApplicationContext.Current.SetProgress(String.Format("{0} - [{1}]", Strings.locale_import, remain), (maxTotal - remain) / (float)maxTotal);
                        
#if PERFMON
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
#endif
                        IdentifiedData dpe = null;
                        if (nextPeek != null) // Was this loaded before? {
                        {
                            queueEntry = nextPeek;
                            dpe = nextDpe;
                        }
                        else
                        {
                            queueEntry = SynchronizationQueue.Inbound.PeekRaw();
                            dpe = SynchronizationQueue.Inbound.DeserializeObject(queueEntry);
                        }

                        // Try to peek off the next queue item while we're doing something else
                        var nextPeekTask = Task<KeyValuePair<InboundQueueEntry, IdentifiedData>>.Run(() =>
                        {
                            var nextRaw = SynchronizationQueue.Inbound.PeekRaw(1);
                            return new KeyValuePair<InboundQueueEntry, IdentifiedData>(nextRaw, nextRaw == null ? null : SynchronizationQueue.Inbound.DeserializeObject(nextRaw));
                        });
                        

#if PERFMON
                        sw.Stop();
                        ApplicationContext.Current.PerformanceLog(nameof(QueueManagerService), nameof(ExhaustInboundQueue), "DeQueue", sw.Elapsed);
                        sw.Reset();
                        sw.Start();
#endif
                        

                        //(dpe as OpenIZ.Core.Model.Collection.Bundle)?.Reconstitute();
                        var bundle = dpe as Bundle;
                        dpe = bundle?.Entry ?? dpe;

                        if(bundle?.Item.Count > 500)
                        {
                            var ofs = 0;
                            while(ofs < bundle.Item.Count)
                            {
                                this.ImportElement(new Bundle()
                                {
                                    Item = bundle.Item.Skip(ofs).Take(250).ToList()
                                });
                                ofs += 500;
                            }
                        }
                        else
                            this.ImportElement(dpe);

                        this.QueueExhausted?.BeginInvoke(this, new QueueExhaustedEventArgs("inbound", bundle?.Item.AsParallel().Select(o => o.Key.Value).ToArray() ?? new Guid[] { dpe.Key.Value }), null, null);

#if PERFMON
                        sw.Stop();
                        ApplicationContext.Current.PerformanceLog(nameof(QueueManagerService), nameof(ExhaustInboundQueue), "ImportComplete", sw.Elapsed);
                        sw.Reset();
#endif
                        queueEntry = null;

                        var peekTaskResult = nextPeekTask.Result;
                        nextPeek = peekTaskResult.Key;
                        nextDpe = peekTaskResult.Value;

                    }
                    catch (Exception e)
                    {
                        try
                        {
                            this.m_tracer.TraceError("Error processing inbound queue entry: {0}", e);
                            SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(queueEntry, Encoding.UTF8.GetBytes(e.ToString())) { OriginalQueue = "inbound" });
                        }
                        catch(Exception e2)
                        {
                            this.m_tracer.TraceEvent(System.Diagnostics.Tracing.EventLevel.Critical, "Error putting dead item on deadletter queue: {0}", e);
                            throw;
                        }
                    }
                    finally
                    {
                        SynchronizationQueue.Inbound.Delete(queueEntry.Id);
                    }
                    remain = SynchronizationQueue.Inbound.Count();

                }

                if(maxTotal > 5)
                    ApplicationContext.Current.SetProgress(String.Format("{0} - [0]", Strings.locale_import, remain), 0);
       
            }
            finally
            {
                if (locked) Monitor.Exit(this.m_inboundLock);
            }
        }

        /// <summary>
        /// Exhaust the administrative queue
        /// </summary>
        private void ExhaustAdminQueue()
        {
            bool locked = false;
            try
            {
                locked = Monitor.TryEnter(this.m_adminLock, 100);
                if (!locked) return;
                // Exhaust the queue
                while (SynchronizationQueue.Admin.Count() > 0)
                {
                    // Exhaust the outbound queue
                    var amiService = OpenIZ.Mobile.Core.ApplicationContext.Current.GetService<IAdministrationIntegrationService>();
                    var syncItm = SynchronizationQueue.Admin.PeekRaw();
                    var dpe = SynchronizationQueue.Admin.DeserializeObject(syncItm);

                    // TODO: Sleep thread here
                    if (!amiService.IsAvailable())
                    {
                        // Come back in 30 seconds...
                        this.m_threadPool.QueueUserWorkItem(new TimeSpan(0, 0, 30), (o) => this.ExhaustOutboundQueue(), null);
                        return;
                    }

                    // try to send
                    try
                    {
                        // Reconstitute bundle
                        (dpe as Bundle)?.Reconstitute();
                        dpe = (dpe as Bundle)?.Entry ?? dpe;

                        // Send the object to the remote host
                        switch (syncItm.Operation)
                        {
                            case DataOperationType.Insert:
                                amiService.Insert(dpe);
                                break;
                            case DataOperationType.Obsolete:
                                amiService.Obsolete(dpe, syncItm.IsRetry);
                                break;
                            case DataOperationType.Update:
                                amiService.Update(dpe, syncItm.IsRetry);
                                break;
                        }


                        SynchronizationQueue.Admin.Delete(syncItm.Id); // Get rid of object from queue
                    }
                    catch (WebException ex)
                    {
                        this.m_tracer.TraceError("Remote server rejected object: {0}", ex);
                        SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm, Encoding.UTF8.GetBytes(ex.ToString())));
                        SynchronizationQueue.Outbound.DequeueRaw();

                        // Construct an alert
                        this.CreateUserAlert(Strings.locale_rejectionSubject, Strings.locale_rejectionBody, String.Format(Strings.ResourceManager.GetString((ex.Response as HttpWebResponse)?.StatusDescription ?? "locale_syncErrorBody"), ex, dpe), dpe);
                    }
                    catch (TimeoutException ex) // Timeout due to lack of connectivity
                    {

                        this.m_tracer.TraceError("Error sending object {0}: {1}", dpe, ex);

                        syncItm.IsRetry = false;

                        // Re-queue
                        if (syncItm.RetryCount > 3) // TODO: Make this configurable
                        {
                            SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm, Encoding.UTF8.GetBytes(ex.ToString())));
                            SynchronizationQueue.Outbound.DequeueRaw(); // Get rid of the last item
                            this.CreateUserAlert(Strings.locale_syncErrorSubject, Strings.locale_syncErrorBody, ex, dpe);
                        }
                        else
                        {
                            SynchronizationQueue.Outbound.UpdateRaw(syncItm);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.m_tracer.TraceError("Error sending object to AMI: {0}", ex);
                        this.CreateUserAlert(Strings.locale_syncErrorSubject, Strings.locale_syncErrorBody, ex, dpe);
                        SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm, Encoding.UTF8.GetBytes(ex.ToString())));
                        SynchronizationQueue.Outbound.DequeueRaw();

                        throw;
                    }
                }
                this.QueueExhausted?.Invoke(this, new QueueExhaustedEventArgs("admin"));

            }
            finally
            {
                if (locked) Monitor.Exit(this.m_adminLock);
            }
        }

        /// <summary>
        /// Exhaust the outbound queue
        /// </summary>
        public void ExhaustOutboundQueue()
        {
            bool locked = false;
            try
            {
                locked = Monitor.TryEnter(this.m_outboundLock, 100);
                if (!locked) return;
                List<Guid> exportKeys = new List<Guid>();
                // Exhaust the queue
                while (SynchronizationQueue.Outbound.Count() > 0)
                {
                    // Exhaust the outbound queue
                    var integrationService = OpenIZ.Mobile.Core.ApplicationContext.Current.GetService<IClinicalIntegrationService>();
                    var syncItm = SynchronizationQueue.Outbound.PeekRaw();
                    var dpe = SynchronizationQueue.Outbound.DeserializeObject(syncItm);

                    // TODO: Sleep thread here
                    if (!integrationService.IsAvailable())
                    {
                        // Come back in 30 seconds...
                        this.m_threadPool.QueueUserWorkItem(new TimeSpan(0, 0, 30), (o) => this.ExhaustOutboundQueue(), null);
                        return;
                    }

                    // try to send
                    try
                    {
                        // Reconstitute bundle
                        var bundle = dpe as Bundle;
                        bundle?.Reconstitute();
                        dpe = bundle?.Entry ?? dpe;

                        // Send the object to the remote host
                        switch (syncItm.Operation)
                        {
                            case DataOperationType.Insert:
                                integrationService.Insert(dpe);
                                break;
                            case DataOperationType.Obsolete:
                                integrationService.Obsolete(dpe, syncItm.IsRetry);
                                break;
                            case DataOperationType.Update:
                                integrationService.Update(dpe, syncItm.IsRetry);
                                break;
                        }


                        if (bundle != null)
                            exportKeys.AddRange(bundle?.Item.Select(o => o.Key.Value));
                        else
                            exportKeys.Add(dpe.Key.Value);

                        SynchronizationQueue.Outbound.Delete(syncItm.Id); // Get rid of object from queue
                    }
                    catch (WebException ex)
                    {
                        this.m_tracer.TraceError("Remote server rejected object: {0}", ex);
                        SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm, Encoding.UTF8.GetBytes(ex.ToString())));
                        SynchronizationQueue.Outbound.DequeueRaw();

                        // Construct an alert
                        this.CreateUserAlert(Strings.locale_rejectionSubject, Strings.locale_rejectionBody, String.Format(Strings.ResourceManager.GetString((ex.Response as HttpWebResponse)?.StatusCode.ToString()) ?? Strings.locale_syncErrorBody, ex, dpe), dpe);
                    }
                    catch (TimeoutException ex) // Timeout due to lack of connectivity
                    {

                        this.m_tracer.TraceError("Error sending object {0}: {1}", dpe, ex);

                        syncItm.RetryCount++;

                        // Re-queue
                        if (syncItm.RetryCount > 3) // TODO: Make this configurable
                        {
                            SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm, Encoding.UTF8.GetBytes(ex.ToString())));
                            SynchronizationQueue.Outbound.DequeueRaw(); // Get rid of the last item
                            this.CreateUserAlert(Strings.locale_syncErrorSubject, Strings.locale_syncErrorBody, ex, dpe);
                        }
                        else
                        {
                            SynchronizationQueue.Outbound.UpdateRaw(syncItm);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.m_tracer.TraceError("Error sending object to IMS: {0}", ex);
                        this.CreateUserAlert(Strings.locale_syncErrorSubject, Strings.locale_syncErrorBody, ex, dpe);
                        SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm, Encoding.UTF8.GetBytes(ex.ToString())));
                        SynchronizationQueue.Outbound.DequeueRaw();

                        throw;
                    }
                }
                this.QueueExhausted?.Invoke(this, new QueueExhaustedEventArgs("outbound", exportKeys.ToArray()));

            }
            finally
            {
                if (locked) Monitor.Exit(this.m_outboundLock);
            }
        }

        /// <summary>
        /// Create an alert that the user can acknowledge
        /// </summary>
        private void CreateUserAlert(String subject, String body, params object[] parms)
        {
            var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
            alertService?.BroadcastAlert(new AlertMessage("SYSTEM", null, subject, String.Format(body, parms), AlertMessageFlags.System));
        }

        /// <summary>
        /// Import element
        /// </summary>
        private void ImportElement(IdentifiedData data)
        {
            var idpType = typeof(IDataPersistenceService<>).MakeGenericType(data.GetType());
            var svc = OpenIZ.Mobile.Core.ApplicationContext.Current.GetService(idpType) as IDataPersistenceService;
            try
            {
                IdentifiedData existing = null;
                if(!(data is Bundle))
                    existing = svc.Get(data.Key.Value) as IdentifiedData;

                this.m_tracer.TraceVerbose("Inserting object from inbound queue: {0}", data);
                if (existing == null)
                    svc.Insert(data);
                else
                {
                    IVersionedEntity ver = data as IVersionedEntity;
                    if (ver?.VersionKey == (existing as IVersionedEntity)?.VersionKey) // no need to update
                        this.m_tracer.TraceVerbose("Object {0} is already up to date", existing);
                    else
                    {
                        svc.Update(data);
                    }
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error inserting object data: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Starts the queue manager service.
        /// </summary>
        /// <returns>Returns true if the service started successfully.</returns>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            this.m_threadPool = ApplicationContext.Current.GetService<IThreadPoolService>();

            // Bind to the inbound queue
            SynchronizationQueue.Inbound.Enqueued += (o, e) =>
            {
                // Someone already got this!
                if (Monitor.IsEntered(this.m_inboundLock)) return;
                Action<Object> async = (itm) =>
                {
                    this.ExhaustInboundQueue();
                };
                this.m_threadPool.QueueUserWorkItem(async);
            };

            // Bind to outbound queue
            SynchronizationQueue.Outbound.Enqueued += (o, e) =>
            {
                // Trigger sync?
                if (e.Data.Type.StartsWith(typeof(Patch).FullName) ||
                            e.Data.Type.StartsWith(typeof(Bundle).FullName) ||
                            ApplicationContext.Current.Configuration.GetSection<SynchronizationConfigurationSection>().SynchronizationResources.
                    Exists(r => r.ResourceType == Type.GetType(e.Data.Type) &&
                            (r.Triggers & SynchronizationPullTriggerType.OnCommit) != 0))
                {
                    Action<Object> async = (itm) =>
                    {
                        this.ExhaustOutboundQueue();
                    };
                    this.m_threadPool.QueueUserWorkItem(async);
                }
            };

            // Bind to administration queue
            SynchronizationQueue.Admin.Enqueued += (o, e) =>
            {
                // Admin is always pushed
                this.m_threadPool.QueueUserWorkItem(a => this.ExhaustAdminQueue());
            };

            ApplicationContext.Current.Started += (o, e) =>
            {
                // startup
                AsyncCallback startup = (iar) =>
                {
                    try
                    {
                        this.ExhaustOutboundQueue();
                        this.ExhaustInboundQueue();
                        this.ExhaustAdminQueue();
                    }
                    catch (Exception ex)
                    {
                        this.m_tracer.TraceError("Error executing initial queues: {0}", ex);
                    }
                };

                startup.BeginInvoke(null, null, null);
            };


            this.Started?.Invoke(this, EventArgs.Empty);

            return true;
        }


        /// <summary>
        /// Stopping the services
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            this.Stopped?.Invoke(this, EventArgs.Empty);

            return true;

        }
    }

    /// <summary>
    /// Queue has been exhausted
    /// </summary>
    public class QueueExhaustedEventArgs : EventArgs
    {
        /// <summary>
        /// The queue which has been exhausted
        /// </summary>
        public String Queue { get; private set; }

        /// <summary>
        /// Gets or sets the object keys
        /// </summary>
        public IEnumerable<Guid> ObjectKeys { get; private set; }

        /// <summary>
        /// Queue has been exhausted
        /// </summary>
        public QueueExhaustedEventArgs(String queueName, params Guid[] objectKeys)
        {
            this.Queue = queueName;
            this.ObjectKeys = objectKeys;
        }


    }
}
