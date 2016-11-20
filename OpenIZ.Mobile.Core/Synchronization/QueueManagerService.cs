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
 * Date: 2016-7-30
 */
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Interfaces;
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

		private IThreadPoolService m_threadPool = null;

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
		public bool IsRunning
		{
			get
			{
				return true;
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
				while (remain > 0)
				{
					try
					{
						ApplicationContext.Current.SetProgress(String.Format("{0} - [{1}]", Strings.locale_import, remain), (float)1 / (float)remain);

						this.m_tracer.TraceInfo("{0} remaining inbound queue items", SynchronizationQueue.Inbound.Count());
						var queueEntry = SynchronizationQueue.Inbound.PeekRaw();
						var dpe = SynchronizationQueue.Inbound.DeserializeObject(queueEntry);
						//(dpe as OpenIZ.Core.Model.Collection.Bundle)?.Reconstitute();
						dpe = (dpe as OpenIZ.Core.Model.Collection.Bundle)?.Entry ?? dpe;

						this.ImportElement(dpe, queueEntry);
						SynchronizationQueue.Inbound.DequeueRaw();
					}
					catch (Exception e)
					{
						this.m_tracer.TraceError("Error processing inbound queue entry: {0}", e);
						SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(SynchronizationQueue.Inbound.DequeueRaw(), Encoding.UTF8.GetBytes(e.ToString())) { OriginalQueue = "In" });
					}
					remain = SynchronizationQueue.Inbound.Count();
				}
			}
			catch (TimeoutException e)
			{

			}
			finally
			{
				if (locked) Monitor.Exit(this.m_inboundLock);
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
				// Exhaust the queue
				while (SynchronizationQueue.Outbound.Count() > 0)
				{
					// Exhaust the outbound queue
					var integrationService = OpenIZ.Mobile.Core.ApplicationContext.Current.GetService<IIntegrationService>();
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
						SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm, Encoding.UTF8.GetBytes(ex.ToString())));
						SynchronizationQueue.Outbound.DequeueRaw();
					}
					catch (TimeoutException ex) // Timeout due to lack of connectivity
					{
						if (syncItm.RetryCount == 0)
						{
							this.m_tracer.TraceError("Error sending object {0}: {1}", dpe, ex);
						}

						syncItm.RetryCount++;

						// Re-queue
						if (syncItm.RetryCount > 3) // TODO: Make this configurable
						{
							SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(syncItm, Encoding.UTF8.GetBytes(ex.ToString())));
							SynchronizationQueue.Outbound.DequeueRaw(); // Get rid of the last item
						}
						else
						{
							SynchronizationQueue.Outbound.UpdateRaw(syncItm);
						}
					}
				}
			}
            finally
            {
                if (locked) Monitor.Exit(this.m_outboundLock);
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
				var existing = svc.Get(data.Key.Value) as IdentifiedData;
				(existing as IdentifiedData)?.SetDelayLoad(false);
				data?.SetDelayLoad(false);

				this.m_tracer.TraceVerbose("Inserting object from inbound queue: {0}", data);
				if (existing == null)
					try
					{
						svc.Insert(data);
					}
					catch (Exception e)
					{
						this.m_tracer.TraceWarning("Batch insert fails, performing individual inserts");

						if (data is Bundle)
						{
							foreach (var d in (data as Bundle).Item)
							{
								this.ImportElement(d as IdentifiedData, queueEntry);
								SynchronizationQueue.Inbound.Enqueue(d as IdentifiedData, DataOperationType.Sync); // Queue this up for later
							}
						}
						else
						{
							throw;
						}
					}
				else
				{
					IVersionedEntity ver = data as IVersionedEntity;
					if (ver?.VersionKey == (existing as IVersionedEntity)?.VersionKey) // no need to update
						this.m_tracer.TraceVerbose("Object {0} is already up to date", existing);
					if (ver?.PreviousVersionKey != null && ver?.PreviousVersionKey != (existing as IVersionedEntity)?.VersionKey ||
						data.GetType() != existing.GetType()) // Conflict, ask the meatbag to resolve it
					{
						XmlSerializer xsz = new XmlSerializer(existing.GetType());
						using (var ms = new MemoryStream())
						{
							xsz.Serialize(ms, existing);
							SynchronizationQueue.DeadLetter.EnqueueRaw(new DeadLetterQueueEntry(queueEntry, ms.ToArray()));
						}
					}
					else
					{
						svc.Update(data);
					}
				}
			}
			catch (Exception e)
			{
				this.m_tracer.TraceError("Error inserting object data: {0}", e);
				var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
				alertService?.BroadcastAlert(new AlertMessage("SYSTEM", null, Strings.locale_importErrorSubject, String.Format(Strings.locale_importErrorBody, e), AlertMessageFlags.Alert));

				SynchronizationQueue.DeadLetter.Enqueue(data, DataOperationType.Sync);
				// throw;
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
				if (ApplicationContext.Current.Configuration.GetSection<SynchronizationConfigurationSection>().SynchronizationResources.
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

			ApplicationContext.Current.Started += (o, e) =>
			{
				// startup
				AsyncCallback startup = (iar) =>
				{
					this.ExhaustOutboundQueue();
					this.ExhaustInboundQueue();
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
}
