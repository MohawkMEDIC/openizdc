using OpenIZ.Core.Model.AMI.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Represents a password reset service
    /// </summary>
    public interface ITwoFactorRequestService
    {

        /// <summary>
        /// Gets the TFA mechanisms
        /// </summary>
        List<TfaMechanismInfo> GetResetMechanisms();

        /// <summary>
        /// Send a verification code to the specified user 
        /// </summary>
        void SendVerificationCode(Guid mechanism, String challengeResponse, String userName, String scope);

    }
}
