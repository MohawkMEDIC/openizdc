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
 * Date: 2016-7-23
 */
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Android.Security
{
    /// <summary>
    /// System principal is used in the Android Security assembly only 
    /// </summary>
    internal class SystemPrincipal : GenericPrincipal
    {

        public SystemPrincipal() : base(new SystemIdentity(), null)
        {

        }
    }

    /// <summary>
    /// Identity representing SYSTEM user
    /// </summary>
    internal class SystemIdentity : GenericIdentity
    {

        /// <summary>
        /// System identity
        /// </summary>
        public SystemIdentity() : base("SYSTEM")
        {

        }

        /// <summary>
        /// SYSTEM is always SYSTEM
        /// </summary>
        public override string Name
        {
            get
            {
                return "SYSTEM";
            }
        }

        /// <summary>
        /// SYSTEM is always authenticated
        /// </summary>
        public override bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }
    }
}