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
 * Date: 2016-7-13
 */
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Synchronization.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Synchronization
{
    /// <summary>
    /// Queue manager daemon
    /// </summary>
    public class SynchronizationManagerService : IDaemonService
    {

        // Queue manager 
        private Tracer m_tracer = Tracer.GetTracer(typeof(SynchronizationManagerService));
        private Object m_inboundLock = new object();
        private Object m_outboundLock = new object();
        private IThreadPoolService m_threadPool = null;

        /// <summary>
        /// Events surrounding the daemon
        /// </summary>
        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
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
        /// Import element
        /// </summary>
        private void ImportElement(IdentifiedData data, InboundQueueEntry queueEntry)
        {
            var idpType = typeof(IDataPersistenceService<>).MakeGenericType(data.GetType());
            var svc = OpenIZ.Mobile.Core.ApplicationContext.Current.GetService(idpType) as IDataPersistenceService;
            try
            {

                this.m_tracer.TraceVerbose("Inserting object from inbound queue: {0}", data);
                if (svc.Get(data.Key.Value) == null)
                    svc.Insert(data);
                else
                    svc.Update(data);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error inserting object data: {0}", e);
                SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(queueEntry));
            }
        }

        /// <summary>
        /// Exhausts the inbound queue
        /// </summary>
        public void ExhaustInboundQueue()
        {
            if (!Monitor.TryEnter(this.m_inboundLock)) return;

            lock (this.m_inboundLock)
            {
                // Exhaust the queue
                while (SynchronizationQueue.Inbound.Count() > 0)
                {
                    var queueEntry = SynchronizationQueue.Inbound.DequeueRaw();
                    var dpe = SynchronizationQueue.Inbound.DeserializeObject(queueEntry);
                    (dpe as OpenIZ.Core.Model.Collection.Bundle)?.Reconstitute();
                    dpe = (dpe as OpenIZ.Core.Model.Collection.Bundle)?.Entry ?? dpe;

                    if (dpe is OpenIZ.Core.Model.Collection.Bundle) // We'll have to iterate and insert
                    {
                        foreach (var dat in (dpe as OpenIZ.Core.Model.Collection.Bundle).Item)
                            this.ImportElement(dat, queueEntry);
                    }
                    else
                        this.ImportElement(dpe, queueEntry);
                }
            }
        }

        /// <summary>
        /// Exhaust the outbound queue
        /// </summary>
        public void ExhaustOutboundQueue()
        {
            if (!Monitor.TryEnter(this.m_outboundLock)) return;

            lock (this.m_outboundLock)
            {
                // Exhaust the queue
                while (SynchronizationQueue.Outbound.Count() > 0)
                {
                    // Exhaust the outbound queue
                    var integrationService = OpenIZ.Mobile.Core.ApplicationContext.Current.GetService<IIntegrationService>();
                    var syncItm = SynchronizationQueue.Outbound.PeekRaw();
                    var dpe = SynchronizationQueue.Outbound.DeserializeObject(syncItm);

                    // TODO: Sleep thread here
                    while (!integrationService.IsAvailable())
                        ;

                    // try to send
                    try
                    {
                        // Send the object to the remote host
                        switch (syncItm.Operation)
                        {
                            case DataOperationType.Insert:
                                integrationService.Insert(dpe);
                                break;
                            case DataOperationType.Obsolete:
                                integrationService.Obsolete(dpe);
                                break;
                            case DataOperationType.Update:
                                integrationService.Update(dpe);
                                break;
                        }

                        SynchronizationQueue.Outbound.DequeueRaw(); // Get rid of object from queue
                    }
                    catch (WebException ex)
                    {
                        this.m_tracer.TraceError("Remote server rejected object: {0}", ex);
                        SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm));
                        SynchronizationQueue.Outbound.DequeueRaw();
                    }
                    catch (TimeoutException ex) // Timeout due to lack of connectivity
                    {
                        if (syncItm.RetryCount == 0)
                            this.m_tracer.TraceError("Error sending object {0}: {1}", dpe, ex);
                        syncItm.RetryCount++;
                        // Re-queue
                        if (syncItm.RetryCount > 3) // TODO: Make this configurable
                        {
                            SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm));
                            SynchronizationQueue.Outbound.DequeueRaw(); // Get rid of the last item
                        }
                        else
                            SynchronizationQueue.Outbound.UpdateRaw(syncItm);

                    }
                }
            }
        }

        /// <summary>
        /// Start command
        /// </summary>
        public bool Start()
        {

            this.Starting?.Invoke(this, EventArgs.Empty);

            this.m_threadPool = ApplicationContext.Current.GetService<IThreadPoolService>();

            // Bind to the inbound queue
            SynchronizationQueue.Inbound.Enqueued += (o, e) =>
            {
                Action<Object> async = (itm) =>
                {
                    this.ExhaustInboundQueue();
                };
                this.m_threadPool.QueueUserWorkItem(async);
            };

            // Bind to outbound queue
            SynchronizationQueue.Outbound.Enqueued += (o, e) =>
            {
                Action<Object> async = (itm) =>
                {
                    this.ExhaustOutboundQueue();
                };
                this.m_threadPool.QueueUserWorkItem(async);

            };

            // startup
            AsyncCallback startup = (iar) =>
            {
                this.ExhaustOutboundQueue();
                this.ExhaustInboundQueue();
            };
            startup.BeginInvoke(null, null, null);


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
}
