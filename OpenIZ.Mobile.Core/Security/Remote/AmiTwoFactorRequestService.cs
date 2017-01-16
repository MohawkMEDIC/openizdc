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
 * Date: 2017-1-16
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Mobile.Core.Interop;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Core.Services;

namespace OpenIZ.Mobile.Core.Security.Remote
{
    /// <summary>
    /// AMI based password reset service
    /// </summary>
    public class AmiTwoFactorRequestService : ITwoFactorRequestService
    {

        // Authentication context
        private AuthenticationContext m_authContext;

        /// <summary>
        /// Ensure the current client is authenticated
        /// </summary>
        private void EnsureAuthenticated()
        {
            var appConfig = ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>();

            if (this.m_authContext == null ||
                ((this.m_authContext.Principal as ClaimsPrincipal)?.FindClaim(ClaimTypes.Expiration)?.AsDateTime().ToLocalTime() ?? DateTime.MinValue) < DateTime.Now)
                this.m_authContext = new AuthenticationContext(ApplicationContext.Current.GetService<IIdentityProviderService>().Authenticate(appConfig.DeviceName, appConfig.DeviceSecret));
        }

        /// <summary>
        /// Get the reset mechanisms
        /// </summary>
        public List<TfaMechanismInfo> GetResetMechanisms()
        {
            this.EnsureAuthenticated();
            using (AmiServiceClient amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami")))
            {
                var authContext = AuthenticationContext.Current;
                AuthenticationContext.Current = this.m_authContext;
                var retVal = amiClient.GetTwoFactorMechanisms().CollectionItem;
                AuthenticationContext.Current = authContext;
                return retVal;
            }
        }


        /// <summary>
        /// Send the verification code
        /// </summary>
        public void SendVerificationCode(Guid mechanism, string challengeResponse, string userName, string scope)
        {
            this.EnsureAuthenticated();
            using (AmiServiceClient amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami")))
            {
                var authContext = AuthenticationContext.Current;
                AuthenticationContext.Current = this.m_authContext;

                // Next I have to request a TFA secret!!
                amiClient.SendTfaSecret(new  TfaRequestInfo()
                {
                    ResetMechanism = mechanism,
                    Verification = challengeResponse,
                    UserName = userName,
                    Purpose = scope
                });

                AuthenticationContext.Current = authContext;
            }
        }
    }
}
