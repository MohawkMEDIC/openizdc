﻿using System;
using OpenIZ.Mobile.Core.Http;

namespace OpenIZ.Mobile.Core.Android.Security
{
	/// <summary>
	/// Credential providerwhich will identify this application
	/// </summary>
	public class OAuth2CredentialProvider : ICredentialProvider
	{
		#region ICredentialProvider implementation
		/// <summary>
		/// Gets or sets the credentials which are used to authenticate
		/// </summary>
		/// <returns>The credentials.</returns>
		/// <param name="context">Context.</param>
		public OpenIZ.Mobile.Core.Authentication.Credentials GetCredentials (IRestClient context)
		{
			// return this application's credentials
			return new OAuthTokenServiceCredentials (ApplicationContext.Current.Principal);
		}

		/// <summary>
		/// Authentication request is required
		/// </summary>
		/// <param name="context">Context.</param>
		public OpenIZ.Mobile.Core.Authentication.Credentials Authenticate (IRestClient context)
		{
			// return this application's credentials
			return new OAuthTokenServiceCredentials (ApplicationContext.Current.Principal);
		}
		#endregion
	}
}
