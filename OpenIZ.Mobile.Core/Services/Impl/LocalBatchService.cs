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
 * Date: 2017-3-31
 */
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Interfaces;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Synchronization;
using System;
using System.Linq;

namespace OpenIZ.Mobile.Core.Services.Impl
{
	/// <summary>
	/// Local batch service
	/// </summary>
	public class LocalBatchService : IBatchRepositoryService, IAuditEventSource
	{
        public event EventHandler<AuditDataEventArgs> DataCreated;
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        public event EventHandler<AuditDataEventArgs> DataUpdated;

        /// <summary>
        /// Insert the bundle
        /// </summary>
        public Bundle Insert(Bundle data)
		{
			data = this.Validate(data);
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
			if (persistence == null)
				throw new InvalidOperationException("Missing persistence service");
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();

            data = breService?.BeforeInsert(data) ?? data;

			data = persistence.Insert(data);
            breService?.AfterInsert(data) ;

            // Insert bundle to the master queue
            ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(o=> SynchronizationQueue.Outbound.Enqueue(Bundle.CreateBundle(data.Item, data.TotalResults, data.Offset), Synchronization.Model.DataOperationType.Insert));

            this.DataCreated?.Invoke(this, new AuditDataEventArgs(data.Item));

            return data;
		}

		/// <summary>
		/// Obsolete all the contents in the bundle
		/// </summary>
		public Bundle Obsolete(Bundle obsolete)
		{
			obsolete = this.Validate(obsolete);
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
			if (persistence == null)
				throw new InvalidOperationException("Missing persistence service");
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();

            obsolete = breService?.BeforeObsolete(obsolete) ?? obsolete;
			obsolete = persistence.Obsolete(obsolete);
            obsolete = breService?.AfterObsolete(obsolete) ?? obsolete;

            SynchronizationQueue.Outbound.Enqueue(obsolete, Synchronization.Model.DataOperationType.Obsolete);

            this.DataObsoleted?.Invoke(this, new AuditDataEventArgs(obsolete.Item));

            return obsolete;
		}

		/// <summary>
		/// Update the specified data in the bundle
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public Bundle Update(Bundle data)
		{
			data = this.Validate(data);
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
			if (persistence == null)
				throw new InvalidOperationException("Missing persistence service");
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();

            // Entry point
            IdentifiedData old = null;
            if(data.EntryKey != null)
            {
                var type = data.Entry.GetType();
                var idps = typeof(IDataPersistenceService<>).MakeGenericType(type);
                var dataService = ApplicationContext.Current.GetService(idps) as IDataPersistenceService;
                old = (dataService.Get(data.EntryKey.Value) as IdentifiedData).Clone();
            }

            data = breService?.BeforeUpdate(data) ?? data;
            data = persistence.Insert(data);
            breService?.AfterUpdate(data);

            this.DataUpdated?.Invoke(this, new AuditDataEventArgs(data.Item));

            // Patch
            if (old != null)
            {
                var diff = ApplicationContext.Current.GetService<IPatchService>()?.Diff(old, data.Entry);
                if (diff != null)
                    SynchronizationQueue.Outbound.Enqueue(diff, Synchronization.Model.DataOperationType.Update);
                else
                    SynchronizationQueue.Outbound.Enqueue(data, Synchronization.Model.DataOperationType.Update);
            }
            else
                SynchronizationQueue.Outbound.Enqueue(data, Synchronization.Model.DataOperationType.Update);
            return data;
        }

        /// <summary>
        /// Validate the bundle and its contents
        /// </summary>
        public Bundle Validate(Bundle bundle)
		{
			bundle = bundle.Clean() as Bundle;

            // BRE validation
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();
            var issues = breService?.Validate(bundle);
            if (issues?.Any(i => i.Priority == DetectedIssuePriorityType.Error) == true)
                throw new DetectedIssueException(issues);

            // Bundle items
            for (int i = 0; i < bundle.Item.Count; i++)
			{
				var itm = bundle.Item[i];
				if (itm is Patient)
					bundle.Item[i] = ApplicationContext.Current.GetService<IPatientRepositoryService>().Validate(itm as Patient);
				else if (itm is Act)
					bundle.Item[i] = ApplicationContext.Current.GetService<IActRepositoryService>().Validate(itm as Act);
			}
			return bundle;
		}
	}
}