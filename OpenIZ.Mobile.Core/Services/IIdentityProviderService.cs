using System;
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Services
{
	/// <summary>
	/// Represents an identity provider
	/// </summary>
	public interface IIdentityProviderService
	{

		/// <summary>
		/// Authenticate the user
		/// </summary>
		IPrincipal Authenticate(string userName, string password);

		/// <summary>
		/// Gets an un-authenticated identity
		/// </summary>
		IIdentity GetIdentity(string userName);

		/// <summary>
		/// Authenticate the user using a TwoFactorAuthentication secret
		/// </summary>
		IPrincipal Authenticate(string userName, string password, string tfaSecret);

		/// <summary>
		/// Change the user's password
		/// </summary>
		void ChangePassword(string userName, string newPassword, IPrincipal principal);

	}
}

