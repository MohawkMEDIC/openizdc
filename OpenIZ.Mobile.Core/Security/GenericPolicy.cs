/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Represents a simple policy implemtnation
    /// </summary>
    public class GenericPolicy : IPolicy
    {
        /// <summary>
        /// Constructs a simple policy 
        /// </summary>
        public GenericPolicy(String oid, String name, bool canOverride)
        {
            this.Oid = oid;
            this.Name = name;
            this.CanOverride = canOverride;
            this.IsActive = true;
        }

        /// <summary>
        /// True if the policy can be overridden
        /// </summary>
        public bool CanOverride
        {
            get; private set;
        }

        /// <summary>
        /// Returns true if the policy is active
        /// </summary>
        public bool IsActive
        {
            get; private set;
        }

        /// <summary>
        /// Gets the name of the policy
        /// </summary>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets the oid of the policy
        /// </summary>
        public string Oid
        {
            get; private set;
        }
    }
}
