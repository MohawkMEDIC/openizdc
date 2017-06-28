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
 * User: justi
 * Date: 2016-6-14
 */
using System;

namespace OpenIZ.Mobile.Core.Security
{
	/// <summary>
	/// Claim types
	/// </summary>
	public static class ClaimTypes
	{

		/// <summary>
		/// The open iz scope claim.
		/// </summary>
		public const string OpenIzScopeClaim = "http://openiz.org/claims/scope";

		/// <summary>
		/// Granted policy claim
		/// </summary>
		public const string OpenIzGrantedPolicyClaim = "http://openiz.org/claims/grant";

		/// <summary>
		/// Device identifier claim
		/// </summary>
		public const string OpenIzDeviceIdentifierClaim = "http://openiz.org/claims/device-id";

		/// <summary>
		/// Identifier of the application
		/// </summary>
		public const string OpenIzApplicationIdentifierClaim = "http://openiz.org/claims/application-id";

		/// <summary>
		/// Patient identifier claim
		/// </summary>
		public const string XUAPatientIdentifierClaim = "urn:oasis:names:tc:xacml:2.0:resource:resource-id";
		/// <summary>
		/// Purpose of use claim
		/// </summary>
		public const string XspaPurposeOfUseClaim = "urn:oasis:names:tc:xacml:2.0:action:purpose";
		/// <summary>
		/// Purpose of use claim
		/// </summary>
		public const string XspaUserRoleClaim = "urn:oasis:names:tc:xacml:2.0:subject:role";
		/// <summary>
		/// Facility id claim
		/// </summary>
		public const string XspaFacilityUrlClaim = "urn:oasis:names:tc:xspa:1.0:subject:facility";
		/// <summary>
		/// Organization name claim
		/// </summary>
		public const string XspaOrganizationNameClaim = "urn:oasis:names:tc:xspa:1.0:subject:organization-id";
		/// <summary>
		/// User identifier claim
		/// </summary>
		public const string XspaUserIdentifierClaim = "urn:oasis:names:tc:xacml:1.0:subject:subject-id";

		/// <summary>
		/// Authentication type
		/// </summary>
		public const string AuthenticationType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication";
		/// <summary>
		/// The authentication instant claim.
		/// </summary>
		public const string AuthenticationInstant = "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant";
		/// <summary>
		/// The authentication method claim.
		/// </summary>
		public const string AuthenticationMethod = "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod";
		/// <summary>
		/// The expiration claim.
		/// </summary>
		public const string Expiration = "http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration";
		/// <summary>
		/// The security identifier claim.
		/// </summary>
		public const string Sid = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid";
		/// <summary>
		/// Email address claim
		/// </summary>
		public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        /// <summary>
        /// Telephone address claim
        /// </summary>
        public const string Telephone = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone";
    }
}

