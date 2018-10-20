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
            var actTypes = new Type[] { typeof(SubstanceAdministration), typeof(QuantityObservation), typeof(TextObservation), typeof(CodedObservation), typeof(Procedure) };
            syncSection.SynchronizationResources.RemoveAll(o => actTypes.Contains(o.ResourceType));
            syncSection.SynchronizationResources.RemoveAll(o => o.ResourceType == typeof(Patient));

            // Re-add substance administrations
            syncSection.SynchronizationResources.AddRange(actTypes.Select(t=>new SynchronizationResource()
            {
                Always = false,
                Filters = syncSection.Facilities.SelectMany(o => new String[] {
                    $"participation[Location|EntryLocation].player=!{o}&participation[RecordTarget].player.relationship[DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation|ServiceDeliveryLocation].target={o}",
                    $"participation[Location|InformationRecipient|EntryLocation].player={o}"
                }).ToList(),
                ResourceType = t,
                Triggers = SynchronizationPullTriggerType.Always
            }));

            // Add patients that are mine and those that are involved in historical acts that are not mine
            syncSection.SynchronizationResources.Add(new SynchronizationResource()
            {
                Always = false,
                Filters = syncSection.Facilities.SelectMany(o => new String[] {
                    $"relationship[ServiceDeliveryLocation|DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation].target={o}",
                    $"participation[RecordTarget].source.participation[Location].player={o}&relationship[ServiceDeliveryLocation|DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation].target=!{o}"
                }).ToList(),
                ResourceType = typeof(Patient),
                Triggers = SynchronizationPullTriggerType.OnStart | SynchronizationPullTriggerType.OnNetworkChange
            });

            // Persons who are related to my patients
            syncSection.SynchronizationResources.Add(new SynchronizationResource()
            {
                Always = false,
                Filters = syncSection.Facilities.SelectMany(o => new String[] {
                    $"classConcept=9de2a846-ddf2-4ebc-902e-84508c5089ea&relationship.source.classConcept=bacd9c6f-3fa9-481e-9636-37457962804d&relationship.source.relationship[DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation|ServiceDeliveryLocation].target={o}"
                }).ToList(),
                ResourceType = typeof(Person),
                Triggers = SynchronizationPullTriggerType.Always
            });
           
            foreach (var ss in syncSection.SynchronizationResources.Where(o => o.Filters.Any(f => f.Contains("relationship[DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation]"))))
            {
                ss.Filters = ss.Filters.Select(o => o.Replace("relationship[DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation]", "relationship[DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation|ServiceDeliveryLocation]")).ToList();
            }

            return true;
        }
    }
}
