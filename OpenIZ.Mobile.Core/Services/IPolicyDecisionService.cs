using System;
using OpenIZ.Mobile.Core.Security;
using System.Security.Principal;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Mobile.Core.Services
{
	/// <summary>
	/// Represents a policy decision service
	/// </summary>
	public interface IPolicyDecisionService
	{

		/// <summary>
		/// Get a policy decision outcome (i.e. make a policy decision)
		/// </summary>
		PolicyGrantType GetPolicyOutcome(IPrincipal principal, string policyId);

        /// <summary>
        /// Get a policy decision
        /// </summary>
        PolicyGrantType GetPolicyDecision(IPrincipal principal, Object securable);
	}
}

