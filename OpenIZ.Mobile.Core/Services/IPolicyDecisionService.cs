using System;
using OpenIZ.Mobile.Core.Security;
using System.Security.Principal;

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
		PolicyDecisionOutcomeType GetPolicyOutcome(IPrincipal principal, string policyId);

	}
}

