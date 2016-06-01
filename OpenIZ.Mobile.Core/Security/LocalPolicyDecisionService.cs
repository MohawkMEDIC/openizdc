using System;
using System.Linq;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Services;

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
		/// Get a policy decision outcome (i.e. make a policy decision)
		/// </summary>
		public PolicyDecisionOutcomeType GetPolicyOutcome(IPrincipal principal, string policyId)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));
			else if (String.IsNullOrEmpty(policyId))
				throw new ArgumentNullException(nameof(policyId));

			// Can we make this decision based on the claims? 
			if (principal is ClaimsPrincipal && (principal as ClaimsPrincipal).HasClaim(c => c.Type == ClaimTypes.OpenIzGrantedPolicyClaim && c.Value == policyId))
				return PolicyDecisionOutcomeType.Grant;
			
			// Get the user object from the principal
			var pip = ApplicationContext.Current.PolicyInformationService;

			// Policies
			var activePolicies = pip.GetActivePolicies(principal).Where(o => policyId.StartsWith(o.Policy.Oid));
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
				return PolicyDecisionOutcomeType.Deny;
			}
			else if (!policyInstance.Policy.CanOverride && policyInstance.Rule == PolicyDecisionOutcomeType.Elevate)
				return PolicyDecisionOutcomeType.Deny;
			else if (!policyInstance.Policy.IsActive)
				return PolicyDecisionOutcomeType.Grant;

			return policyInstance.Rule;

		}

	}
}

