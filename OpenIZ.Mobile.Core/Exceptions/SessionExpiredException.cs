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
 * Date: 2016-10-17
 */
using OpenIZ.Mobile.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Exceptions
{
    /// <summary>
    /// Session expired exception
    /// </summary>
    public class SessionExpiredException : SecurityException
    {
        /// <summary>
        /// Session has expired
        /// </summary>
        public SessionExpiredException() : base(Strings.locale_session_expired)
        {

        }
    }
}
