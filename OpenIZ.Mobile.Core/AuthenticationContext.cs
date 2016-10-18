﻿/*
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
 * Date: 2016-6-14
 */
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core
{
    /// <summary>
    /// Authentication context
    /// </summary>
    public sealed class AuthenticationContext
    {

        /// <summary>
        /// SYSTEM user's SID
        /// </summary>
        public const String SystemUserSid = "fadca076-3690-4a6e-af9e-f1cd68e8c7e8";

        /// <summary>
        /// ANONYMOUS user's SID
        /// </summary>
        public const String AnonymousUserSid = "C96859F0-043C-4480-8DAB-F69D6E86696C";

        /// <summary>
        /// System identity
        /// </summary>
        private static readonly IPrincipal s_system = new ClaimsPrincipal(new ClaimsIdentity("SYSTEM", true, new Claim[] {
            new Claim(ClaimTypes.OpenIzGrantedPolicyClaim, PolicyIdentifiers.AccessClientAdministrativeFunction)
        }));
        
        /// <summary>
        /// Anonymous identity
        /// </summary>
        private static readonly IPrincipal s_anonymous = new ClaimsPrincipal(new ClaimsIdentity("ANONYMOUS", true, null));

        /// <summary>
        /// Gets the anonymous principal
        /// </summary>
        public static IPrincipal AnonymousPrincipal
        {
            get
            {
                return s_anonymous;
            }
        }

        /// <summary>
        /// Get the system principal
        /// </summary>
        public static IPrincipal SystemPrincipal
        {
            get
            {
                return s_system;
            }
        }

        /// <summary>
        /// Current context in the request pipeline
        /// </summary>
        [ThreadStatic]
        private static AuthenticationContext s_current;

        /// <summary>
        /// Locking
        /// </summary>
        private static Object s_lockObject = new object();

        /// <summary>
        /// The principal
        /// </summary>
        private IPrincipal m_principal;

        /// <summary>
        /// Creates a new instance of the authentication context
        /// </summary>
        public AuthenticationContext(IPrincipal principal)
        {
            this.m_principal = principal;
        }

        /// <summary>
        /// Creates a new instance of the authentication context from the specified user session
        /// </summary>
        public AuthenticationContext(SessionInfo session)
        {
            if (session.Expiry < DateTime.Now)
                throw new SessionExpiredException();
            this.m_principal = session.Principal;
            this.Session = session;
        }

        /// <summary>
        /// Gets or sets the current context
        /// </summary>
        public static AuthenticationContext Current
        {
            get
            {
                if(s_current == null)
                    lock(s_lockObject)
                        s_current = new AuthenticationContext(s_anonymous);
                return s_current;
            }
            set { s_current = value; }
        }

        /// <summary>
        /// Gets the principal 
        /// </summary>
        public IPrincipal Principal
        {
            get
            {
                return this.m_principal;
            }
        }

        /// <summary>
        /// Represents the user session if applicable
        /// </summary>
        public SessionInfo Session { get; private set; }
    }
}
