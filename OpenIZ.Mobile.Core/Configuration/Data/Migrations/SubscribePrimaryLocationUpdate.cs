using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Configuration.Data.Migrations
{
    /// <summary>
    /// Subscribe to primary location update
    /// </summary>
    class SubscribePrimaryLocationUpdate : IDbMigration
    {
        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get
            {
                return "Updates subscriptions to include all data the location authored";
            }
        }

        /// <summary>
        /// Gets id of the update
        /// </summary>
        public string Id
        {
            get
            {
                return "update-subscription-primary-location";
            }
        }

        /// <summary>
        /// Install the migration
        /// </summary>
        public bool Install()
        {

            var syncSection = ApplicationContext.Current.Configuration.GetSection<SynchronizationConfigurationSection>();

            // Un-subscribe to SubstanceAdministration
            syncSection.SynchronizationResources.RemoveAll(o => o.ResourceType == typeof(SubstanceAdministration));
            syncSection.SynchronizationResources.RemoveAll(o => o.ResourceType == typeof(Patient));
            syncSection.SynchronizationResources.RemoveAll(o => o.ResourceType == typeof(Person));
            syncSection.SynchronizationResources.RemoveAll(o => o.Name == "locale.sync.resource.PatientEncounter.my");

            // Re-add substance administrations
            syncSection.SynchronizationResources.Add(new SynchronizationResource()
            {
                Always = false,
                Filters = syncSection.Facilities.SelectMany(o=> new String[] {
                    $"participation[Location].player=!{o}&participation[RecordTarget].player.relationship[DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation|ServiceDeliveryLocation].target={o}",
                    $"participation[Location|InformationRecipient|EntryLocation].player={o}"
                }).ToList(),
                ResourceType = typeof(SubstanceAdministration),
                Triggers = SynchronizationPullTriggerType.Always
            });
            syncSection.SynchronizationResources.Add(new SynchronizationResource()
            {
                Always = false,
                Filters = syncSection.Facilities.SelectMany(o => new String[] {
                    $"relationship[ServiceDeliveryLocation|DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation].target={o}"
                }).ToList(),
                ResourceType = typeof(Patient),
                Triggers = SynchronizationPullTriggerType.OnStart | SynchronizationPullTriggerType.OnNetworkChange
            });
            syncSection.SynchronizationResources.Add(new SynchronizationResource()
            {
                Always = false,
                Filters = syncSection.Facilities.SelectMany(o => new String[] {
                    $"classConcept=9de2a846-ddf2-4ebc-902e-84508c5089ea&relationship.source.classConcept=bacd9c6f-3fa9-481e-9636-37457962804d&relationship.source.relationship[DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation|ServiceDeliveryLocation].target={o}"
                }).ToList(),
                ResourceType = typeof(Person),
                Triggers = SynchronizationPullTriggerType.Always
            });
            syncSection.SynchronizationResources.Add(new SynchronizationResource()
            {
                Name = "locale.sync.resource.PatientEncounter.my",
                Always = false,
                Filters = syncSection.Facilities.SelectMany(o => new String[] {
                    $"participation[RecordTarget].source.participation[Location].player={o}&participation[RecordTarget].source.statusConcept=c8064cbd-fa06-4530-b430-1a52f1530c27&participation[RecordTarget].source.classConcept=54b52119-1709-4098-8911-5df6d6c84140"
                }).ToList(),
                ResourceType = typeof(Person),
                Triggers = SynchronizationPullTriggerType.PeriodicPoll
            });
            return true;
        }
    }
}
