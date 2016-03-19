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
using OpenIZ.Mobile.Core.Http;

namespace OpenIZ.Mobile.Core.Android.Security
{
	/// <summary>
	/// Represents a credential provider which provides a token
	/// </summary>
	public class TokenCredentialProvider : ICredentialProvider
	{
		#region ICredentialProvider implementation
		/// <summary>
		/// Gets or sets the credentials which are used to authenticate
		/// </summary>
		/// <returns>The credentials.</returns>
		/// <param name="context">Context.</param>
		public OpenIZ.Mobile.Core.Authentication.Credentials GetCredentials (IRestClient context)
		{
			if (ApplicationContext.Current.Principal is TokenClaimsPrincipal) {
				return new TokenCredentials (ApplicationContext.Current.Principal);
			} else {
				// We need a token claims principal
				// TODO: Re-authenticate this user against the ACS
				return new TokenCredentials(ApplicationContext.Current.Principal);
			}
		}

		/// <summary>
		/// Authenticate a user - this occurs when reauth is required
		/// </summary>
		/// <param name="context">Context.</param>
		public OpenIZ.Mobile.Core.Authentication.Credentials Authenticate (IRestClient context)
		{

			// TODO: Determine why we're reauthenticating... if it is an expired token we'll need to get the refresh token
			return null;
		}
		#endregion
	}

}

