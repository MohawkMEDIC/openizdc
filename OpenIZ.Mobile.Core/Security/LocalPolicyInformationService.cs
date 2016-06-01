using System;
using OpenIZ.Mobile.Core.Configuration;
using System.Collections.Generic;
using OpenIZ.Core.Model.Security;
using System.Security.Principal;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Services;

namespace OpenIZ.Mobile.Core.Security
{
	/// <summary>
	/// Represents a PIP which uses the local data store
	/// </summary>
	public class LocalPolicyInformationService : IPolicyInformationService
	{

		/// <summary>
		/// Get active policies for the specified securable type
		/// </summary>
		public IEnumerable<IPolicyInstance> GetActivePolicies(object securable)
		{
			// Security device
			if (securable is SecurityDevice)
				throw new NotImplementedException ();
			else if (securable is SecurityRole)
				throw new NotImplementedException ();
			else if (securable is SecurityApplication)
				throw new NotImplementedException ();
			else if (securable is IPrincipal || securable is IIdentity) {
				var identity = (securable as IPrincipal)?.Identity ?? securable as IIdentity;
				throw new NotImplementedException ();

			} else if (securable is Act) {
				var pAct = securable as Act;
				throw new NotImplementedException ();

			} else if (securable is Entity) {
				throw new NotImplementedException ();
			} else
				return new List<IPolicyInstance> ();
		}

		/// <summary>
		/// Get all policies on the system
		/// </summary>
		public IEnumerable<IPolicy> GetPolicies()
		{
			return null;
		}

		/// <summary>
		/// Get a specific policy
		/// </summary>
		public IPolicy GetPolicy(string policyOid)
		{
			return null;
		}

	}
}

