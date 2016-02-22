using System;
using OpenIZ.Mobile.Core.Authentication;

namespace OpenIZ.Mobile.Core.Http
{
	/// <summary>
	/// Fired when there are invalid certificate is encountered
	/// </summary>
	public interface ICertificateValidator
	{
		/// <summary>
		/// Determines if the remote certificate is valid
		/// </summary>
		/// <returns><c>true</c>, if certificate was validated, <c>false</c> otherwise.</returns>
		/// <param name="certificate">Certificate.</param>
		/// <param name="chain">Chain.</param>
		bool ValidateCertificate(Object certificate, Object chain);
	}

}

