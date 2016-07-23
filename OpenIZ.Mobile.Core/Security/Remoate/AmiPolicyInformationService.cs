using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Mobile.Core.Interop;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Policy information service which feeds from AMI
    /// </summary>
    public class AmiPolicyInformationService : IPolicyInformationService
    {
        // Service client
        private AmiServiceClient m_client = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));

        /// <summary>
        /// Get active policies for the specified securable
        /// </summary>
        public IEnumerable<IPolicyInstance> GetActivePolicies(object securable)
        {
            // Security device
            if (securable is SecurityDevice)
                throw new NotImplementedException();
            else if (securable is SecurityRole)
            {
                string name = (securable as SecurityRole).Name;
                return this.m_client.FindRole(o => o.Name == name).CollectionItem.First().Policies.Select(o => new GenericPolicyInstance(new GenericPolicy(o.Oid, o.Name, o.CanOverride), o.Grant)).ToList();
            }
            else if (securable is SecurityApplication)
                throw new NotImplementedException();
            else if (securable is IPrincipal || securable is IIdentity)
                throw new NotImplementedException();
            else if (securable is Act)
                throw new NotImplementedException();
            else if (securable is Entity)
                throw new NotImplementedException();
            else
                return new List<IPolicyInstance>();
        }

        public IEnumerable<IPolicy> GetPolicies()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the specified policy from the AMI
        /// </summary>
        public IPolicy GetPolicy(string policyOid)
        {
            return this.m_client.FindPolicy(p=>p.Oid == policyOid).CollectionItem.Select(o => new GenericPolicy(o.Oid, o.Name, o.CanOverride)).FirstOrDefault();
        }
    }
}
