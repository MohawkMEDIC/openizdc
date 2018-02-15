using OpenIZ.Core.Model.Acts;
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

            // Re-add substance administrations
            syncSection.SynchronizationResources.Add(new SynchronizationResource()
            {
                Always = false,
                Filters = syncSection.Facilities.SelectMany(o=> new String[] {
                    $"participation[RecordTarget].player.relationship[DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation].target={o}",
                    $"participation[Location|InformationRecipient|EntryLocation].player={o}&participation[RecordTarget].player.relationship[DedicatedServiceDeliveryLocation|IncidentalServiceDeliveryLocation].target=!{o}"
                }).ToList(),
                ResourceType = typeof(SubstanceAdministration),
                Triggers = SynchronizationPullTriggerType.Always
            });
            return true;
        }
    }
}
