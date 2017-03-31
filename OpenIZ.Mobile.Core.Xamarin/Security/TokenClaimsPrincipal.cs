/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2017-1-16
 */
using System;
using OpenIZ.Mobile.Core.Security;
using System.Security.Cryptography.X509Certificates;
using OpenIZ.Mobile.Core.Configuration;
using System.Security;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;
using OpenIZ.Mobile.Core.Xamarin.Exceptions;

namespace OpenIZ.Mobile.Core.Xamarin.Security
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
			{ "nbf", ClaimTypes.AuthenticationInstant },
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
        /// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Xamarin.Security.TokenClaimsPrincipal"/> class.
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
                tokenObjects[i] = tokenObjects[i].PadRight(tokenObjects[i].Length + (tokenObjects[i].Length % 4), '=').Replace("===","=");
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
				var keyId = headers ["keyid"].Value<Int32>();
                if (keyId > this.m_configuration?.TokenSymmetricSecrets?.Count && !TokenValidationManager.OnSymmetricKeyValidationCallback(this, keyId, body["iss"].Value<String>()))
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

			Claim expiryClaim = claims.Find (o => o.Type == ClaimTypes.Expiration),
				notBeforeClaim = claims.Find (o => o.Type == ClaimTypes.AuthenticationInstant);

            if (expiryClaim == null || notBeforeClaim == null)
                throw new SecurityTokenException(SecurityTokenExceptionType.InvalidClaim, "Missing NBF or EXP claim");
            else
            {
                DateTime expiry = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    notBefore = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                expiry = expiry.AddSeconds(Int32.Parse(expiryClaim.Value)).ToLocalTime();
                notBefore = notBefore.AddSeconds(Int32.Parse(notBeforeClaim.Value)).ToLocalTime();

                if (expiry == null || expiry < DateTime.Now)
                    throw new SecurityTokenException(SecurityTokenExceptionType.TokenExpired, "Token expired");
                else if (notBefore == null || Math.Abs(notBefore.Subtract(DateTime.Now).TotalMinutes) > 3)
                    throw new SecurityTokenException(SecurityTokenExceptionType.NotYetValid, "Token cannot yet be used (issued in the future)");
            }
            this.RefreshToken = refreshToken;

			this.m_identities.Clear ();
			this.m_identities.Add(new ClaimsIdentity(body["unique_name"]?.Value<String>().ToLower() ?? body["sub"]?.Value<String>().ToLower(), true, claims));
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

