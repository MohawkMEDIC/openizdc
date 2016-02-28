using System;
using System.Security;
using OpenIZ.Mobile.Core.Security;

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
		public PolicyViolationException(string policyId, PolicyDecisionOutcomeType outcome)
		{
			this.PolicyId = policyId;
			this.PolicyDecision = outcome;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Exceptions.PolicyViolationException"/> class.
		/// </summary>
		/// <param name="policy">Policy.</param>
		/// <param name="outcome">Outcome.</param>
		public PolicyViolationException(IPolicy policy, PolicyDecisionOutcomeType outcome)
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
		public PolicyDecisionOutcomeType PolicyDecision { get; private set; }
		/// <summary>
		/// Gets the policy identifier.
		/// </summary>
		/// <value>The policy identifier.</value>
		public string PolicyId { get; private set; }
	}
}

