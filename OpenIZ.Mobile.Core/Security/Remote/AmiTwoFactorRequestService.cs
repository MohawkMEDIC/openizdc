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
                DateTime.Parse((this.m_authContext.Principal as ClaimsPrincipal)?.FindClaim(ClaimTypes.Expiration)?.Value ?? "0001-01-01") < DateTime.Now)
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
