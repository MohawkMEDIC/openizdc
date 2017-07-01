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
 * Date: 2017-3-31
 */
using System;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Xamarin.Security;
using System.Security.Permissions;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Configuration;
using System.Security.Principal;
using OpenIZ.Core.Http;

namespace OpenIZ.Mobile.Core.Xamarin.Security
{
	/// <summary>
	/// Represents a Credential which is a token credential
	/// </summary>
	public class TokenCredentials : Credentials
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Xamarin.Security.TokenCredentials"/> class.
		/// </summary>
		/// <param name="principal">Principal.</param>
		public TokenCredentials (IPrincipal principal) : base (principal)
		{
			
		}

		#region implemented abstract members of Credentials
		/// <summary>
		/// Get HTTP header
		/// </summary>
		/// <returns>The http headers.</returns>
		public override System.Collections.Generic.Dictionary<string, string> GetHttpHeaders ()
		{
			if (this.Principal is TokenClaimsPrincipal)
				return new System.Collections.Generic.Dictionary<string, string> () {
					{ "Authorization", String.Format ("Bearer {0}", this.Principal.ToString ()) }
				};
			else
				throw new InvalidOperationException ("Cannot create a token credential from non-token principal");
		}
		#endregion
	}


}

