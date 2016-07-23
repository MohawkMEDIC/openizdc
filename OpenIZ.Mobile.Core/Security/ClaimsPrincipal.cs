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
 * Date: 2016-6-14
 */
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

		/// <summary>
		/// Finds the specified claim.
		/// </summary>
		/// <returns>The claim.</returns>
		/// <param name="claimType">Claim type.</param>
		public Claim FindClaim(String claimType)
		{
			return this.Claims.FirstOrDefault (o => o.Type == claimType);
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

