/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-10-11
 */
using System;
using System.Linq;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Configuration;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Security;
using Newtonsoft.Json;
using System.Security;
using OpenIZ.Mobile.Core.Xamarin.Exceptions;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Serices;
using OpenIZ.Core.Http;
using OpenIZ.Mobile.Core.Interop;
using System.Net;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using System.Text;

namespace OpenIZ.Mobile.Core.Xamarin.Security
{
    /// <summary>
    /// Represents an OAuthIdentity provider
    /// </summary>
    public class OAuthIdentityProvider : IIdentityProviderService
    {
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
        public System.Security.Principal.IPrincipal Authenticate(string userName, string password)
        {
            return this.Authenticate(new GenericPrincipal(new GenericIdentity(userName), null), password);
        }

        /// <summary>
        /// Perform authentication with specified password
        /// </summary>
        public System.Security.Principal.IPrincipal Authenticate(System.Security.Principal.IPrincipal principal, string password)
        {
            return this.Authenticate(principal, password, null);
        }

        /// <summary>
        /// Authenticate the user
        /// </summary>
        /// <param name="principal">Principal.</param>
        /// <param name="password">Password.</param>
        public System.Security.Principal.IPrincipal Authenticate(System.Security.Principal.IPrincipal principal, string password, String tfaSecret)
        {

            AuthenticatingEventArgs e = new AuthenticatingEventArgs(principal.Identity.Name, password) { Principal = principal };
            this.Authenticating?.Invoke(this, e);
            if (e.Cancel)
            {
                this.m_tracer.TraceWarning("Pre-Event ordered cancel of auth {0}", principal);
                return e.Principal;
            }

            // Get the scope being requested
            String scope = "*";
            if (principal is ClaimsPrincipal)
                scope = (principal as ClaimsPrincipal).Claims.FirstOrDefault(o => o.Type == ClaimTypes.OpenIzScopeClaim)?.Value ?? scope;
            else
                scope = ApplicationContext.Current.GetRestClient("imsi")?.Description.Endpoint[0].Address ??
                    ApplicationContext.Current.GetRestClient("ami")?.Description.Endpoint[0].Address ??
                    "*";

            // Authenticate
            IPrincipal retVal = null;
            var localIdp = new LocalIdentityService();

            using (IRestClient restClient = ApplicationContext.Current.GetRestClient("acs"))
            {

                try
                {
                    // Set credentials
                    restClient.Credentials = new OAuthTokenServiceCredentials(principal);

                    // Create grant information
                    OAuthTokenRequest request = null;
                    if (!String.IsNullOrEmpty(password))
                        request = new OAuthTokenRequest(principal.Identity.Name, password, scope);
                    else if (principal is TokenClaimsPrincipal)
                        request = new OAuthTokenRequest(principal as TokenClaimsPrincipal, scope);
                    else
                        request = new OAuthTokenRequest(principal.Identity.Name, null, scope);

                    try
                    {
                        restClient.Requesting += (o, p) =>
                        {
                            p.AdditionalHeaders.Add("X-OpenIZClient-Claim", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}={1}", ClaimTypes.OpenIzScopeClaim, scope))));
                            if (!String.IsNullOrEmpty(tfaSecret))
                                p.AdditionalHeaders.Add("X-OpenIZ-TfaSecret", tfaSecret);
                        };

                        // Invoke
                        OAuthTokenResponse response = restClient.Post<OAuthTokenRequest, OAuthTokenResponse>("oauth2_token", "application/x-www-urlform-encoded", request);
                        retVal = new TokenClaimsPrincipal(response.AccessToken, response.TokenType, response.RefreshToken);
                    }
                    catch (WebException ex) // Raw level web exception
                    {
                        // Not network related, but a protocol level error
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                            throw;

                        this.m_tracer.TraceWarning("Original OAuth2 request failed trying local. {0}", ex);
                        try
                        {
                            retVal = localIdp.Authenticate(principal.Identity.Name, password);
                        }
                        catch
                        {
                            throw new SecurityException(Strings.err_offline_use_cache_creds);
                        }
                    }
                    catch (SecurityException ex)
                    {
                        this.m_tracer.TraceError("Server was contacted however the token is invalid: {0}", ex);
                        throw;
                    }
                    catch (Exception ex) // fallback to local
                    {
                        try
                        {
                            this.m_tracer.TraceWarning("Original OAuth2 request failed trying local. {0}", ex);
                            retVal = localIdp.Authenticate(principal.Identity.Name, password);
                        }
                        catch
                        {
                            throw new SecurityException(Strings.err_offline_use_cache_creds);
                        }
                    }

                    // Create a security user and ensure they exist!
                    var localRp = new LocalRoleProviderService();
                    var localPip = new LocalPolicyInformationService();

                    // We have a match! Lets make sure we cache this data
                    // TODO: Clean this up
                    if (!String.IsNullOrEmpty(password) && retVal is ClaimsPrincipal && 
                        XamarinApplicationContext.Current.ConfigurationManager.IsConfigured)
                    {
                        ClaimsPrincipal cprincipal = retVal as ClaimsPrincipal;
                        var amiPip = new AmiPolicyInformationService(cprincipal);

                        // We want to impersonate SYSTEM
                        //AndroidApplicationContext.Current.SetPrincipal(cprincipal);

                        // Ensure policies exist from the claim
                        foreach (var itm in cprincipal.Claims.Where(o => o.Type == ClaimTypes.OpenIzGrantedPolicyClaim))
                        {
                            if (localPip.GetPolicy(itm.Value) == null)
                            {
                                var policy = amiPip.GetPolicy(itm.Value);
                                localPip.CreatePolicy(policy, new SystemPrincipal());
                            }
                        }

                        // Ensure roles exist from the claim
                        var localRoles = localRp.GetAllRoles();
                        foreach (var itm in cprincipal.Claims.Where(o => o.Type == ClaimsIdentity.DefaultRoleClaimType))
                        {
                            // Ensure policy exists
                            var amiPolicies = amiPip.GetActivePolicies(new SecurityRole() { Name = itm.Value }).ToArray();
                            foreach(var pol in amiPolicies)
                                if (localPip.GetPolicy(pol.Policy.Oid) == null)
                                {
                                    var policy = amiPip.GetPolicy(pol.Policy.Oid);
                                    localPip.CreatePolicy(policy, new SystemPrincipal());
                                }

                            // Local role doesn't exist
                            if (!localRoles.Contains(itm.Value))
                            {
                                localRp.CreateRole(itm.Value, new SystemPrincipal());
                            }
                            localRp.AddPoliciesToRoles(amiPolicies, new String[] { itm.Value }, new SystemPrincipal());

                        }

                        IIdentity localUser = XamarinApplicationContext.Current.ConfigurationManager.IsConfigured ? localIdp.GetIdentity(principal.Identity.Name) : null;
                        try
                        {
                            if (localUser == null)
                                localIdp.CreateIdentity(Guid.Parse(cprincipal.FindClaim(ClaimTypes.Sid).Value), principal.Identity.Name, password);
                            else
                            {
                                localIdp.ChangePassword(principal.Identity.Name, password, principal);
                                localIdp.Authenticate(principal, password);
                            }
                        }
                        catch(Exception ex)
                        {
                            this.m_tracer.TraceWarning("Insertion of local cache credential failed: {0}", ex);
                        }

                        // Add user to roles
                        // TODO: Remove users from specified roles?
                        localRp.AddUsersToRoles(new String[] { principal.Identity.Name }, cprincipal.Claims.Where(o => o.Type == ClaimsIdentity.DefaultRoleClaimType).Select(o => o.Value).ToArray(), new SystemPrincipal());

                    }
                }
                catch (RestClientException<OAuthTokenResponse> ex)
                {
                    this.m_tracer.TraceError("REST client exception: {0}", ex);
                    var se = new SecurityException(
                        String.Format("err_oauth2_{0}", ex.Result.Error),
                        ex
                    );
                    se.Data.Add("detail", ex.Result);
                    throw se;
                }
                catch (SecurityTokenException ex)
                {
                    this.m_tracer.TraceError("TOKEN exception: {0}", ex);
                    throw new SecurityException(
                        String.Format("err_token_{0}", ex.Type),
                        ex
                    );
                }
                catch(SecurityException ex)
                {
                    this.m_tracer.TraceError("Security exception: {0}", ex);
                    throw;
                }
                catch (Exception ex)
                {
                    this.m_tracer.TraceError("Generic exception: {0}", ex);
                    throw new SecurityException(
                        Strings.err_authentication_exception,
                        ex);
                }
            }

            this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(principal.Identity.Name, password) { Principal = retVal });
            return retVal;
        }
        /// <summary>
        /// Gets the specified identity
        /// </summary>
		public System.Security.Principal.IIdentity GetIdentity(string userName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Authenticates the specified user
        /// </summary>
		public System.Security.Principal.IPrincipal Authenticate(string userName, string password, string tfaSecret)
        {
            return this.Authenticate(new GenericPrincipal(new GenericIdentity(userName), null), password, tfaSecret);
        }

        /// <summary>
        /// Changes the users password.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="newPassword">The new password of the user.</param>
        /// <param name="principal">The authentication principal (the user that is changing the password).</param>
        public void ChangePassword(string userName, string newPassword, System.Security.Principal.IPrincipal principal)
        {

            try
            {
                // The principal must change their own password or must have the changepassword credential
                if (!userName.Equals(principal.Identity.Name, StringComparison.InvariantCultureIgnoreCase))
                    new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PolicyIdentifiers.ChangePassword).Demand();
                else if (!principal.Identity.IsAuthenticated)
                    throw new InvalidOperationException("Unauthenticated principal cannot change user password");

                // Get the user's identity
                var securityUserService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
                using (AmiServiceClient client = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami")))
                {
                    client.Client.Accept = "application/xml";

                    Guid userId = Guid.Empty;
                    if (principal is ClaimsPrincipal)
                    {
                        var subjectClaim = (principal as ClaimsPrincipal).FindClaim(ClaimTypes.Sid);
                        if (subjectClaim != null)
                            userId = Guid.Parse(subjectClaim.Value);
                    }

                    // User ID not found - lookup
                    if (userId == Guid.Empty)
                    { 
                        // User service is null
                        var securityUser = securityUserService.GetUser(principal.Identity);
                        if (securityUser == null)
                        {
                            var tuser = client.GetUsers(o => o.UserName == principal.Identity.Name).CollectionItem.FirstOrDefault();
                            if (tuser == null)
                                throw new ArgumentException(string.Format("User {0} not found", userName));
                            else
                                userId = tuser.UserId.Value;
                        }
                        else
                            userId = securityUser.Key.Value;
                    }
                    // Use the current configuration's credential provider
                    var user = new SecurityUserInfo()
                    {
                        UserId = userId,
                        UserName = userName,
                        Password = newPassword
                    };

                    // Set the credentials 
                    client.Client.Credentials = ApplicationContext.Current.Configuration.GetServiceDescription("ami").Binding.Security.CredentialProvider.GetCredentials(principal);

                    client.UpdateUser(user.UserId.Value, user);
                }

            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error changing password for user {0} : {1}", userName, e);
                throw;
            }

		}

		/// <summary>
		/// Changes the users password.
		/// </summary>
		/// <param name="userName">The username of the user.</param>
		/// <param name="password">The new password of the user.</param>
        public void ChangePassword(string userName, string password)
        {
			this.ChangePassword(userName, password, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Creates an identity
        /// </summary>
        public IIdentity CreateIdentity(string userName, string password)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets the user's lockout status
        /// </summary>
        public void SetLockout(string userName, bool v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified identity
        /// </summary>
        public void DeleteIdentity(string userName)
        {
            throw new NotImplementedException();
        }

        public IIdentity CreateIdentity(Guid sid, string userName, string password)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

