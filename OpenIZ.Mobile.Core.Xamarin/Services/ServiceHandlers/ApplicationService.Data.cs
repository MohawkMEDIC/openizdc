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

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents an administrative service
    /// </summary>
    public partial class ApplicationService
    {

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

            // Perform a backup if possible
            foreach (var itm in ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().ConnectionString)
            {
                var bakFile = Path.ChangeExtension(itm.Value, ".bak");
                if (File.Exists(bakFile))
                {
                    File.Copy(bakFile, itm.Value, true);
                    File.Delete(bakFile);
                }
                else
                    throw new InvalidOperationException(Strings.err_backupNotExist);
            }
            ApplicationContext.Current.SaveConfiguration();
        }


        /// <summary>
        /// Instructs the service to compact all databases
        /// </summary>
        [RestOperation(FaultProvider = nameof(AdminFaultProvider), Method = "POST", UriPath = "/data")]
        [Demand(PolicyIdentifiers.UnrestrictedAdministration)]
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
            foreach (var itm in ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().ConnectionString)
            {
                if (MiniImsServer.CurrentContext.Request.QueryString["backup"] == "true" ||
                    parm?["backup"]?.Value<Boolean>() == true)
                    File.Copy(itm.Value, Path.ChangeExtension(itm.Value, ".bak"), true);
                File.Delete(itm.Value);
            }
            ApplicationContext.Current.SaveConfiguration();
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
                    SynchronizationQueue.Inbound.Delete(id);
                    break;
                case "outbound":
                    SynchronizationQueue.Outbound.Delete(id);
                    break;
                case "dead":
                    SynchronizationQueue.DeadLetter.Delete(id);
                    break;
                case "admin":
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

            switch (queueItem.OriginalQueue)
            {
                case "inbound":
                    SynchronizationQueue.Inbound.EnqueueRaw(new InboundQueueEntry(queueItem));
                    break;
                case "outbound":
                    SynchronizationQueue.Outbound.EnqueueRaw(new OutboundQueueEntry(queueItem));
                    break;
                case "admin":
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
                count = Int32.Parse(MiniImsServer.CurrentContext.Request.QueryString["_count"] ?? "10"),
                totalResults = 0;

            // Get the queue
            switch (MiniImsServer.CurrentContext.Request.QueryString["_queue"])
            {
                case "inbound":
                    {
                        var predicate = QueryExpressionParser.BuildLinqExpression<InboundQueueEntry>(search);
                        return new AmiCollection<SynchronizationQueueEntry>(SynchronizationQueue.Inbound.Query(predicate, offset, count, out totalResults).OfType<SynchronizationQueueEntry>().ToList())
                        {
                            Size = totalResults,
                            Offset = offset
                        };
                    }
                case "outbound":
                    {
                        var predicate = QueryExpressionParser.BuildLinqExpression<OutboundQueueEntry>(search);
                        return new AmiCollection<SynchronizationQueueEntry>(SynchronizationQueue.Outbound.Query(predicate, offset, count, out totalResults).OfType<SynchronizationQueueEntry>().ToList())
                        {
                            Size = totalResults,
                            Offset = offset
                        };
                    }
                case "admin":
                    {
                        var predicate = QueryExpressionParser.BuildLinqExpression<OutboundAdminQueueEntry>(search);
                        return new AmiCollection<SynchronizationQueueEntry>(SynchronizationQueue.Admin.Query(predicate, offset, count, out totalResults).OfType<SynchronizationQueueEntry>().ToList())
                        {
                            Size = totalResults,
                            Offset = offset
                        };
                    }
                case "dead":
                    {
                        var predicate = QueryExpressionParser.BuildLinqExpression<DeadLetterQueueEntry>(search);
                        return new AmiCollection<SynchronizationQueueEntry>(SynchronizationQueue.DeadLetter.Query(predicate, offset, count, out totalResults).OfType<SynchronizationQueueEntry>().ToList())
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
