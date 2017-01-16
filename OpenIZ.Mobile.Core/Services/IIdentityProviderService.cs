/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2016-10-25
 */
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

        /// <summary>
        /// Changes the user's password
        /// </summary>
        void ChangePassword(string userName, string password);

        /// <summary>
        /// Creates the specified user
        /// </summary>
        IIdentity CreateIdentity(string userName, string password);

        /// <summary>
        /// Create an identity with the specified data
        /// </summary>
        IIdentity CreateIdentity(Guid sid, String userName, String password);

        /// <summary>
        /// Locks the user account out
        /// </summary>
        void SetLockout(string userName, bool v);

        /// <summary>
        /// Deletes the specified identity
        /// </summary>
        /// <param name="userName"></param>
        void DeleteIdentity(string userName);
    }
}

