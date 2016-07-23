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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Android.Services.Attributes;
using OpenIZ.Mobile.Core.Android.Security;
using System.Security;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Query;

namespace OpenIZ.Mobile.Core.Android.Services.ServiceHandlers
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
        public SessionInformation Authenticate([RestMessage(RestMessageFormat.FormData)] NameValueCollection authRequest)
        {

            switch (authRequest["grant_type"][0])
            {
                case "password":
                    AndroidApplicationContext.Current.Authenticate(authRequest["username"].FirstOrDefault(), authRequest["password"].FirstOrDefault());
                    break;
                case "refresh":
                    var idp = ApplicationContext.Current.GetService<IIdentityProviderService>();
                    var principal = idp.Authenticate(ApplicationContext.Current.Principal, null); // Force a re-issue
                    AndroidApplicationContext.Current.SetPrincipal(principal);
                    break;
            }

            // Authenticated?
            if (ApplicationContext.Current.Principal == null)
                throw new SecurityException();
            else
                return new SessionInformation(ApplicationContext.Current.Principal);
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
        [return: RestMessage(RestMessageFormat.Json)]
        public SessionInformation GetSession()
        {
            if (ApplicationContext.Current.Principal == null)
                return null;
            else 
                return new SessionInformation(ApplicationContext.Current.Principal);
        }
    }
}
