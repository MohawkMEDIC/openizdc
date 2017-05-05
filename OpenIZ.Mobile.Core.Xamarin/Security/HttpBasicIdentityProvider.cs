using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Xamarin.Security
{
    /// <summary>
    /// Represents an HTTP BASIC identity provider
    /// </summary>
    public class HttpBasicIdentityProvider : IIdentityProviderService
    {
        /// <summary>
        /// Authenticated and authenticating args
        /// </summary>
        public event EventHandler<AuthenticatedEventArgs> Authenticated;
        public event EventHandler<AuthenticatingEventArgs> Authenticating;

        public IPrincipal Authenticate(IPrincipal principal, string password)
        {
            throw new NotImplementedException();
        }

        public IPrincipal Authenticate(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public IPrincipal Authenticate(string userName, string password, string tfaSecret)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string userName, string newPassword, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public IIdentity CreateIdentity(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public IIdentity CreateIdentity(Guid sid, string userName, string password)
        {
            throw new NotImplementedException();
        }

        public void DeleteIdentity(string userName)
        {
            throw new NotImplementedException();
        }

        public IIdentity GetIdentity(string userName)
        {
            throw new NotImplementedException();
        }

        public void SetLockout(string userName, bool v)
        {
            throw new NotImplementedException();
        }
    }
}
