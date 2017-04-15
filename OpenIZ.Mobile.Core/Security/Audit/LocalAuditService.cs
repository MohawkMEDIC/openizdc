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

        /// <summary>
        /// Send an audit (which stores the audit locally in the audit file and then queues it for sending)
        /// </summary>
        public bool SendAudit(AuditData audit)
        {
            ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(o =>
            {
                var ad = o as AuditData;
                try
                {
                    // First, save the audit locally
                    var ar = ApplicationContext.Current.GetService<IAuditRepositoryService>();
                    if (ar == null)
                        throw new InvalidOperationException("!!SECURITY ALERT!! >> Cannot find audit repository");
                    ad = ar.Insert(ad);

                    // Queue for send
                    SynchronizationQueue.Admin.Enqueue(new AuditInfo(ad), Synchronization.Model.DataOperationType.Insert);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("!!SECURITY ALERT!! >> Error sending audit {0}: {1}", ad, e);
                    throw;
                }
            }, audit);
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

                    ApplicationContext.Current.GetService<IIdentityProviderService>().Authenticated += (so, se) => AuditUtil.AuditLogin(se.Principal, se.UserName, so as IIdentityProviderService, se.Success);
                    ApplicationContext.Current.GetService<QueueManagerService>().QueueExhausted += (so, se) =>
                    {
                        if (se.ObjectKeys.Count() > 0)
                            switch (se.Queue)
                            {
                                case "inbound":
                                    AuditUtil.AuditDataAction<IdentifiedData>(EventTypeCodes.Import, ActionType.Create, AuditableObjectLifecycle.Import, EventIdentifierType.Import, OutcomeIndicator.Success, null);
                                    break;
                                case "outbound":
                                    AuditUtil.AuditDataAction<IdentifiedData>(EventTypeCodes.Export, ActionType.Execute, AuditableObjectLifecycle.Export, EventIdentifierType.Export, OutcomeIndicator.Success, null);
                                    break;
                            }
                    };

                    // Scan for IRepositoryServices and bind to their events as well
                    foreach (var svc in ApplicationContext.Current.GetServices().OfType<IAuditEventSource>())
                    {
                        svc.DataCreated += (so, se) => AuditUtil.AuditDataAction(EventTypeCodes.PatientRecord, ActionType.Create, AuditableObjectLifecycle.Creation, EventIdentifierType.PatientRecord, se.Success ? OutcomeIndicator.Success : OutcomeIndicator.SeriousFail, null, se.Objects.OfType<IdentifiedData>().ToArray());
                        svc.DataUpdated += (so, se) => AuditUtil.AuditDataAction(EventTypeCodes.PatientRecord, ActionType.Update, AuditableObjectLifecycle.Amendment, EventIdentifierType.PatientRecord, se.Success ? OutcomeIndicator.Success : OutcomeIndicator.SeriousFail, null, se.Objects.OfType<IdentifiedData>().ToArray());
                        svc.DataObsoleted += (so, se) => AuditUtil.AuditDataAction(EventTypeCodes.PatientRecord, ActionType.Delete, AuditableObjectLifecycle.LogicalDeletion, EventIdentifierType.PatientRecord, se.Success ? OutcomeIndicator.Success : OutcomeIndicator.SeriousFail, null, se.Objects.OfType<IdentifiedData>().ToArray());
                        svc.DataDisclosed += (so, se) => AuditUtil.AuditDataAction(EventTypeCodes.Query, ActionType.Read, AuditableObjectLifecycle.Disclosure, EventIdentifierType.Query, se.Success ? OutcomeIndicator.Success : OutcomeIndicator.SeriousFail, null, se.Objects.OfType<IdentifiedData>().ToArray());

                        if (svc is ISecurityAuditEventSource)
                            (svc as ISecurityAuditEventSource).SecurityAttributesChanged += (so, se) => AuditUtil.AuditSecurityAttributeAction(se.Objects, se.Success, se.ChangedProperties);
                    }

                    AuditUtil.AuditApplicationStartStop(EventTypeCodes.ApplicationStart);
                }
                catch(Exception ex)
                {
                    this.m_tracer.TraceError("Error starting up audit repository service: {0}", ex);
                }
            };
            ApplicationContext.Current.Stopped += (o, e) => AuditUtil.AuditApplicationStartStop(EventTypeCodes.ApplicationStop);
            ApplicationContext.Current.Stopping += (o, e) => this.m_safeToStop = true;

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
            if (!this.m_safeToStop) {
                AuditData securityAlertData = new AuditData(DateTime.Now, ActionType.Execute, OutcomeIndicator.EpicFail, EventIdentifierType.SecurityAlert, AuditUtil.CreateAuditActionCode(EventTypeCodes.UseOfARestrictedFunction));
                AuditUtil.AddDeviceActor(securityAlertData);
                AuditUtil.SendAudit(securityAlertData);
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
