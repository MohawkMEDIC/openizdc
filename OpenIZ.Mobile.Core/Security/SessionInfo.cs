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
using OpenIZ.Core.Model.Attributes;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Threading;
using OpenIZ.Mobile.Core.Configuration;
using System.Security;
using OpenIZ.Mobile.Core.Resources;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Session information
    /// </summary>
    [JsonObject("SessionInfo"), XmlType("SessionInfo", Namespace = "http://openiz.org/model")]
    public class SessionInfo : IdentifiedData
    {

        // The entity 
        private UserEntity m_entity;

        // Lock
        private object m_syncLock = new object();

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
            this.ProcessPrincipal(principal);
        }

        private object ApplicationContextIDataPersistenceService<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the principal of the session
        /// </summary>
        [JsonIgnore, DataIgnore]
        public IPrincipal Principal { get; private set; }

        /// <summary>
        /// Clear the set cache
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ClearCached() { this.m_entity = null; }

        /// <summary>
        /// Gets the user entity
        /// </summary>
        [JsonProperty("entity")]
        public UserEntity UserEntity
        {
            get
            {
                if (this.m_entity != null || this.Principal == null)
                    return this.m_entity;

                // HACK: Find a better way
                var userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

                UserEntity entity = null;
                try
                {
                    entity = userService.GetUserEntity(this.Principal.Identity);

                    if (entity == null && this.SecurityUser != null)
                        entity = new UserEntity()
                        {
                            SecurityUserKey = this.SecurityUser.Key,
                            LanguageCommunication = new List<PersonLanguageCommunication>() { new PersonLanguageCommunication(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, true) },
                            Telecoms = new List<EntityTelecomAddress>()
                            {
                                                    new EntityTelecomAddress(TelecomAddressUseKeys.Public, this.SecurityUser.Email ?? this.SecurityUser.PhoneNumber)
                            },
                            Names = new List<EntityName>()
                            {
                                                    new EntityName() { NameUseKey =  NameUseKeys.OfficialRecord, Component = new List<EntityNameComponent>() { new EntityNameComponent(NameComponentKeys.Given, this.SecurityUser.UserName) } }
                            }
                        };
                    else
                        this.m_entity = entity;
                    return entity;
                }
                catch { return null; }

            }
        }

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

        /// <summary>
        /// Extends the session
        /// </summary>
        public bool Extend()
        {
            try
            {
                lock (this.m_syncLock)
                {
                    if (this.Expiry > DateTime.Now.AddMinutes(5)) // session will still be valid in 5 mins so no auth
                        return true;
                    this.ProcessPrincipal(ApplicationContext.Current.GetService<IIdentityProviderService>().Authenticate(this.Principal, null));
                    return this.Principal != null;
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error extending session: {0}", e);
                return false;
            }
        }

        /// <summary>
        /// Process a principal
        /// </summary>
        /// <param name="principal"></param>
        private void ProcessPrincipal(IPrincipal principal)
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

                this.Issued = (cp.FindClaim(ClaimTypes.AuthenticationInstant)?.AsDateTime().ToLocalTime() ?? DateTime.Now);
                this.Expiry = (cp.FindClaim(ClaimTypes.Expiration)?.AsDateTime().ToLocalTime() ?? DateTime.MaxValue);
                this.Roles = cp.Claims.Where(o => o.Type == ClaimsIdentity.DefaultRoleClaimType)?.Select(o => o.Value)?.ToList();
                this.AuthenticationType = cp.FindClaim(ClaimTypes.AuthenticationMethod)?.Value;

                var subKey = Guid.Empty;
                if (cp.HasClaim(o => o.Type == ClaimTypes.Sid))
                    Guid.TryParse(cp.FindClaim(ClaimTypes.Sid)?.Value, out subKey);

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

                // User entity available?
                this.m_entity = userService.GetUserEntity(principal.Identity);
                
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error getting extended session information: {0}", e);
            }

            // Only subscribed faciliites
            if (ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().OnlySubscribedFacilities)
            {
                var subFacl = ApplicationContext.Current.Configuration.GetSection<SynchronizationConfigurationSection>().Facilities;
                var isInSubFacility = this.m_entity?.Relationships.Any(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation && subFacl.Contains(o.TargetEntityKey.ToString())) == true;
                if (!isInSubFacility && ApplicationContext.Current.PolicyDecisionService.GetPolicyOutcome(principal, PolicyIdentifiers.AccessClientAdministrativeFunction) != PolicyGrantType.Grant)
                    throw new SecurityException(Strings.locale_loginFromUnsubscribedFacility);
            }

        }
    }
}