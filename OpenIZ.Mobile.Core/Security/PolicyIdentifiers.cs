﻿/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
	/// Policy identifiers.
	/// </summary>
	public static class PolicyIdentifiers
	{
		#region IMS Policies in the 1.3.6.1.4.1.33349.3.1.5.9.2 namespace
		/// <summary>
		/// Access administrative function
		/// </summary>
		public const string AccessAdministrativeFunction = "1.3.6.1.4.1.33349.3.1.5.9.2.0";

		/// <summary>
		/// Policy identifier for allowance of changing passwords
		/// </summary>
		/// TODO: Affix the mohawk college OID for this
		public const string ChangePassword = "1.3.6.1.4.1.33349.3.1.5.9.2.0.1";

		/// <summary>
		/// Whether the user can create roles
		/// </summary>
		public const string CreateRoles = "1.3.6.1.4.1.33349.3.1.5.9.2.0.2";

		/// <summary>
		/// Policy identifier for allowance of altering passwords
		/// </summary>
		public const string AlterRoles = "1.3.6.1.4.1.33349.3.1.5.9.2.0.3";

		/// <summary>
		/// Policy identifier for allowing of creating new identities
		/// </summary>
		public const string CreateIdentity = "1.3.6.1.4.1.33349.3.1.5.9.2.0.4";

		/// <summary>
		/// Policy identifier for allowing of creating new devices
		/// </summary>
		public const string CreateDevice = "1.3.6.1.4.1.33349.3.1.5.9.2.0.5";

		/// <summary>
		/// Policy identifier for allowing of creating new applications
		/// </summary>
		public const string CreateApplication = "1.3.6.1.4.1.33349.3.1.5.9.2.0.6";

		/// <summary>
		/// Policy identifier for allowance of login
		/// </summary>
		public const string Login = "1.3.6.1.4.1.33349.3.1.5.9.2.1";

		/// <summary>
		/// Access clinical data permission 
		/// </summary>
		public const string UnrestrictedClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2";

		/// <summary>
		/// Query clinical data
		/// </summary>
		public const string QueryClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2.0";

		/// <summary>
		/// Write clinical data
		/// </summary>
		public const string WriteClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2.1";

		/// <summary>
		/// Delete clinical data
		/// </summary>
		public const string DeleteClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2.2";

		/// <summary>
		/// Read clinical data
		/// </summary>
		public const string ReadClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.2.3";

		/// <summary>
		/// Indicates the user can elevate themselves (Break the glass)
		/// </summary>
		public const string ElevateClinicalData = "1.3.6.1.4.1.33349.3.1.5.9.2.3";	


		#endregion

		#region OpenIZ Client Functions

		/// <summary>
		/// Access administrative function on the OpenIZ Client
		/// </summary>
		public const string AccessClientAdministrativeFunction = "1.3.6.1.4.1.33349.3.1.5.9.2.10";

		#endregion
	}
}

