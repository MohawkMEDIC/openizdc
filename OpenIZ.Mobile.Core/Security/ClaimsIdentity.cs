using System;
using System.Security.Principal;
using System.Linq;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Security
{

	/// <summary>
	/// Represents an identity with one or more claims
	/// </summary>
	public class ClaimsIdentity : IIdentity
	{

		public const string DefaultNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
		public const string DefaultRoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

		// Claims made about the user
		private List<Claim> m_claims;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Security.ClaimsIdentity"/> class.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <param name="isAuthenticated">If set to <c>true</c> is authenticated.</param>
		public ClaimsIdentity (String userName, bool isAuthenticated, IEnumerable<Claim> claims)
		{
			this.m_claims = new List<Claim> (claims);
			if(!this.m_claims.Exists(o=>o.Type == DefaultNameClaimType))
				this.m_claims.Add(new Claim(DefaultNameClaimType, userName));
			this.IsAuthenticated = isAuthenticated;
		}

		/// <summary>
		/// Gets the list of claims made about the identity
		/// </summary>
		/// <value>The claim.</value>
		public IEnumerable<Claim> Claim {
			get {
				return this.m_claims;
			}
		}

		#region IIdentity implementation

		/// <summary>
		/// Gets the type of authentication used.
		/// </summary>
		/// <returns>The type of authentication used to identify the user.</returns>
		/// <value>The type of the authentication.</value>
		public string AuthenticationType {
			get {
				return String.Empty;
			}
		}

		/// <summary>
		/// Gets a value that indicates whether the user has been authenticated.
		/// </summary>
		/// <returns>true if the user was authenticated; otherwise, false.</returns>
		/// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
		public bool IsAuthenticated {
			get;
			private set;
		}

		/// <summary>
		/// Gets the name of the current user.
		/// </summary>
		/// <returns>The name of the user on whose behalf the code is running.</returns>
		/// <value>The name.</value>
		public string Name {
			get {
				return this.m_claims.Find (o => o.Type == DefaultNameClaimType).Value;
			}
		}

		#endregion
	}

}
