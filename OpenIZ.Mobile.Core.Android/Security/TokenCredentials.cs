using System;
using Android.Webkit;
using Java.Interop;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Android.Security;
using System.Security.Permissions;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Android.Http;
using System.Security.Principal;
using OpenIZ.Core.PCL.Http;

namespace OpenIZ.Mobile.Core.Android.Security
{
	/// <summary>
	/// Represents a Credential which is a token credential
	/// </summary>
	public class TokenCredentials : Credentials
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Security.TokenCredentials"/> class.
		/// </summary>
		/// <param name="principal">Principal.</param>
		public TokenCredentials (IPrincipal principal) : base (principal)
		{
			
		}

		#region implemented abstract members of Credentials
		/// <summary>
		/// Get HTTP header
		/// </summary>
		/// <returns>The http headers.</returns>
		public override System.Collections.Generic.Dictionary<string, string> GetHttpHeaders ()
		{
			if (this.Principal is TokenClaimsPrincipal)
				return new System.Collections.Generic.Dictionary<string, string> () {
					{ "Authorization", String.Format ("Bearer {0}", this.Principal.ToString ()) }
				};
			else
				throw new InvalidOperationException ("Cannot create a token credential from non-token principal");
		}
		#endregion
	}


}

