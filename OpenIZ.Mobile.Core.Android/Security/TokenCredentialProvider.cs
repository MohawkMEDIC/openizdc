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
using OpenIZ.Core.Http;
using OpenIZ.Mobile.Core.Services;
using System.Security;

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
		public Credentials GetCredentials (IRestClient context)
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
		public Credentials Authenticate (IRestClient context)
		{

            // TODO: Determine why we're reauthenticating... if it is an expired token we'll need to get the refresh token
            var tokenCredentials = ApplicationContext.Current.Principal as TokenClaimsPrincipal;
            if (tokenCredentials != null)
            {
                var expiryTime = DateTime.MinValue;
                if (DateTime.TryParse(tokenCredentials.FindClaim(ClaimTypes.Expiration).Value, out expiryTime) &&
                    expiryTime < DateTime.Now)
                {
                    var idp = ApplicationContext.Current.GetService<IIdentityProviderService>();
                    var principal = idp.Authenticate(ApplicationContext.Current.Principal, null);   // Force a re-issue
                    AndroidApplicationContext.Current.SetPrincipal(principal);
                }
                else if (expiryTime > DateTime.Now) // Token is good?
                    return this.GetCredentials(context);
                else // I don't know what happened
                    throw new SecurityException();
            }
            else
                throw new SecurityException();
                return null;
		}
		#endregion
	}

}

