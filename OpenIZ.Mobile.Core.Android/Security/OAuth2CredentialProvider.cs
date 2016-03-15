using System;
using OpenIZ.Mobile.Core.Http;

namespace OpenIZ.Mobile.Core.Android.Security
{
	/// <summary>
	/// Credential providerwhich will identify this application
	/// </summary>
	public class OAuth2CredentialProvider : ICredentialProvider
	{
		#region ICredentialProvider implementation
		public OpenIZ.Mobile.Core.Authentication.Credentials GetCredentials (IRestClient context)
		{
			return new OAuthTokenServiceCredentials (ApplicationContext.Current.Principal);
		}
		#endregion
	}
}

