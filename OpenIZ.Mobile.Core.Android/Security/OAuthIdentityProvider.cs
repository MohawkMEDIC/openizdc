using System;
using System.Linq;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Http;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Security;
using Newtonsoft.Json;
using System.Security;
using OpenIZ.Mobile.Core.Android.Exceptions;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Serices;

namespace OpenIZ.Mobile.Core.Android.Security
{
	/// <summary>
	/// Represents an OAuthIdentity provider
	/// </summary>
	public class OAuthIdentityProvider: IIdentityProviderService
	{

		/// <summary>
		/// OAuth token response.
		/// </summary>
		[JsonObject]
		private class OAuthTokenResponse
		{

			/// <summary>
			/// Gets or sets the error
			/// </summary>
			[JsonProperty("error")]
			public String Error { get; set; }

			/// <summary>
			/// Description of the error
			/// </summary>
			[JsonProperty("error_description")]
			public String ErrorDescription { get; set; }

			/// <summary>
			/// Access token
			/// </summary>
			[JsonProperty("access_token")]
			public String AccessToken { get; set; }

			/// <summary>
			/// Token type
			/// </summary>
			[JsonProperty("token_type")]
			public String TokenType { get; set; }

			/// <summary>
			/// Expires in
			/// </summary>
			[JsonProperty("expires_in")]
			public int ExpiresIn { get; set; }

			/// <summary>
			/// Refresh token
			/// </summary>
			[JsonProperty("refresh_token")]
			public String RefreshToken { get; set; }

			/// <summary>
			/// Represent the object as a string
			/// </summary>
			public override string ToString ()
			{
				return string.Format ("[OAuthTokenResponse: Error={0}, ErrorDescription={1}, AccessToken={2}, TokenType={3}, ExpiresIn={4}, RefreshToken={5}]", Error, ErrorDescription, AccessToken, TokenType, ExpiresIn, RefreshToken);
			}
		}

		/// <summary>
		/// OAuth token request.
		/// </summary>
		private class OAuthTokenRequest
		{

			/// <summary>
			/// Initializes a new instance of the
			/// <see cref="OpenIZ.Mobile.Core.Android.Security.OAuthTokenServiceCredentials+OAuthTokenRequest"/> class.
			/// </summary>
			/// <param name="username">Username.</param>
			/// <param name="password">Password.</param>
			/// <param name="scope">Scope.</param>
			public OAuthTokenRequest (String username, String password, String scope)
			{
				this.Username = username;
				this.Password = password;
				this.Scope = scope;
			}

			/// <summary>
			/// Gets the type of the grant.
			/// </summary>
			/// <value>The type of the grant.</value>
			[FormElement("grant_type")]
			public String GrantType {
				get { return "password"; }
			}

			/// <summary>
			/// Gets the username.
			/// </summary>
			/// <value>The username.</value>
			[FormElement("username")]
			public String Username {
				get;
				private set;
			}

			/// <summary>
			/// Gets the password.
			/// </summary>
			/// <value>The password.</value>
			[FormElement("password")]
			public String Password {
				get;
				private set;
			}

			/// <summary>
			/// Gets the scope.
			/// </summary>
			/// <value>The scope.</value>
			[FormElement("scope")]
			public String Scope {
				get;
				private set;
			}

		}


		// Tracer
		private Tracer m_tracer = Tracer.GetTracer(typeof(OAuthIdentityProvider));

		#region IIdentityProviderService implementation
		/// <summary>
		/// Occurs when authenticating.
		/// </summary>
		public event EventHandler<AuthenticatingEventArgs> Authenticating;
		/// <summary>
		/// Occurs when authenticated.
		/// </summary>
		public event EventHandler<AuthenticatedEventArgs> Authenticated;
		/// <summary>
		/// Authenticate the user
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <param name="password">Password.</param>
		public System.Security.Principal.IPrincipal Authenticate (string userName, string password)
		{
			return this.Authenticate (new GenericPrincipal (new GenericIdentity (userName), null), password);
		}

		/// <summary>
		/// Authenticate the user
		/// </summary>
		/// <param name="principal">Principal.</param>
		/// <param name="password">Password.</param>
		public System.Security.Principal.IPrincipal Authenticate (System.Security.Principal.IPrincipal principal, string password)
		{

			AuthenticatingEventArgs e = new AuthenticatingEventArgs (principal.Identity.Name, password) { Principal = principal };
			this.Authenticating?.Invoke (this, e);
			if (e.Cancel) {
				this.m_tracer.TraceWarning ("Pre-Event ordered cancel of auth {0}", principal);
				return e.Principal;
			}

			// Get the scope being requested
			String scope = "*";
			if (principal is ClaimsPrincipal)
				scope = (principal as ClaimsPrincipal).Claims.FirstOrDefault (o => o.Type == ClaimTypes.OpenIzScopeClaim)?.Value ?? scope;
			else
				scope = ApplicationContext.Current.GetRestClient ("imsi").Description.Endpoint [0].Address;
			
			// Authenticate
			ClaimsPrincipal retVal = null;
			using (IRestClient restClient = ApplicationContext.Current.GetRestClient ("acs")) {

				try
				{
					// Set credentials
					restClient.Credentials = new OAuthTokenServiceCredentials(principal);

					// Invoke
					OAuthTokenResponse response = restClient.Post<OAuthTokenRequest, OAuthTokenResponse>("oauth2_token", "application/x-www-urlform-encoded", new OAuthTokenRequest(principal.Identity.Name, password, scope));
					retVal = new TokenClaimsPrincipal (response.AccessToken, response.TokenType);

					// Create a security user and ensure they exist!
					// TODO: Use the business services instead
					IDataPersistenceService<SecurityUser> persistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
					Guid sidKey = Guid.Parse(retVal.FindClaim(ClaimTypes.Sid).Value);
					var localUser = persistence.Get(sidKey);
					if(localUser == null)
					{
						persistence.Insert(new SecurityUser() {
							Email = retVal.FindClaim(ClaimTypes.Email)?.Value,
							LastLoginTime= DateTime.Now,
							PasswordHash = ApplicationContext.Current.GetService<IPasswordHashingService>().ComputeHash(password),
							SecurityHash = Guid.NewGuid().ToString(),
							UserName = retVal.Identity.Name,
							Key = sidKey
						});
					}
					else 
					{
						localUser.PasswordHash = ApplicationContext.Current.GetService<IPasswordHashingService>().ComputeHash(password);
						persistence.Update(localUser);
					}
				}
				catch(RestClientException<OAuthTokenResponse> ex)
				{
					this.m_tracer.TraceError("REST client exception: {0}", ex);
					throw new SecurityException(
						String.Format("err_oauth2_{0}", ex.Result.Error), 
						ex
					);
				}
				catch(SecurityTokenException ex) {
					this.m_tracer.TraceError ("TOKEN exception: {0}", ex);
					throw new SecurityException (
						String.Format ("err_token_{0}", ex.Type),
						ex
					);
				}
				catch(Exception ex)
				{
					this.m_tracer.TraceError("Generic exception: {0}", ex);
					throw new SecurityException(
						"err_exception",
						ex);
				}
			}

			this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(principal.Identity.Name, password) { Principal = retVal });
			return retVal;
		}
		public System.Security.Principal.IIdentity GetIdentity (string userName)
		{
			throw new NotImplementedException ();
		}
		public System.Security.Principal.IPrincipal Authenticate (string userName, string password, string tfaSecret)
		{
			throw new NotImplementedException ();
		}
		public void ChangePassword (string userName, string newPassword, System.Security.Principal.IPrincipal principal)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

