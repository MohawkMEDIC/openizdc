using System;
using System.Net;

namespace OpenIZ.Mobile.Core.Authentication
{
	/// <summary>
	/// Identifies an authentication provider which can be used to authenticate data (and/or modify requests)
	/// </summary>
	public interface IAuthenticationProvider
	{

		/// <summary>
		/// Authenticates the specified application and user against the specified auth provider
		/// </summary>
		Credentials Authenticate(String appId, String appSecret, String userName, String password);

	}
}

