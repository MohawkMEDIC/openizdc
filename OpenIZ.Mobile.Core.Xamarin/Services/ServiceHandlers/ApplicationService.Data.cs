/*
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
using OpenIZ.Core.Model.Query;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Synchronization.Model;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using OpenIZ.Mobile.Core.Serices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Services;
using System.IO;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using Newtonsoft.Json.Linq;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Xamarin.Data;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using Jint.Parser.Ast;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Mobile.Core.Interop.IMSI;
using OpenIZ.Core.Model.Entities;
using System.Threading;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents an administrative service
    /// </summary>
    public partial class ApplicationService
    {

        // Is downloading
        private static bool s_isDownloading = false;

        /// <summary>
        /// Force re-queue of all data to server
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "PUT", UriPath = "/data/sync")]
        [Demand(PolicyIdentifiers.AccessClientAdministrativeFunction)]
        public void ForceRequeue()
        {

            // What does this do... oh my ... it is complex
            //
            // 1. We scan the entire database for all Patients that were created in the specified date ranges
            // 2. We scan the entire database for all Acts that were created in the specified date ranges
            // 3. We take all of those and we put them in the outbox in bundles to be shipped to the server at a later time
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            // Hit the act repository
            var patientDataRepository = ApplicationContext.Current.GetService<IRepositoryService<Patient>>() as IPersistableQueryRepositoryService;
            var actDataRepository = ApplicationContext.Current.GetService<IRepositoryService<Act>>() as IPersistableQueryRepositoryService;
            var imsiIntegration = ApplicationContext.Current.GetService<ImsiIntegrationService>();

            // Get all patients matching
            int ofs = 0, tr = 1;
            Guid qid = Guid.NewGuid();
            var filter = QueryExpressionParser.BuildLinqExpression<Patient>(search);
            while (ofs < tr)
            {
                var res = patientDataRepository.Find<Patient>(filter, ofs, 25, out tr, qid);
                ApplicationContext.Current.SetProgress(Strings.locale_preparingPush, (float)ofs / (float)tr * 0.5f);
                ofs += 25;

                var serverSearch = new NameValueCollection();
                serverSearch.Add("id", res.Select(o => o.Key.ToString()).ToList());

                var serverKeys = imsiIntegration.Find<Patient>(serverSearch, 0, 25, new IntegrationQueryOptions()
                {
                    Lean = true,
                    InfrastructureOptions = NameValueCollection.ParseQueryString("_exclude=participation&_exclude=relationship&_exclude=tag&_exclude=identifier&_exclude=address&_exclude=name")
                }).Item.Select(o => o.Key);

                SynchronizationQueue.Outbound.Enqueue(Bundle.CreateBundle(res.Where(o => !serverKeys.Contains(o.Key)), tr, ofs), DataOperationType.Update);
            }

            // Get all acts matching
            qid = Guid.NewGuid();
            var actFilter = QueryExpressionParser.BuildLinqExpression<Act>(search);
            ofs = 0; tr = 1;
            while (ofs < tr)
            {
                var res = actDataRepository.Find<Act>(actFilter, ofs, 25, out tr, qid);
                ApplicationContext.Current.SetProgress(Strings.locale_preparingPush, (float)ofs / (float)tr * 0.5f + 0.5f);
                ofs += 25;

                var serverSearch = new NameValueCollection();
                serverSearch.Add("id", res.Select(o => o.Key.ToString()).ToList());

                var serverKeys = imsiIntegration.Find<Act>(serverSearch, 0, 25, new IntegrationQueryOptions()
                {
                    Lean = true,
                    InfrastructureOptions = NameValueCollection.ParseQueryString("_exclude=participation&_exclude=relationship&_exclude=tag&_exclude=identifier")
                }).Item.Select(o => o.Key);

                SynchronizationQueue.Outbound.Enqueue(Bundle.CreateBundle(res.Where(o => !serverKeys.Contains(o.Key)), tr, ofs), DataOperationType.Update);
            }

        }

        /// <summary>
        /// Delete queue entry
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "POST", UriPath = "/data/restore")]
        [Demand(PolicyIdentifiers.UnrestrictedAdministration)]
        public void Restore()
        {

            // Close all connections
            var conmgr = ApplicationContext.Current.GetService<IDataConnectionManager>();
            var warehouse = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
            if (conmgr == null)
                throw new InvalidOperationException(Strings.err_restoreNotPermitted);

            conmgr.Stop();
            (warehouse as IDaemonService)?.Stop();

            var bksvc = XamarinApplicationContext.Current.GetService<IBackupService>();
            if (bksvc.HasBackup(BackupMedia.Public))
                bksvc.Restore(BackupMedia.Public);
            else if (bksvc.HasBackup(BackupMedia.Private))
                bksvc.Restore(BackupMedia.Private);

            ApplicationContext.Current.SaveConfiguration();
        }

        /// <summary>
        /// Delete queue entry
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "POST", UriPath = "/data/backup")]
        [Demand(PolicyIdentifiers.ExportClinicalData)]
        public void Backup()
        {

            // Close all connections
            var conmgr = ApplicationContext.Current.GetService<IDataConnectionManager>();
            var warehouse = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
            if (conmgr == null)
                throw new InvalidOperationException(Strings.err_restoreNotPermitted);

            conmgr.Stop();
            (warehouse as IDaemonService)?.Stop();

            var bksvc = XamarinApplicationContext.Current.GetService<IBackupService>();
            bksvc.Backup(BackupMedia.Public);

            ApplicationContext.Current.SaveConfiguration();
        }

        /// <summary>
        /// Delete queue entry
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "GET", UriPath = "/data/backup")]
        public bool GetBackup()
        {
            var bksvc = XamarinApplicationContext.Current.GetService<IBackupService>();
            return bksvc.HasBackup(BackupMedia.Public);
        }

        /// <summary>
        /// Instructs the service to compact all databases
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "POST", UriPath = "/data")]
        [Demand(PolicyIdentifiers.Login)]
        public void Compact()
        {

            // Run the specified command vaccuum command on each database
            var conmgr = ApplicationContext.Current.GetService<IDataConnectionManager>();
            if (conmgr == null)
                throw new InvalidOperationException(Strings.err_compactNotPermitted);

            // Iterate compact open connections
            conmgr.Compact();

        }

        /// <summary>
        /// Delete queue entry
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "DELETE", UriPath = "/data")]
        [Demand(PolicyIdentifiers.UnrestrictedAdministration)]
        public void Purge([RestMessage(RestMessageFormat.Json)] JObject parm)
        {
            // Purge the data = Remove the fact that migrations were performed
            ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().MigrationLog.Entry.RemoveAll(o => true);

            // Close all connections
            var conmgr = ApplicationContext.Current.GetService<IDataConnectionManager>();
            var warehouse = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();

            if (conmgr == null)
                throw new InvalidOperationException(Strings.err_purgeNotPermitted);

            conmgr.Stop();
            (warehouse as IDaemonService)?.Stop();

            // Perform a backup if possible
            var bksvc = XamarinApplicationContext.Current.GetService<IBackupService>();
            if (MiniImsServer.CurrentContext.Request.QueryString["backup"] == "true" ||
                    parm?["backup"]?.Value<Boolean>() == true)
                bksvc.Backup(BackupMedia.Public);

            ApplicationContext.Current.SaveConfiguration();
        }

        /// <summary>
        /// Force a re-synchronization
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "POST", UriPath = "/queue")]
        [return: RestMessage(RestMessageFormat.Json)]
        [Demand(PolicyIdentifiers.Login)]
        public void ForceSync()
        {

            var qmService = ApplicationContext.Current.GetService<QueueManagerService>();
            if (qmService.IsBusy || ApplicationContext.Current.GetService<ISynchronizationService>().IsSynchronizing || s_isDownloading)
                throw new InvalidOperationException(Strings.err_already_syncrhonizing);
            else
            {
                ManualResetEvent waitHandle = new ManualResetEvent(false);

                ApplicationContext.Current.SetProgress(Strings.locale_waitForOutbound, 0.1f);

                // Wait for outbound queue to finish
                EventHandler<QueueExhaustedEventArgs> exhaustCallback = (o, e) =>
                {
                    if (e.Queue == "outbound")
                        waitHandle.Set();
                };

                qmService.QueueExhausted += exhaustCallback;
                qmService.ExhaustOutboundQueue();
                qmService.ExhaustAdminQueue();
                waitHandle.WaitOne();
                qmService.QueueExhausted -= exhaustCallback;

                s_isDownloading = true;
                try
                {
                    ApplicationContext.Current.SetProgress(String.Format(Strings.locale_downloading, ""), 0);
                    var targets = ApplicationContext.Current.Configuration.GetSection<SynchronizationConfigurationSection>().SynchronizationResources.Where(o => o.Triggers.HasFlag(SynchronizationPullTriggerType.Always) || o.Triggers.HasFlag(SynchronizationPullTriggerType.OnNetworkChange) || o.Triggers.HasFlag(SynchronizationPullTriggerType.PeriodicPoll)).ToList();
                    for (var i = 0; i < targets.Count(); i++)
                    {
                        var itm = targets[i];
                        ApplicationContext.Current.SetProgress(String.Format(Strings.locale_downloading, itm.ResourceType.Name), (float)i / targets.Count);

                        if (itm.Filters.Count > 0)
                            foreach (var f in itm.Filters)
                                ApplicationContext.Current.GetService<RemoteSynchronizationService>().Pull(itm.ResourceType, NameValueCollection.ParseQueryString(f), itm.Always, itm.Name);
                        else
                            ApplicationContext.Current.GetService<ISynchronizationService>().Pull(itm.ResourceType);
                    }
                }
                finally
                {
                    s_isDownloading = false;
                }
            }

        }

        /// <summary>
        /// Delete queue entry
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "DELETE", UriPath = "/queue")]
        [return: RestMessage(RestMessageFormat.Json)]
        [Demand(PolicyIdentifiers.AccessClientAdministrativeFunction)]
        public void DeleteQueueEntry()
        {
            var id = Int32.Parse(MiniImsServer.CurrentContext.Request.QueryString["_id"]);
            var queue = MiniImsServer.CurrentContext.Request.QueryString["_queue"];

            // Now delete
            switch (queue)
            {
                case "inbound":
                case "inbound_queue":
                    SynchronizationQueue.Inbound.Delete(id);
                    break;
                case "outbound":
                case "outbound_queue":
                    SynchronizationQueue.Outbound.Delete(id);
                    break;
                case "dead":
                case "dead_queue":
                    SynchronizationQueue.DeadLetter.Delete(id);
                    break;
                case "admin":
                case "admin_queue":
                    SynchronizationQueue.Admin.Delete(id);
                    break;
            }
        }

        /// <summary>
        /// Delete queue entry
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "PUT", UriPath = "/queue")]
        [Demand(PolicyIdentifiers.Login)]
        [return: RestMessage(RestMessageFormat.Json)]
        public void ReQueueDead()
        {
            var id = Int32.Parse(MiniImsServer.CurrentContext.Request.QueryString["_id"]);

            // Get > Requeue > Delete
            var queueItem = SynchronizationQueue.DeadLetter.Get(id);
            queueItem.IsRetry = true;

            // HACK: If the queue item is for a bundle and the reason it was rejected was a not null constraint don't re-queue it... 
            // This is caused by older versions of the IMS sending down extensions on our place without an extension type (pre 0.9.11)
            if (Encoding.UTF8.GetString(queueItem.TagData).Contains("entity_extension.extensionType"))
            {
                // Get the bundle object
                var data = ApplicationContext.Current.GetService<IQueueFileProvider>().GetQueueData(queueItem.Data, Type.GetType(queueItem.Type));

                if (data is Place ||
                    (data as Bundle)?.Item.All(o => o is Place) == true &&
                    (data as Bundle)?.Item.Count == 1
                )
                {
                    SynchronizationQueue.DeadLetter.Delete(id);
                    return;
                }
            }

            switch (queueItem.OriginalQueue)
            {
                case "inbound":
                case "inbound_queue":
                    SynchronizationQueue.Inbound.EnqueueRaw(new InboundQueueEntry(queueItem));
                    break;
                case "outbound":
                case "outbound_queue":
                    SynchronizationQueue.Outbound.EnqueueRaw(new OutboundQueueEntry(queueItem));
                    break;
                case "admin":
                case "admin_queue":
                    SynchronizationQueue.Admin.EnqueueRaw(new OutboundAdminQueueEntry(queueItem));
                    break;
                default:
                    throw new KeyNotFoundException(queueItem.OriginalQueue);
            }

            SynchronizationQueue.DeadLetter.Delete(id);
        }

        /// <summary>
        /// Get the specified queue
        /// </summary>
        /// <returns></returns>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "GET", UriPath = "/queue")]
        [Demand(PolicyIdentifiers.Login)]
        [return: RestMessage(RestMessageFormat.Json)]
        public AmiCollection<SynchronizationQueueEntry> GetQueueEntry()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            int offset = Int32.Parse(MiniImsServer.CurrentContext.Request.QueryString["_offset"] ?? "0"),
                count = Int32.Parse(MiniImsServer.CurrentContext.Request.QueryString["_count"] ?? "100"),
                totalResults = 0;

            var explId = MiniImsServer.CurrentContext.Request.QueryString["_id"];
            if (!String.IsNullOrEmpty(explId))
            {
                SynchronizationQueueEntry retVal = null;
                // Get the queue
                switch (MiniImsServer.CurrentContext.Request.QueryString["_queue"])
                {
                    case "inbound":
                        retVal = SynchronizationQueue.Inbound.Get(Int32.Parse(explId));
                        break;
                    case "outbound":
                        retVal = SynchronizationQueue.Outbound.Get(Int32.Parse(explId));
                        break;
                    case "admin":
                        retVal = SynchronizationQueue.Admin.Get(Int32.Parse(explId));
                        break;
                    case "dead":
                        retVal = SynchronizationQueue.DeadLetter.Get(Int32.Parse(explId));
                        break;
                    default:
                        throw new KeyNotFoundException();
                }

                retVal.Data = Convert.ToBase64String(ApplicationContext.Current.GetService<IQueueFileProvider>().GetQueueData(retVal.Data));

                return new AmiCollection<SynchronizationQueueEntry>() { CollectionItem = new List<SynchronizationQueueEntry>() { retVal } };
            }
            else
                // Get the queue
                switch (MiniImsServer.CurrentContext.Request.QueryString["_queue"])
                {
                    case "inbound":
                        {
                            var predicate = QueryExpressionParser.BuildLinqExpression<InboundQueueEntry>(search);
                            return new AmiCollection<SynchronizationQueueEntry>(SynchronizationQueue.Inbound.Query(predicate, offset, count, out totalResults)
                                .Select(o => new InboundQueueEntry()
                                {
                                    Id = o.Id,
                                    CreationTime = o.CreationTime,
                                    Operation = o.Operation,
                                    Type = o.Type
                                })
                                .OfType<SynchronizationQueueEntry>()
                                .ToList())
                            {
                                Size = totalResults,
                                Offset = offset
                            };
                        }
                    case "outbound":
                        {
                            var predicate = QueryExpressionParser.BuildLinqExpression<OutboundQueueEntry>(search);
                            return new AmiCollection<SynchronizationQueueEntry>(SynchronizationQueue.Outbound.Query(predicate, offset, count, out totalResults)
                                .Select(o => new OutboundQueueEntry()
                                {
                                    Id = o.Id,
                                    CreationTime = o.CreationTime,
                                    Operation = o.Operation,
                                    Type = o.Type
                                })
                                .OfType<SynchronizationQueueEntry>().ToList())
                            {
                                Size = totalResults,
                                Offset = offset
                            };
                        }
                    case "admin":
                        {
                            var predicate = QueryExpressionParser.BuildLinqExpression<OutboundAdminQueueEntry>(search);
                            return new AmiCollection<SynchronizationQueueEntry>(SynchronizationQueue.Admin.Query(predicate, offset, count, out totalResults).Select(o => new OutboundAdminQueueEntry()
                            {
                                Id = o.Id,
                                CreationTime = o.CreationTime,
                                Operation = o.Operation,
                                Type = o.Type
                            }).OfType<SynchronizationQueueEntry>().ToList())
                            {
                                Size = totalResults,
                                Offset = offset
                            };
                        }
                    case "dead":
                        {
                            var predicate = QueryExpressionParser.BuildLinqExpression<DeadLetterQueueEntry>(search);
                            return new AmiCollection<SynchronizationQueueEntry>(SynchronizationQueue.DeadLetter.Query(predicate, offset, count, out totalResults).Select(o => new DeadLetterQueueEntry()
                            {
                                Id = o.Id,
                                CreationTime = o.CreationTime,
                                Operation = o.Operation,
                                Type = o.Type,
                                OriginalQueue = o.OriginalQueue,
                                TagData = o.TagData
                            }).OfType<SynchronizationQueueEntry>().ToList())
                            {
                                Size = totalResults,
                                Offset = offset
                            };
                        }
                    default:
                        throw new KeyNotFoundException();
                }

        }

        /// <summary>
        /// Fault provider
        /// </summary>
        public ErrorResult AdminFaultProvider(Exception e)
        {
            return new ErrorResult()
            {
                Error = e is TargetInvocationException ? e.InnerException.Message : e.Message,
                ErrorDescription = e.InnerException?.ToString(),
                ErrorType = e.GetType().Name
            };
        }
    }
}
