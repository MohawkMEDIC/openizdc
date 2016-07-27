using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Represents a service which can initiate a remote synchronization
    /// </summary>
    public interface IPushNotificationService
    {
        /// <summary>
        /// Push has been received
        /// </summary>
        event EventHandler<PushNotificationEventArgs> MessageReceived;

    }

    /// <summary>
    /// Push notification event args
    /// </summary>
    public class PushNotificationEventArgs : EventArgs
    {
        /// <summary>
        /// The unique identifier of the object which was updated
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// The type of resource the push is about
        /// </summary>
        public String Resource { get; set; }

        /// <summary>
        /// The data related to the push if the notification has it
        /// </summary>
        public IdentifiedData Data { get; set; }

    }
}
