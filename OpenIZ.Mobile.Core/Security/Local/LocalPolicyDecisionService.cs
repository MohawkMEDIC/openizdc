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
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Security
{
	/// <summary>
	/// Represents the policy decision service
	/// </summary>
	public class LocalPolicyDecisionService : IPolicyDecisionService
	{

        // Policy cache
        private Dictionary<String, Dictionary<String, PolicyGrantType>> m_policyCache = new Dictionary<string, Dictionary<string, PolicyGrantType>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Security.PolicyDecisionService"/> class.
		/// </summary>
		public LocalPolicyDecisionService()
		{
		}

		/// <summary>
		/// Get a policy decision for a particular securable
		/// </summary>
		public PolicyGrantType GetPolicyDecision(IPrincipal principal, object securable)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));
			else if (securable == null)
				throw new ArgumentNullException(nameof(securable));

			// Get the user object from the principal
			var pip = ApplicationContext.Current.PolicyInformationService;

			// Policies
			var securablePolicies = pip.GetActivePolicies(securable);

			// Most restrictive
			PolicyGrantType decision = PolicyGrantType.Grant;
			foreach (var pol in securablePolicies)
			{
				var securablePdp = this.GetPolicyOutcome(principal, pol.Policy.Oid);

				if (securablePdp < decision)
					decision = securablePdp;
			}

			return decision;
		}

		/// <summary>
		/// Get a policy decision outcome (i.e. make a policy decision)
		/// </summary>
		public PolicyGrantType GetPolicyOutcome(IPrincipal principal, string policyId)
		{
            Dictionary<String, PolicyGrantType> grants = null;
            PolicyGrantType rule;

			if (principal == null)
			{
				throw new ArgumentNullException(nameof(principal));
			}
			else if (String.IsNullOrEmpty(policyId))
			{
				throw new ArgumentNullException(nameof(policyId));
			}
            else if(this.m_policyCache.TryGetValue(principal.Identity.Name, out grants) &&
                grants.TryGetValue(policyId, out rule))
            {
                return rule;
            }

            // Can we make this decision based on the claims?
            if (principal is ClaimsPrincipal && (principal as ClaimsPrincipal).HasClaim(c => c.Type == ClaimTypes.OpenIzGrantedPolicyClaim && c.Value == policyId))
            {
                rule = PolicyGrantType.Grant;
            }
            else
            {
                // Get the user object from the principal
                var pip = ApplicationContext.Current.PolicyInformationService;

                // Policies
                var activePolicies = pip.GetActivePolicies(principal).Where(o => policyId == o.Policy.Oid || policyId.StartsWith(String.Format("{0}.", o.Policy.Oid)));

                // Most restrictive
                IPolicyInstance policyInstance = null;

                foreach (var pol in activePolicies)
                {
                    if (policyInstance == null)
                    {
                        policyInstance = pol;
                    }
                    else if (pol.Rule < policyInstance.Rule)
                    {
                        policyInstance = pol;
                    }
                }

                if (policyInstance == null)
                {
                    // TODO: Configure OptIn or OptOut
                    rule = PolicyGrantType.Deny;
                }
                else if (!policyInstance.Policy.CanOverride && policyInstance.Rule == PolicyGrantType.Elevate)
                {
                    rule = PolicyGrantType.Deny;
                }
                else if (!policyInstance.Policy.IsActive)
                {
                    rule = PolicyGrantType.Grant;
                }
                else
                    rule = policyInstance.Rule;
                
            } // db lookup

            // Add to local policy cache
            lock (this.m_policyCache)
            {

                if (!this.m_policyCache.ContainsKey(principal.Identity.Name))
                {
                    grants = new Dictionary<string, PolicyGrantType>();
                    this.m_policyCache.Add(principal.Identity.Name, grants);
                }
                else if (grants == null)
                    grants = this.m_policyCache[principal.Identity.Name];
                if (!grants.ContainsKey(policyId))
                    grants.Add(policyId, rule);
            }
            return rule;
        }
	}
}