/*
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
 * Date: 2016-7-23
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services;
using System.Security.Principal;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using System.Globalization;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Model;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Session information
    /// </summary>
    [JsonObject]
    public class SessionInfo : IdentifiedData
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public SessionInfo()
        {
            this.Key = Guid.NewGuid();
        }

        // The tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SessionInfo));

        /// <summary>
        /// Create the session object from the principal
        /// </summary>
        public SessionInfo(IPrincipal principal)
        {
            this.UserName = principal.Identity.Name;
            this.IsAuthenticated = principal.Identity.IsAuthenticated;
            this.AuthenticationType = principal.Identity.AuthenticationType;
            this.Principal = principal;
            if (principal is ClaimsPrincipal)
                this.Token = principal.ToString();

            // Expiry / etc
            if (principal is ClaimsPrincipal)
            {
                var cp = principal as ClaimsPrincipal;

                this.Issued = DateTime.Parse(cp.FindClaim(ClaimTypes.AuthenticationInstant)?.Value ?? DateTime.Now.ToString());
                this.Expiry = DateTime.Parse(cp.FindClaim(ClaimTypes.Expiration)?.Value ?? DateTime.MaxValue.ToString());
                this.Roles = cp.Claims.Where(o => o.Type == ClaimsIdentity.DefaultRoleClaimType)?.Select(o => o.Value)?.ToList();
                this.AuthenticationType = cp.FindClaim(ClaimTypes.AuthenticationMethod)?.Value;

            }
            else
            {
                IRoleProviderService rps = ApplicationContext.Current.GetService<IRoleProviderService>();
                this.Roles = rps.GetAllRoles(this.UserName).ToList();
                this.Issued = DateTime.Now;
                this.Expiry = DateTime.MaxValue;
            }

            // Grab the user entity
            try
            {
                var userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
                var securityUser = userService.GetUser(principal.Identity);
                this.SecurityUser = securityUser;
                this.UserEntity = userService.GetUserEntity(principal.Identity) ??
                    new UserEntity()
                    {
                        SecurityUserKey = securityUser.Key,
                        LanguageCommunication = new List<PersonLanguageCommunication>() { new PersonLanguageCommunication(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, true) },
                        Telecoms = new List<EntityTelecomAddress>()
                        {
                        new EntityTelecomAddress(TelecomAddressUseKeys.Public, securityUser.Email ?? securityUser.PhoneNumber)
                        },
                        Names = new List<EntityName>()
                        {
                        new EntityName() { NameUseKey =  NameUseKeys.OfficialRecord, Component = new List<EntityNameComponent>() { new EntityNameComponent(NameComponentKeys.Given, securityUser.UserName) } }
                        }
                    };
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error getting extended session information: {0}", e);
            }
        }

        private object ApplicationContextIDataPersistenceService<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the principal of the session
        /// </summary>
        [JsonIgnore]
        public IPrincipal Principal { get; private set; }

        /// <summary>
        /// Gets the user entity
        /// </summary>
        [JsonProperty("entity")]
        public UserEntity UserEntity { get; set; }

        /// <summary>
        /// Gets or sets the security user information
        /// </summary>
        [JsonProperty("user")]
        public SecurityUser SecurityUser { get; set; }

        /// <summary>
        /// Gets the user name
        /// </summary>
        [JsonProperty("username")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets the roles to which the identity belongs
        /// </summary>
        [JsonProperty("roles")]
        public List<String> Roles { get; set; }

        /// <summary>
        /// True if authenticated
        /// </summary>
        [JsonProperty("isAuthenticated")]
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the mechanism
        /// </summary>
        [JsonProperty("method")]
        public String AuthenticationType { get; set; }

        /// <summary>
        /// Expiry time
        /// </summary>
        [JsonProperty("exp")]
        public DateTime Expiry { get; set; }

        /// <summary>
        /// Issued time
        /// </summary>
        [JsonProperty("nbf")]
        public DateTime Issued { get; set; }

        /// <summary>
        /// Gets or sets the JWT token
        /// </summary>
        [JsonProperty("jwt")]
        public String Token { get; set; }

        /// <summary>
        /// Gets or sets the refresh token
        /// </summary>
        [JsonProperty("refresh_token")]
        public String RefreshToken { get; set; }

        /// <summary>
        /// Issue date
        /// </summary>
        public override DateTimeOffset ModifiedOn
        {
            get
            {
                return this.Issued;
            }
        }
    }
}