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
