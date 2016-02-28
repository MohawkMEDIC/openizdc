using System;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Security;

namespace OpenIZ.Mobile.Core.Services
{
	/// <summary>
	/// Represents a contract for a policy information service
	/// </summary>
	public interface IPolicyInformationService
	{
		/// Get active policies for the specified securable type
		/// </summary>
		IEnumerable<IPolicyInstance> GetActivePolicies(object securable);

		/// <summary>
		/// Get all policies on the system
		/// </summary>
		IEnumerable<IPolicy> GetPolicies();

		/// <summary>
		/// Get a specific policy
		/// </summary>
		IPolicy GetPolicy(string policyOid);
	}
}

