using OpenIZ.Core.Model.Security;
using System;

namespace OpenIZ.Mobile.Core.Security
{
	/// <summary>
	/// Represents a policy instance class
	/// </summary>
	public interface IPolicyInstance
	{
		IPolicy Policy { get; }
		PolicyGrantType Rule { get; }
		object Securable { get; }
	}
}

