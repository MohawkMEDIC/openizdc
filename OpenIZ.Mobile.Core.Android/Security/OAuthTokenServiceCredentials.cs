using System;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenIZ.Mobile.Core.Serices;
using System.Security.Cryptography;
using System.Text;
using OpenIZ.Mobile.Core.Security;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Android.Security
{
	/// <summary>
	/// Represents credentials for this android application
	/// </summary>
	public class AndroidApplicationCredentials : OpenIZ.Mobile.Core.Authentication.Credentials
	{

		// Application Secret
		byte[] s_secret = new byte[]{
			0xFF, 0x00, 0x43, 0x23, 0x55, 0x98, 0xA0, 0x20,
			0xC3, 0xE3, 0xE2, 0xA1, 0x42, 0x92, 0x81, 0xE3
		};

		#region implemented abstract members of Credentials

		/// <summary>
		/// Get the http headers which are requried for the credential
		/// </summary>
		/// <returns>The http headers.</returns>
		public override System.Collections.Generic.Dictionary<string, string> GetHttpHeaders ()
		{
			// Get password hash
			SHA256 hasher = SHA256.Create();

			// App ID credentials
			String appId = typeof(AndroidApplicationCredentials).Assembly.GetCustomAttribute<GuidAttribute> (),
			devId = String.Empty;
			String appAuthString = String.Format ("{0}:{1}", appId, BitConverter.ToString (hasher.ComputeHash (s_secret)));

			// TODO: Add claims
			List<Claim> claims = new List<Claim> () {
				new Claim(ClaimTypes.OpenIzApplicationIdentifierClaim, appId),
				new Claim(ClaimTypes.OpenIzDeviceIdentifierClaim, devId)
			};

			// Add authenticat header
			return new System.Collections.Generic.Dictionary<string, string> () {
				{ "Authorization", String.Format("BASIC {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(appAuthString))) }
			};
		}
		#endregion

	}
}

