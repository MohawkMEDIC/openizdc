﻿/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */
using OpenIZ.Mobile.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Represents a session manager service
    /// </summary>
    public interface ISessionManagerService
    {

        /// <summary>
        /// Authenticates the specified username/password pair
        /// </summary>
        SessionInfo Authenticate(String userName, String password);

        /// <summary>
        /// Authenticates the specified username/password/tfasecret pair
        /// </summary>
        SessionInfo Authenticate(String userName, String password, String tfaSecret);

        /// <summary>
        /// Refreshes the specified session
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        SessionInfo Refresh(SessionInfo session, String password);
       
        /// <summary>
        /// Deletes (abandons) the session
        /// </summary>
        SessionInfo Delete(Guid sessionId);

        /// <summary>
        /// Gets the session
        /// </summary>
        SessionInfo Get(Guid sessionId);

    }
}
