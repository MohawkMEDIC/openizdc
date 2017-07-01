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
 * Date: 2016-6-14
 */
using System;
using System.Security;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Mobile.Core.Exceptions
{
	/// <summary>
	/// Represents a policy violation
	/// </summary>
	public class PolicyViolationException : SecurityException
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Exceptions.PolicyViolationException"/> class.
		/// </summary>
		/// <param name="policyId">Policy identifier.</param>
		/// <param name="outcome">Outcome.</param>
		public PolicyViolationException(string policyId, PolicyGrantType outcome)
		{
			this.PolicyId = policyId;
			this.PolicyDecision = outcome;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Exceptions.PolicyViolationException"/> class.
		/// </summary>
		/// <param name="policy">Policy.</param>
		/// <param name="outcome">Outcome.</param>
		public PolicyViolationException(IPolicy policy, PolicyGrantType outcome)
		{
			this.Policy = policy;
			this.PolicyId = policy.Oid;
			this.PolicyDecision = outcome;
		}

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>The error message that explains the reason for the exception, or an empty string ("").</returns>
		/// <filterpriority>1</filterpriority>
		/// <value>The message.</value>
		public override string Message { 
			get { 
				return String.Format ("Policy '{0}' was violated with outcome '{1}'", this.PolicyId, this.PolicyDecision);
			}
		}

		/// <summary>
		/// Gets the policy that was violated
		/// </summary>
		/// <value>The policy.</value>
		public IPolicy Policy { get; private set; }
		/// <summary>
		/// Gets the policy decision.
		/// </summary>
		/// <value>The policy decision.</value>
		public PolicyGrantType PolicyDecision { get; private set; }
		/// <summary>
		/// Gets the policy identifier.
		/// </summary>
		/// <value>The policy identifier.</value>
		public string PolicyId { get; private set; }
	}
}

