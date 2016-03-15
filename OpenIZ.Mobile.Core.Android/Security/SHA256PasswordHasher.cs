using System;
using OpenIZ.Mobile.Core.Serices;
using System.Security.Cryptography;
using System.Text;

namespace OpenIZ.Mobile.Core.Android.Security
{
	/// <summary>
	/// SHA256 password hasher service
	/// </summary>
	public class SHA256PasswordHasher : IPasswordHashingService
	{
		#region IPasswordHashingService implementation
		/// <summary>
		/// Compute hash
		/// </summary>
		/// <returns>The hash.</returns>
		/// <param name="password">Password.</param>
		public string ComputeHash (string password)
		{
			SHA256 hasher = SHA256.Create();
			return BitConverter.ToString(hasher.ComputeHash(Encoding.UTF8.GetBytes(password)));
		}
		#endregion
	}
}

