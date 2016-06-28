using System;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenIZ.Mobile.Core.Serices;
using System.Security.Cryptography;
using System.Text;
using OpenIZ.Mobile.Core.Security;
using System.Collections.Generic;
using System.Security.Principal;
using OpenIZ.Core.PCL.Http;

namespace OpenIZ.Mobile.Core.Android.Security
{
	/// <summary>
	/// Represents credentials for this android application on all requests going to the OAuth service
	/// </summary>
	public class OAuthTokenServiceCredentials : Credentials
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Security.OAuthTokenServiceCredentials"/> class.
		/// </summary>
		/// <param name="principal">Principal.</param>
		public OAuthTokenServiceCredentials (IPrincipal principal) : base(principal)
		{
		}

		#region implemented abstract members of Credentials

		/// <summary>
		/// Get the http headers which are requried for the credential
		/// </summary>
		/// <returns>The http headers.</returns>
		public override System.Collections.Generic.Dictionary<string, string> GetHttpHeaders ()
		{
			// App ID credentials
			String appAuthString = String.Format ("{0}:{1}", 
				ApplicationContext.Current.Application.Key.ToString(), 
				ApplicationContext.Current.Application.ApplicationSecret);

			// TODO: Add claims
			List<Claim> claims = new List<Claim> () {
			};

			// Additional claims?
			if (this.Principal is ClaimsPrincipal) {
				claims.AddRange ((this.Principal as ClaimsPrincipal).Claims);
			}

			// Build the claim string
			StringBuilder claimString = new StringBuilder();
			foreach (var c in claims) {
				claimString.AppendFormat ("{0},", 
					Convert.ToBase64String (Encoding.UTF8.GetBytes (String.Format ("{0}={1}", c.Type, c.Value))));
			}
			if(claimString.Length > 0)
				claimString.Remove (claimString.Length - 1, 1);
			
			// Add authenticat header
			var retVal = new System.Collections.Generic.Dictionary<string, string> () {
				{ "Authorization", String.Format("BASIC {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(appAuthString))) }
			};
			if (claimString.Length > 0)
				retVal.Add ("X-OpenIZClient-Claim", claimString.ToString ());
				
			return retVal;
		}
		#endregion

	}
}

