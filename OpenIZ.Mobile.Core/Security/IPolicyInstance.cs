using System;

namespace OpenIZ.Mobile.Core.Security
{
	/// <summary>
	/// Represents a policy instance class
	/// </summary>
	public interface IPolicyInstance
	{
		IPolicy Policy { get; }
		PolicyDecisionOutcomeType Rule { get; }
		object Securable { get; }
	}
}

