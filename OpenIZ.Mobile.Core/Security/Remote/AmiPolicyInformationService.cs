/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-23
 */
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
        /// Remote policies only
        /// </summary>
        public AmiPolicyInformationService()
        {

        }

        /// <summary>
        /// Policy as a specific principal
        /// </summary>
        public AmiPolicyInformationService(ClaimsPrincipal cprincipal)
        {
            this.m_client.Client.Credentials = this.m_client.Client?.Description?.Binding?.Security?.CredentialProvider?.GetCredentials(cprincipal);
        }

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
