using System;

namespace OpenIZ.Mobile.Core.Security
{
	/// <summary>
	/// Represents a simple policy
	/// </summary>
	public interface IPolicy
	{
		bool CanOverride { get; }
		bool IsActive { get; }
		string Name { get; }
		string Oid { get; }
	}
}

