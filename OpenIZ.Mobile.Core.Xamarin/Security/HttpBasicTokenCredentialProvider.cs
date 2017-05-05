using OpenIZ.Core.Http;
using OpenIZ.Mobile.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Security
{
    /// <summary>
    /// Represents a basic token crendtial provider
    /// </summary>
    public class HttpBasicTokenCredentialProvider : ICredentialProvider
    {
        #region ICredentialProvider implementation
        /// <summary>
        /// Gets or sets the credentials which are used to authenticate
        /// </summary>
        /// <returns>The credentials.</returns>
        /// <param name="context">Context.</param>
        public Credentials GetCredentials(IRestClient context)
        {
            return this.GetCredentials(AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Authenticate a user - this occurs when reauth is required
        /// </summary>
        /// <param name="context">Context.</param>
        public Credentials Authenticate(IRestClient context)
        {
            throw new SecurityException();
        }

        /// <summary>
        /// Get credentials from the specified principal
        /// </summary>
        public Credentials GetCredentials(IPrincipal principal)
        {
            if (principal is ClaimsPrincipal)
                return new HttpBasicCredentials(principal, (principal as ClaimsPrincipal)?.FindClaim("passwd")?.Value);
            else
                throw new InvalidOperationException("Cannot create basic principal from non-claims principal");
        }
        #endregion
    }
}
