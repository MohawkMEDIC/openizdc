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
 * User: fyfej
 * Date: 2016-10-25
 */
using System;
using OpenIZ.Mobile.Core.Security;
using System.Security.Principal;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Mobile.Core.Services
{
	/// <summary>
	/// Represents a policy decision service
	/// </summary>
	public interface IPolicyDecisionService
	{

		/// <summary>
		/// Get a policy decision outcome (i.e. make a policy decision)
		/// </summary>
		PolicyGrantType GetPolicyOutcome(IPrincipal principal, string policyId);

        /// <summary>
        /// Get a policy decision
        /// </summary>
        PolicyGrantType GetPolicyDecision(IPrincipal principal, Object securable);
	}
}

