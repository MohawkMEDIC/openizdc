using System;
using System.Security.Principal;
using System.Linq;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Security
{
	/// <summary>
	/// Represents an implementation of a principal which has one or more claims attached to it
	/// </summary>
	public class ClaimsPrincipal : IPrincipal
	{


		// Get the identities
		protected List<ClaimsIdentity> m_identities;

		/// <summary>
		/// Create a new claims principal
		/// </summary>
		public ClaimsPrincipal (ClaimsIdentity identity)
		{
			this.m_identities = new List<ClaimsIdentity> () { identity };
		}

		/// <summary>
		/// Gets the claims in all the identities
		/// </summary>
		/// <value>The claims.</value>
		public IEnumerable<Claim> Claims {
			get {
				return this.m_identities.SelectMany (o => o.Claim);
			}
		}

		/// <summary>
		/// Determines whether this instance has claim the specified predicate.
		/// </summary>
		/// <returns><c>true</c> if this instance has claim the specified predicate; otherwise, <c>false</c>.</returns>
		/// <param name="predicate">Predicate.</param>
		public bool HasClaim(Func<Claim, bool> predicate)
		{
			return this.Claims.Any (predicate);
		}

		#region IPrincipal implementation
		/// <summary>
		/// Determines whether the current principal belongs to the specified role.
		/// </summary>
		/// <returns>true if the current principal is a member of the specified role; otherwise, false.</returns>
		/// <param name="role">The name of the role for which to check membership.</param>
		public bool IsInRole (string role)
		{
			return this.Claims.Any (o => o.Type == ClaimsIdentity.DefaultRoleClaimType && o.Value == role);
		}

		/// <summary>
		/// Gets the primary identity
		/// </summary>
		/// <value>The identity.</value>
		public IIdentity Identity {
			get {
				return this.m_identities.FirstOrDefault ();
			}
		}
		#endregion
	}
}

