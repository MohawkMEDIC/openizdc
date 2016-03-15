using System;

namespace OpenIZ.Mobile.Core.Serices
{
	/// <summary>
	/// Password hashing service.
	/// </summary>
	public interface IPasswordHashingService
	{

		/// <summary>
		/// Compute the password hash
		/// </summary>
		/// <returns>The hash.</returns>
		/// <param name="password">Password.</param>
		String ComputeHash(String password);
	}
}

