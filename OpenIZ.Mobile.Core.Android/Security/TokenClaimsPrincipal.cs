using System;
using OpenIZ.Mobile.Core.Security;
using System.Security.Cryptography.X509Certificates;
using OpenIZ.Mobile.Core.Configuration;
using System.Security;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;
using OpenIZ.Mobile.Core.Android.Exceptions;

namespace OpenIZ.Mobile.Core.Android.Security
{
	/// <summary>
	/// Token claims principal.
	/// </summary>
	public class TokenClaimsPrincipal : ClaimsPrincipal
	{

		// Claim map
		private readonly Dictionary<String, String> claimMap = new Dictionary<string, string>() {
			{ "unique_name", ClaimsIdentity.DefaultNameClaimType },
			{ "role", ClaimsIdentity.DefaultRoleClaimType },
			{ "sub", ClaimTypes.Sid },
			{ "authmethod", ClaimTypes.AuthenticationMethod },
			{ "exp", ClaimTypes.Expiration },
			{ "auth_time", ClaimTypes.AuthenticationInstant },
			{ "email", ClaimTypes.Email }
		};

		// The token
		private String m_token;

		// Configuration
		private SecurityConfigurationSection m_configuration = ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>();

        /// <summary>
        /// Gets the refresh token
        /// </summary>
        public String RefreshToken { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Security.TokenClaimsPrincipal"/> class.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="tokenType">Token type.</param>
        public TokenClaimsPrincipal (String token, String tokenType, String refreshToken) : base(null)
		{
			if (String.IsNullOrEmpty (token))
				throw new ArgumentNullException (nameof (token));
			else if (String.IsNullOrEmpty (tokenType))
				throw new ArgumentNullException (nameof (tokenType));
			else if (tokenType != "urn:ietf:params:oauth:token-type:jwt")
				throw new ArgumentOutOfRangeException (nameof (tokenType), "expected urn:ietf:params:oauth:token-type:jwt");

			// Token
			this.m_token = token;

			String[] tokenObjects = token.Split ('.');
            // Correct each token to be proper B64 encoding
            for (int i = 0; i < tokenObjects.Length; i++)
                tokenObjects[i] = tokenObjects[i].PadRight(tokenObjects[i].Length + (tokenObjects[i].Length % 2), '=');
			JObject headers = JObject.Parse (Encoding.UTF8.GetString (Convert.FromBase64String (tokenObjects [0]))),
				body = JObject.Parse (Encoding.UTF8.GetString (Convert.FromBase64String (tokenObjects [1])));

			// Algorithm is valid?
			if (this.m_configuration.TokenAlgorithms?.Contains ((String)headers ["alg"]) == false)
				throw new SecurityTokenException(SecurityTokenExceptionType.InvalidTokenType, String.Format ("Token algorithm {0} not permitted", headers ["alg"]));


			// Attempt to get the certificate
			if (((String)headers ["alg"]).StartsWith ("RS")) {
				var cert = X509CertificateUtils.FindCertificate (X509FindType.FindByThumbprint, StoreLocation.CurrentUser, StoreName.My, headers ["x5t"].ToString ());
				//if (cert == null)
				//	throw new SecurityTokenException(SecurityTokenExceptionType.KeyNotFound, String.Format ("Cannot find certificate {0}", headers ["x5t"]));
				// TODO: Verify signature
			} else if (((String)headers ["alg"]).StartsWith ("HS")) {
				int keyId = Int32.Parse ((String)headers ["keyid"]);
				if (keyId > this.m_configuration.TokenSymmetricSecrets.Count)
					throw new SecurityTokenException (SecurityTokenExceptionType.KeyNotFound, "Symmetric key not found");
				// TODO: Verfiy signature
			} 
				

			
			// Parse the jwt
			List<Claim> claims = new List<Claim>();

			foreach (var kf in body) {
				String claimName = kf.Key;
				if (!claimMap.TryGetValue (kf.Key, out claimName))
					claims.AddRange (this.ProcessClaim (kf, kf.Key));
				else
					claims.AddRange (this.ProcessClaim (kf, claimName));
			}

			Claim expiry = claims.Find (o => o.Type == ClaimTypes.Expiration),
				notBefore = claims.Find (o => o.Type == ClaimTypes.AuthenticationInstant);
			if (expiry == null || DateTime.Parse (expiry.Value) < DateTime.Now)
				throw new SecurityTokenException (SecurityTokenExceptionType.TokenExpired, "Token expired");
			else if (notBefore == null || Math.Abs(DateTime.Parse (notBefore.Value).Subtract(DateTime.Now).TotalMinutes) > 2)
				throw new SecurityTokenException (SecurityTokenExceptionType.NotYetValid, "Token cannot yet be used");

            this.RefreshToken = refreshToken;

			this.m_identities.Clear ();
			this.m_identities.Add(new ClaimsIdentity((String)body["unique_name"], true, claims));
		}


		/// <summary>
		/// Processes the claim.
		/// </summary>
		/// <returns>The claim.</returns>
		/// <param name="jwtClaim">Jwt claim.</param>
		public IEnumerable<Claim> ProcessClaim(KeyValuePair<String, JToken> jwtClaim, String claimType)
		{
			List<Claim> retVal = new List<Claim> ();
			if(jwtClaim.Value is JArray)
				foreach(var val in jwtClaim.Value as JArray)
					retVal.Add(new Claim(claimType, (String)val));
			else
				retVal.Add(new Claim(claimType, jwtClaim.Value.ToString()));
			return retVal;
		}

		/// <summary>
		/// Represent the token claims principal as a string (the JWT token itself)
		/// </summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		public override string ToString ()
		{
			return this.m_token;
		}
	}
}

