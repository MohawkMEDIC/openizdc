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

namespace OpenIZ.Mobile.Core.Security.Audit
{
    /// <summary>
    /// Local auditing service
    /// </summary>
    public class LocalAuditService : IAuditorService
    {

        // Tracer class
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalAuditService));

        /// <summary>
        /// Send an audit (which stores the audit locally in the audit file and then queues it for sending)
        /// </summary>
        public bool SendAudit(AuditData ad)
        {
            try
            {
                // First, save the audit locally
                var ar = ApplicationContext.Current.GetService<IAuditRepositoryService>();
                if (ar == null)
                    throw new InvalidOperationException("!!SECURITY ALERT!! >> Cannot find audit repository");
                ad = ar.Insert(ad);

                // Queue for send
                SynchronizationQueue.Admin.Enqueue(new AuditInfo(ad), Synchronization.Model.DataOperationType.Insert);

                return true;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("!!SECURITY ALERT!! >> Error sending audit {0}: {1}", ad, e);
                throw;
            }
        }
    }
}
