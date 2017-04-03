﻿/*
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
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Represents a policy instance 
    /// </summary>
    public class GenericPolicyInstance : IPolicyInstance
    {
        /// <summary>
        /// Constructs a new instance of the policy instance
        /// </summary>
        public GenericPolicyInstance(IPolicy policy, PolicyGrantType rule)
        {
            this.Policy = policy;
            this.Rule = rule;
        }

        /// <summary>
        /// Gets the policy to which the instance applies
        /// </summary>
        public IPolicy Policy
        {
            get; private set;
        }

        /// <summary>
        /// Gets the rule or, rather, the enforcement type
        /// </summary>
        public PolicyGrantType Rule
        {
            get;private set;
        }

        /// <summary>
        /// Gets or sets the policy securable
        /// </summary>
        public object Securable
        {
            get;private set;
        }
    }
}
