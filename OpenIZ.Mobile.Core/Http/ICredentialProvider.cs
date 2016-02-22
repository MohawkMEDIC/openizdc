using System;
using OpenIZ.Mobile.Core.Authentication;

namespace OpenIZ.Mobile.Core.Http
{
	/// <summary>
	/// Authorization event args
	/// </summary>
	public interface ICredentialProvider 
	{

		/// <summary>
		/// Gets or sets the credentials which are used to authenticate
		/// </summary>
		Credentials GetCredentials(IRestClient context);

	}

}

