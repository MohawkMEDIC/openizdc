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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Attributes
{
    /// <summary>
    /// Represents a demand for a particular permission to perform the action
    /// </summary>
    public class DemandAttribute : System.Attribute
    {

        /// <summary>
        /// Demand attribute
        /// </summary>
        public DemandAttribute(String policyId)
        {
            this.PolicyId = policyId;
        }

        /// <summary>
        /// Policy identifier for demand
        /// </summary>
        public String PolicyId { get; set; }

    }
}
