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
 * Date: 2016-7-23
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

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents a service which handles authentication requests
    /// </summary>
    [RestService("/__auth")]
    public class AuthenticationService
    {
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

            switch (authRequest["grant_type"][0])
            {
                case "password":
                    retVal = sessionService.Authenticate(authRequest["username"].FirstOrDefault().ToLower(), authRequest["password"].FirstOrDefault());
                    break;
                case "refresh":
                    retVal = sessionService.Refresh(AuthenticationContext.Current.Session, null); // Force a re-issue
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
                if(!authRequest.ContainsKey("scope"))
                    MiniImsServer.CurrentContext.Response.SetCookie(new Cookie("_s", retVal.Key.ToString())
                    {
                        Expires = retVal.Expiry,
                        HttpOnly = false,
                        Secure = true,
                        Path ="/"
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
		[RestOperation(Method = "GET", UriPath = "/get_user")]
		[return: RestMessage(RestMessageFormat.Json)]
		public SecurityUser GetUser()
		{
            // this is used for the forgot password functionality
            // need to find a way to stop people from simply searching users via username...
            NameValueCollection query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var predicate = QueryExpressionParser.BuildLinqExpression<SecurityUser>(query);

			ISecurityRepositoryService securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			var user = securityRepositoryService.FindUsers(predicate).FirstOrDefault();

			return user;
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
