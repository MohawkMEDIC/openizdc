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
 * Date: 2017-6-28
 */
using MARC.HI.EHRS.SVC.Auditing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Core.Model;
using System.Xml.Serialization;
using OpenIZ.Mobile.Core.Synchronization.Model;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Interfaces;
using OpenIZ.Mobile.Core.Configuration;
using System.Threading;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Mobile.Core.Security.Audit
{
    /// <summary>
    /// Local auditing service
    /// </summary>
    public class LocalAuditService : IAuditorService, IDaemonService
    {

        /// <summary>
        /// Dummy identiifed wrapper
        /// </summary>
        private class DummyIdentifiedWrapper : IdentifiedData
        {
            /// <summary>
            /// Modified on
            /// </summary>
            public override DateTimeOffset ModifiedOn
            {
                get
                {
                    return DateTimeOffset.Now;
                }
            }
        }

        private bool m_safeToStop = false;

        // Tracer class
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalAuditService));

        // Audit queue
        private Queue<AuditData> m_auditQueue = new Queue<AuditData>();

        // Reset event
        private AutoResetEvent m_resetEvent = new AutoResetEvent(false);

        /// <summary>
        ///  True if the service is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return true;
            }
        }

        public event EventHandler Started;
        public event EventHandler Starting;
        public event EventHandler Stopped;
        public event EventHandler Stopping;

        // Duplicate guard
        private Dictionary<Guid, DateTime> m_duplicateGuard = new Dictionary<Guid, DateTime>();

        /// <summary>
        /// Send an audit (which stores the audit locally in the audit file and then queues it for sending)
        /// </summary>
        public bool SendAudit(AuditData audit)
        {
            // Check duplicate guard
            Guid objId = Guid.Empty;
            var queryObj = audit.AuditableObjects.FirstOrDefault(o => o.Role == AuditableObjectRole.Query);
            if (queryObj != null && Guid.TryParse(queryObj.QueryData, out objId))
            {
                // prevent duplicate sending
                DateTime lastAuditObj = default(DateTime);
                if (this.m_duplicateGuard.TryGetValue(objId, out lastAuditObj) && DateTime.Now.Subtract(lastAuditObj).TotalSeconds < 2)
                    return true; // duplicate
                else
                    lock (this.m_duplicateGuard)
                    {
                        if (this.m_duplicateGuard.ContainsKey(objId))
                            this.m_duplicateGuard[objId] = DateTime.Now;
                        else
                            this.m_duplicateGuard.Add(objId, DateTime.Now);
                    }
            }

            lock (this.m_auditQueue)
                this.m_auditQueue.Enqueue(audit);
            this.m_resetEvent.Set();
            return true;
        }

        /// <summary>
        /// Start auditor service
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            this.m_safeToStop = false;
            ApplicationContext.Current.Started += (o, e) =>
            {
                try
                {
                    this.m_tracer.TraceInfo("Binding to service events...");

                    ApplicationContext.Current.GetService<IIdentityProviderService>().Authenticated += (so, se) =>
                    {
                        if ((se.Principal?.Identity.Name ?? se.UserName).ToLower() != ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().DeviceName.ToLower())
                            AuditUtil.AuditLogin(se.Principal, se.UserName, so as IIdentityProviderService, se.Success);
                    };
                    ApplicationContext.Current.GetService<QueueManagerService>().QueueExhausted += (so, se) =>
                    {
                        if (se.ObjectKeys.Count() > 0)
                            switch (se.Queue)
                            {
                                case "inbound":
                                    if(SynchronizationQueue.Inbound.Count() == 0)
                                        AuditUtil.AuditDataAction<IdentifiedData>(EventTypeCodes.Import, ActionType.Create, AuditableObjectLifecycle.Import, EventIdentifierType.Import, OutcomeIndicator.Success, null);
                                    break;
                                case "outbound":
                                    if (SynchronizationQueue.Outbound.Count() == 0)
                                        AuditUtil.AuditDataAction<IdentifiedData>(EventTypeCodes.Export, ActionType.Execute, AuditableObjectLifecycle.Export, EventIdentifierType.Export, OutcomeIndicator.Success, null);
                                    break;
                            }
                    };

                    // Scan for IRepositoryServices and bind to their events as well
                    foreach (var svc in ApplicationContext.Current.GetServices().OfType<IAuditEventSource>())
                    {
                        svc.DataCreated += (so, se) =>
                        {
                            if (se.Objects.Any(x => x is Entity || x is Act))
                                AuditUtil.AuditDataAction(EventTypeCodes.PatientRecord, ActionType.Create, AuditableObjectLifecycle.Creation, EventIdentifierType.PatientRecord, se.Success ? OutcomeIndicator.Success : OutcomeIndicator.SeriousFail, null, se.Objects.OfType<IdentifiedData>().ToArray());
                        };
                        svc.DataUpdated += (so, se) =>
                        {
                            if (se.Objects.Any(x => x is Entity || x is Act))
                                AuditUtil.AuditDataAction(EventTypeCodes.PatientRecord, ActionType.Update, AuditableObjectLifecycle.Amendment, EventIdentifierType.PatientRecord, se.Success ? OutcomeIndicator.Success : OutcomeIndicator.SeriousFail, null, se.Objects.OfType<IdentifiedData>().ToArray());
                        };
                        svc.DataObsoleted += (so, se) =>
                        {
                            if (se.Objects.Any(x => x is Entity || x is Act))
                                AuditUtil.AuditDataAction(EventTypeCodes.PatientRecord, ActionType.Delete, AuditableObjectLifecycle.LogicalDeletion, EventIdentifierType.PatientRecord, se.Success ? OutcomeIndicator.Success : OutcomeIndicator.SeriousFail, null, se.Objects.OfType<IdentifiedData>().ToArray());
                        };
                        svc.DataDisclosed += (so, se) =>
                        {
                            if (se.Objects.Count() > 0 && se.Objects.Any(i=>i is Patient || i is Act))
                                AuditUtil.AuditDataAction(EventTypeCodes.Query, ActionType.Read, AuditableObjectLifecycle.Disclosure, EventIdentifierType.Query, se.Success ? OutcomeIndicator.Success : OutcomeIndicator.SeriousFail, se.Query, se.Objects.OfType<IdentifiedData>().ToArray());
                        };

                        if (svc is ISecurityAuditEventSource)
                            (svc as ISecurityAuditEventSource).SecurityAttributesChanged += (so, se) => AuditUtil.AuditSecurityAttributeAction(se.Objects, se.Success, se.ChangedProperties);
                    }

                    AuditUtil.AuditApplicationStartStop(EventTypeCodes.ApplicationStart);
                }
                catch (Exception ex)
                {
                    this.m_tracer.TraceError("Error starting up audit repository service: {0}", ex);
                }
            };
            ApplicationContext.Current.Stopped += (o, e) => AuditUtil.AuditApplicationStartStop(EventTypeCodes.ApplicationStop);
            ApplicationContext.Current.Stopping += (o, e) => this.m_safeToStop = true;

            AuditInfo sendAudit = new AuditInfo();

            // Send audit
            Action<Object> timerQueue = null;
            timerQueue = o =>
            {
                lock(sendAudit)
                    if (sendAudit.Audit.Count > 0)
                        SynchronizationQueue.Admin.Enqueue(sendAudit, Synchronization.Model.DataOperationType.Insert);
                ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(new TimeSpan(0, 0, 30), timerQueue, null);
            };

            // Queue user work item for sending
            ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(new TimeSpan(0, 0, 30), timerQueue, null);

            // Queue pooled item
            ApplicationContext.Current.GetService<IThreadPoolService>().QueueNonPooledWorkItem(o =>
            {
                while (!this.m_safeToStop)
                {
                    try
                    {
                        this.m_resetEvent.WaitOne();
                        while (this.m_auditQueue.Count > 0)
                        {
                            AuditData ad = null;

                            lock(this.m_auditQueue)
                                ad = this.m_auditQueue.Dequeue();

                            try
                            {
                                // First, save the audit locally
                                var ar = ApplicationContext.Current.GetService<IAuditRepositoryService>();
                                if (ar == null)
                                    throw new InvalidOperationException("!!SECURITY ALERT!! >> Cannot find audit repository");
                                ad = ar.Insert(ad);

                                lock(sendAudit)
                                    sendAudit.Audit.Add(ad);
                            }
                            catch (Exception e)
                            {
                                this.m_tracer.TraceError("!!SECURITY ALERT!! >> Error sending audit {0}: {1}", ad, e);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("!!SECURITY ALERT!! >> Error polling audit task list {0}", e);
                    }
                }
            }, null);
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stopped 
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            // Audit tool should never stop!!!!!
            if (!this.m_safeToStop)
            {
                AuditData securityAlertData = new AuditData(DateTime.Now, ActionType.Execute, OutcomeIndicator.EpicFail, EventIdentifierType.SecurityAlert, AuditUtil.CreateAuditActionCode(EventTypeCodes.UseOfARestrictedFunction));
                AuditUtil.AddDeviceActor(securityAlertData);
                AuditUtil.SendAudit(securityAlertData);
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
