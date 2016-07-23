using System;
using System.Linq;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Mobile.Core.Security
{
	/// <summary>
	/// Represents the policy decision service
	/// </summary>
	public class LocalPolicyDecisionService : IPolicyDecisionService
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Security.PolicyDecisionService"/> class.
		/// </summary>
		public LocalPolicyDecisionService ()
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
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));
			else if (String.IsNullOrEmpty(policyId))
				throw new ArgumentNullException(nameof(policyId));

			// Can we make this decision based on the claims? 
			if (principal is ClaimsPrincipal && (principal as ClaimsPrincipal).HasClaim(c => c.Type == ClaimTypes.OpenIzGrantedPolicyClaim && c.Value == policyId))
				return PolicyGrantType.Grant;
			
			// Get the user object from the principal
			var pip = ApplicationContext.Current.PolicyInformationService;

			// Policies
			var activePolicies = pip.GetActivePolicies(principal).Where(o => policyId == o.Policy.Oid || policyId.StartsWith(String.Format("{0}.",o.Policy.Oid)));
			// Most restrictive
			IPolicyInstance policyInstance = null;
			foreach (var pol in activePolicies)
				if (policyInstance == null)
					policyInstance = pol;
				else if (pol.Rule < policyInstance.Rule)
					policyInstance = pol;

			if(policyInstance == null)
			{
				// TODO: Configure OptIn or OptOut
				return PolicyGrantType.Deny;
			}
			else if (!policyInstance.Policy.CanOverride && policyInstance.Rule == PolicyGrantType.Elevate)
				return PolicyGrantType.Deny;
			else if (!policyInstance.Policy.IsActive)
				return PolicyGrantType.Grant;

			return policyInstance.Rule;

		}

	}
}

