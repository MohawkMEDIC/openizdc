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
 * User: justi
 * Date: 2017-3-31
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Mobile.Core.Xamarin.Security;
using System.Security;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Security;
using System.Globalization;
using OpenIZ.Mobile.Core.Security;
using System.Net;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Mobile.Core.Interop;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using OpenIZ.Mobile.Core.Security.Audit;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.AMI.Security;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents a service which handles authentication requests
    /// </summary>
    [RestService("/__auth")]
    public class AuthenticationService
    {

        // Get tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(AuthenticationService));

        /// <summary>
        /// Abandons the users session.
        /// </summary>
        /// <returns>Returns an empty session.</returns>
        [RestOperation(Method = "POST", UriPath = "/abandon", FaultProvider = nameof(AuthenticationFault))]
        public SessionInfo Abandon()
        {
            var cookie = MiniImsServer.CurrentContext.Request.Cookies["_s"];

            var value = Guid.Empty;

            if (cookie != null && Guid.TryParse(cookie.Value, out value))
            {
                ISessionManagerService sessionService = ApplicationContext.Current.GetService<ISessionManagerService>();
                var sessionInfo = sessionService.Delete(value);
                AuditUtil.AuditLogout(sessionInfo.Principal);
            }

            return new SessionInfo();
        }


        /// <summary>
        /// Update the security user
        /// </summary>
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        [RestOperation(UriPath = "/SecurityUser", Method = "POST", FaultProvider = nameof(AuthenticationFault))]
        public SecurityUser UpdateSecurityUser([RestMessage(RestMessageFormat.SimpleJson)] SecurityUser user)
        {
            var localSecSrv = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
            var amiServ = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));

            // Session
            amiServ.Client.Credentials = new TokenCredentials(AuthenticationContext.Current.Principal);
            var remoteUser = amiServ.GetUser(user.Key.ToString());
            remoteUser.User.Email = user.Email;
            remoteUser.User.PhoneNumber = user.PhoneNumber;
            // Save the remote user in the local
            localSecSrv.SaveUser(remoteUser.User);
            amiServ.UpdateUser(remoteUser.UserId.Value, remoteUser);
            return remoteUser.User;
        }

        /// <summary>
        /// Gets the TFA authentication mechanisms
        /// </summary>
        [RestOperation(Method = "POST", UriPath = "/tfa", FaultProvider = nameof(AuthenticationFault))]
        [return: RestMessage(RestMessageFormat.Json)]
        public bool SendTfaSecret([RestMessage(RestMessageFormat.Json)]TfaRequestInfo resetInfo)
        {
            try
            {
                var resetService = ApplicationContext.Current.GetService<ITwoFactorRequestService>();
                if (resetService == null)
                    throw new InvalidOperationException(Strings.err_reset_not_supported);
                resetService.SendVerificationCode(resetInfo.ResetMechanism, resetInfo.Verification, resetInfo.UserName, resetInfo.Purpose);
                return true;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error getting sending secret: {0}", e);
                throw;
            }


        }
        /// <summary>
        /// Gets the TFA authentication mechanisms
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/tfa", FaultProvider = nameof(AuthenticationFault))]
        [return: RestMessage(RestMessageFormat.Json)]
        public List<TfaMechanismInfo> GetTfaMechanisms()
        {
            try
            {
                var resetService = ApplicationContext.Current.GetService<ITwoFactorRequestService>();
                if (resetService == null)
                    throw new InvalidOperationException(Strings.err_reset_not_supported);
                return resetService.GetResetMechanisms();

            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error getting TFA mechanisms: {0}", e);
                throw;
            }

        }

        /// <summary>
        /// Authenticate the user returning the session if successful
        /// </summary>
        /// <param name="authRequest"></param>
        /// <returns></returns>
        [RestOperation(Method = "POST", UriPath = "/authenticate", FaultProvider = nameof(AuthenticationFault))]
        [return: RestMessage(RestMessageFormat.Json)]
        public SessionInfo Authenticate([RestMessage(RestMessageFormat.FormData)] NameValueCollection authRequest)
        {

            ISessionManagerService sessionService = ApplicationContext.Current.GetService<ISessionManagerService>();
            SessionInfo retVal = null;

            List<String> usernameColl = null,
                tfaSecretColl = null,
                passwordColl = null;
            authRequest.TryGetValue("username", out usernameColl);
            authRequest.TryGetValue("password", out passwordColl);
            authRequest.TryGetValue("tfaSecret", out tfaSecretColl);

            String username = usernameColl?.FirstOrDefault().ToLower(),
                password = passwordColl?.FirstOrDefault(),
                tfaSecret = tfaSecretColl?.FirstOrDefault();

            switch (authRequest["grant_type"][0])
            {
                case "password":
                    retVal = sessionService.Authenticate(username, password);
                    break;
                case "refresh":
                    retVal = sessionService.Refresh(AuthenticationContext.Current.Session, null); // Force a re-issue
                    break;
                case "tfa":
                    retVal = sessionService.Authenticate(username, password, tfaSecret);
                    break;
            }

            if (retVal == null)
            {
                throw new SecurityException();
            }
            else
            {
                var lanugageCode = retVal?.UserEntity?.LanguageCommunication?.FirstOrDefault(o => o.IsPreferred)?.LanguageCode;

                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(CultureInfo.DefaultThreadCurrentUICulture?.TwoLetterISOLanguageName ?? "en");

                if (lanugageCode != null)
                    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(lanugageCode);

                // Set the session 
                if (!authRequest.ContainsKey("scope"))
                    MiniImsServer.CurrentContext.Response.SetCookie(new Cookie("_s", retVal.Key.ToString())
                    {

                        HttpOnly = true,
                        Secure = true,
                        Path = "/",
                        Domain = MiniImsServer.CurrentContext.Request.Url.Host
                    });
                return retVal;
            }
        }

        /// <summary>
        /// Authentication fault
        /// </summary>
        public OAuthTokenResponse AuthenticationFault(Exception e)
        {
            if (e.Data.Contains("detail"))
                return e.Data["detail"] as OAuthTokenResponse;
            else
                return new OAuthTokenResponse()
                {
                    Error = e.Message,
                    ErrorDescription = e.InnerException?.Message
                };
        }


        /// <summary>
        /// Authenticate the user returning the session if successful
        /// </summary>
        /// <param name="authRequest"></param>
        /// <returns></returns>
        [RestOperation(Method = "GET", UriPath = "/get_session")]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public SessionInfo GetSession()
        {
            NameValueCollection query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            ISessionManagerService sessionService = ApplicationContext.Current.GetService<ISessionManagerService>();

            if (query.ContainsKey("_id"))
                return sessionService.Get(Guid.Parse(query["_id"][0]));
            else
                return AuthenticationContext.Current.Session;
        }

        /// <summary>
        /// Gets a user by username.
        /// </summary>
        /// <param name="username">The username of the user to be retrieved.</param>
        /// <returns>Returns the user.</returns>
        [RestOperation(Method = "GET", UriPath = "/SecurityUser")]
        [return: RestMessage(RestMessageFormat.Json)]
        public IdentifiedData GetUser()
        {
            // this is used for the forgot password functionality
            // need to find a way to stop people from simply searching users via username...

            NameValueCollection query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var predicate = QueryExpressionParser.BuildLinqExpression<SecurityUser>(query);
            ISecurityRepositoryService securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

            if (query.ContainsKey("_id"))
                return securityRepositoryService.GetUser(Guid.Parse(query["_id"][0]));
            else
                return Bundle.CreateBundle(securityRepositoryService.FindUsers(predicate), 0, 0);
        }

        /// <summary>
        /// Sets the user's password
        /// </summary>
        [RestOperation(Method = "POST", UriPath = "/passwd", FaultProvider = nameof(AuthenticationFault))]
        [return: RestMessage(RestMessageFormat.Json)]
        public SessionInfo SetPassword([RestMessage(RestMessageFormat.FormData)]NameValueCollection controlData)
        {
            var idp = ApplicationContext.Current.GetService<IIdentityProviderService>();
            idp.ChangePassword(controlData["username"].FirstOrDefault().ToLower(), controlData["password"].FirstOrDefault(), AuthenticationContext.Current.Principal);
            return AuthenticationContext.Current.Session;
        }
    }
}
