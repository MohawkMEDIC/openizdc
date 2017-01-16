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
 * Date: 2016-10-14
 */
using OpenIZ.Mobile.Core.Resources;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
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
        /// Authentication with the user and establish a session
        /// </summary>
        public SessionInfo Authenticate(string userName, string password)
        {
            return this.Authenticate(userName, password, null);
        }

        /// <summary>
        /// Authenticate the user and establish a sessions
        /// </summary>
        public SessionInfo Authenticate(string userName, string password, string tfaSecret)
        {
            var idp = ApplicationContext.Current.GetService<IIdentityProviderService>();
            IPrincipal principal = null;
            if (String.IsNullOrEmpty(tfaSecret))
                principal = idp.Authenticate(userName, password);
            else
                principal = idp.Authenticate(userName, password, tfaSecret);

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
