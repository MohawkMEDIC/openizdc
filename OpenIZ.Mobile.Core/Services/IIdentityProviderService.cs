using System;
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Services
{

	/// <summary>
	/// Authenticating event arguments.
	/// </summary>
	public class AuthenticatingEventArgs : AuthenticatedEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.AuthenticatingEventArgs"/> class.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <param name="password">Password.</param>
		public AuthenticatingEventArgs (String userName, String password) : base(userName, password)
		{
			
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance cancel.
		/// </summary>
		/// <value><c>true</c> if this instance cancel; otherwise, <c>false</c>.</value>
		public bool Cancel {
			get;
			set;
		}
	}

	/// <summary>
	/// Authentication event args.
	/// </summary>
	public class AuthenticatedEventArgs : EventArgs
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.AuthenticatingEventArgs"/> class.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <param name="password">Password.</param>
		public AuthenticatedEventArgs (String userName, String password)
		{
			this.UserName = userName;
			this.Password = password;
		}

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		public String UserName {
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public String Password {
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the principal.
		/// </summary>
		/// <value>The principal.</value>
		public IPrincipal Principal {
			get;
			set;
		}

	}

	/// <summary>
	/// Represents an identity provider
	/// </summary>
	public interface IIdentityProviderService
	{

		/// <summary>
		/// Occurs when authenticating.
		/// </summary>
		event EventHandler<AuthenticatingEventArgs> Authenticating;

		/// <summary>
		/// Occurs when authenticated.
		/// </summary>
		event EventHandler<AuthenticatedEventArgs> Authenticated;

		/// <summary>
		/// Authenticate the user
		/// </summary>
		IPrincipal Authenticate(string userName, string password);

		/// <summary>
		/// Authenticate the specified principal with the password
		/// </summary>
		/// <param name="principal">Principal.</param>
		/// <param name="password">Password.</param>
		IPrincipal Authenticate(IPrincipal principal, string password);

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

