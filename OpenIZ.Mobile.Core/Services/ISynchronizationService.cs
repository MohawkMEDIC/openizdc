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
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Mobile.Core.Synchronization.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{

    public class SynchronizationEventArgs
    {
        /// <summary>
        /// Date of objects from pull
        /// </summary>
        public DateTime FromDate { get; private set; }

        /// <summary>
        /// True if the pull is the initial pull
        /// </summary>
        public bool IsInitial { get; private set; }

        /// <summary>
        /// Gets the type that was pulled
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the filter of the type that was pulled
        /// </summary>
        public NameValueCollection Filter { get; private set; }

        /// <summary>
        /// Count of records imported
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Synchronization type events
        /// </summary>
        public SynchronizationEventArgs(Type type, NameValueCollection filter, DateTime fromDate, int totalResults) : this(totalResults)  
        {
            this.Type = type;
            this.Filter = filter;
            this.FromDate = fromDate;
            this.IsInitial = fromDate == default(DateTime);
        }

        /// <summary>
        /// Create an empty pull event arg
        /// </summary>
        public SynchronizationEventArgs(int totalResults)
        {
            this.Count = totalResults;
        }

        /// <summary>
        /// Creates a new initial pull event arg
        /// </summary>
        public SynchronizationEventArgs(bool isInitial, int totalResults) : this(totalResults) 
        {
            this.IsInitial = isInitial;

        }
    }

    /// <summary>
    /// Represents a synchronization service 
    /// </summary>
    public interface ISynchronizationService
    {

        /// <summary>
        /// Fired when a pull has completed and imported data
        /// </summary>
        event EventHandler<SynchronizationEventArgs> PullCompleted;

        /// <summary>
        /// Fetch to see if there are any particular changes on the specified model type
        /// </summary>
        bool Fetch(Type modelType);

        /// <summary>
        /// Pull data from the remove server and place it on the inbound queue
        /// </summary>
        int Pull(Type modelType);

        /// <summary>
        /// Pull data from the remove server and place it on the inbound queue
        /// </summary>
        int Pull(Type modelType, NameValueCollection filter);


    }
}
