using OpenIZ.Mobile.Core.Resources;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Memory session manager service
    /// </summary>
    public class MemorySessionManagerService : ISessionManagerService
    {

        /// <summary>
        /// Sessions 
        /// </summary>
        private Dictionary<Guid, SessionInfo> m_session = new Dictionary<Guid, SessionInfo>();

        /// <summary>
        /// Authenticate the user and establish a sessions
        /// </summary>
        public SessionInfo Authenticate(string userName, string password)
        {
            var idp = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var principal = idp.Authenticate(userName, password);
            if (principal == null)
                throw new SecurityException(Strings.locale_sessionError);
            else
            {
                var session = new SessionInfo(principal);
                session.Key = Guid.NewGuid();
                this.m_session.Add(session.Key.Value, session);
                return session;
            }
        }

        /// <summary>
        /// Deletes the specified session
        /// </summary>
        public SessionInfo Delete(Guid sessionId)
        {
            SessionInfo ses = null;
            if (this.m_session.TryGetValue(sessionId, out ses))
                this.m_session.Remove(sessionId);
            return ses;
        }

        /// <summary>
        /// Get the specified session
        /// </summary>
        public SessionInfo Get(Guid sessionId)
        {
            SessionInfo ses = null;
            if (!this.m_session.TryGetValue(sessionId, out ses))
                return null;
            return ses;
        }

        /// <summary>
        /// Refreshes the specified session
        /// </summary>
        public SessionInfo Refresh(SessionInfo session, String password)
        {
            var idp = ApplicationContext.Current.GetService<IIdentityProviderService>();

            // First is this a valid session?
            if (!this.m_session.ContainsKey(session.Key.Value))
                throw new KeyNotFoundException();

            var principal = idp.Authenticate(session.Principal, password);
            if (principal == null)
                throw new SecurityException(Strings.locale_sessionError);
            else
            {
                this.m_session.Remove(session.Key.Value);
                session = new SessionInfo(principal);
                session.Key = Guid.NewGuid();
                this.m_session.Add(session.Key.Value, session);
                return session;
            }
        }
    }
}
