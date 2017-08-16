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
 * Date: 2017-2-4
 */
using OpenIZ.Core.Interfaces;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Data.Model.Security;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Resources;
using OpenIZ.Mobile.Core.Serices;
using OpenIZ.Mobile.Core.Services;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Local identity service.
    /// </summary>
    public class LocalIdentityService : IIdentityProviderService, ISecurityAuditEventSource
    {
        // Configuration
        private DataConfigurationSection m_configuration = ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>();

        // Local tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalIdentityService));

        #region IIdentityProviderService implementation

        /// <summary>
        /// Occurs when authenticated.
        /// </summary>
        public event EventHandler<AuthenticatedEventArgs> Authenticated;

        /// <summary>
        /// Fired on authenticating
        /// </summary>
        public event EventHandler<AuthenticatingEventArgs> Authenticating;
        public event EventHandler<SecurityAuditDataEventArgs> SecurityAttributesChanged;
        public event EventHandler<AuditDataEventArgs> DataCreated;
        public event EventHandler<AuditDataEventArgs> DataUpdated;
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;
        public event EventHandler<SecurityAuditDataEventArgs> SecurityResourceCreated;
        public event EventHandler<SecurityAuditDataEventArgs> SecurityResourceDeleted;

        /// <summary>
        /// Authenticate the user
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        public System.Security.Principal.IPrincipal Authenticate(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            if (String.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            return this.Authenticate(new SQLitePrincipal(new SQLiteIdentity(userName, false), null), password);
        }

        /// <summary>
        /// Authenticate the user
        /// </summary>
        /// <param name="principal">Principal.</param>
        /// <param name="password">Password.</param>
        public IPrincipal Authenticate(IPrincipal principal, String password)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (String.IsNullOrEmpty(password))
            {
                if (principal.Identity.IsAuthenticated)
                {
                    // Refresh
                    if (principal is SQLitePrincipal) /// extend the existing session 
                        (principal as SQLitePrincipal).Expires = DateTime.Now.Add(ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>()?.MaxLocalSession ?? new TimeSpan(0, 15, 0));
                    else if (principal is ClaimsPrincipal) // switch them to a SQLitePrincipal
                    {
                        var sid = (principal as ClaimsPrincipal).FindClaim(ClaimTypes.Sid)?.Value;
                        var uname = (principal as ClaimsPrincipal).FindClaim(ClaimsIdentity.DefaultNameClaimType)?.Value;
                        if (!String.IsNullOrEmpty(uname))
                        {
                            ApplicationContext.Current.GetService<ITickleService>()?.SendTickle(new Tickler.Tickle(Guid.Parse(sid), Tickler.TickleType.SecurityInformation | Tickler.TickleType.Toast, Strings.locale_securitySwitchedMode, DateTime.Now.AddSeconds(10)));
                            return new SQLitePrincipal(new SQLiteIdentity(uname, true), (principal as ClaimsPrincipal).Claims.Where(o => o.Type == ClaimsIdentity.DefaultRoleClaimType).Select(o => o.Value).ToArray())
                            {
                                Expires = DateTime.Now.Add(ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>()?.MaxLocalSession ?? new TimeSpan(0, 15, 0)),
                                IssueTime = DateTime.Now
                            };
                        }
                        else
                            throw new SecurityException(Strings.locale_sessionError);
                    }
                    return principal;
                }
                else
                    throw new ArgumentNullException(nameof(password));
            }

            // Pre-event
            AuthenticatingEventArgs e = new AuthenticatingEventArgs(principal.Identity.Name, password) { Principal = principal };
            this.Authenticating?.Invoke(this, e);
            if (e.Cancel)
            {
                this.m_tracer.TraceWarning("Pre-Event hook indicates cancel {0}", principal.Identity.Name);
                return e.Principal;
            }

            IPrincipal retVal = null;
            try
            {
                // Connect to the db
                var connection = this.CreateConnection();
                using (connection.Lock())
                {
                    // Password service
                    IPasswordHashingService passwordHash = ApplicationContext.Current.GetService(typeof(IPasswordHashingService)) as IPasswordHashingService;

                    DbSecurityUser dbs = connection.Table<DbSecurityUser>().FirstOrDefault(o => o.UserName == principal.Identity.Name);
                    if (dbs == null)
                        throw new SecurityException(Strings.locale_invalidUserNamePassword);
                    else if (dbs.Lockout.HasValue && dbs.Lockout > DateTime.Now)
                        throw new SecurityException(Strings.locale_accountLocked);
                    else if (dbs.ObsoletionTime != null)
                        throw new SecurityException(Strings.locale_accountObsolete);
                    else if (passwordHash.ComputeHash(password) != dbs.PasswordHash)
                    {
                        dbs.InvalidLoginAttempts++;
                        connection.Update(dbs);
                        throw new SecurityException(Strings.locale_invalidUserNamePassword);
                    }
                    else if (dbs.InvalidLoginAttempts > 20)
                    { //s TODO: Make this configurable
                        dbs.Lockout = DateTime.Now.AddSeconds(30 * (dbs.InvalidLoginAttempts - 10));
                        connection.Update(dbs);
                        throw new SecurityException(Strings.locale_accountLocked);
                    } // TODO: Lacks login permission
                    else
                    {
                        dbs.LastLoginTime = DateTime.Now;
                        dbs.InvalidLoginAttempts = 0;
                        connection.Update(dbs);

                        // Create the principal
                        retVal = new SQLitePrincipal(new SQLiteIdentity(dbs.UserName, true),
                            connection.Query<DbSecurityRole>("SELECT security_role.* FROM security_user_role INNER JOIN security_role ON (security_role.uuid = security_user_role.role_id) WHERE security_user_role.user_id = ?",
                            dbs.Uuid).Select(o => o.Name).ToArray())
                        {
                            IssueTime = DateTime.Now,
                            Expires = DateTime.Now.Add(ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>()?.MaxLocalSession ?? new TimeSpan(0, 15, 0))
                        };

                    }
                }

                // Post-event
                this.Authenticated?.Invoke(e, new AuthenticatedEventArgs(principal.Identity.Name, password, true) { Principal = retVal });

            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error establishing session: {0}", ex);
                this.Authenticated?.Invoke(e, new AuthenticatedEventArgs(principal.Identity.Name, password, false) { Principal = retVal });

                throw;
            }

            return retVal;
        }

        /// <summary>
        /// Authenticate the user using a TwoFactorAuthentication secret
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="tfaSecret">Tfa secret.</param>
        public System.Security.Principal.IPrincipal Authenticate(string userName, string password, string tfaSecret)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change the user's password
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="newPassword">New password.</param>
        /// <param name="principal">Principal.</param>
        public void ChangePassword(string userName, string password, System.Security.Principal.IPrincipal principal)
        {
            // We must demand the change password permission
            try
            {
                IPolicyDecisionService pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();

                if (userName != principal.Identity.Name &&
                    pdp.GetPolicyOutcome(principal, PolicyIdentifiers.ChangePassword) == OpenIZ.Core.Model.Security.PolicyGrantType.Deny)
                    throw new SecurityException("User cannot change specified users password");
                var conn = this.CreateConnection();
                using (conn.Lock())
                {
                    var dbu = conn.Table<DbSecurityUser>().Where(o => o.UserName == userName).FirstOrDefault();
                    if (dbu == null)
                        throw new KeyNotFoundException();
                    else
                    {
                        IPasswordHashingService hash = ApplicationContext.Current.GetService<IPasswordHashingService>();
                        dbu.PasswordHash = hash.ComputeHash(password);
                        dbu.SecurityHash = Guid.NewGuid().ToString();
                        dbu.UpdatedByUuid = conn.Table<DbSecurityUser>().First(u => u.UserName == principal.Identity.Name).Uuid;
                        dbu.UpdatedTime = DateTime.Now;
                        conn.Update(dbu);
                        this.SecurityAttributesChanged?.Invoke(this, new SecurityAuditDataEventArgs(dbu, "password"));
                    }
                }
            }
            catch (Exception e)
            {

                this.SecurityAttributesChanged?.Invoke(this, new SecurityAuditDataEventArgs(new SecurityUser() { Key = Guid.Empty, UserName = userName }, "password") { Success = false });

                this.m_tracer.TraceError("Error changing password for user {0} : {1}", userName, e);
                throw;
            }
        }

        /// <summary>
        /// Change the user's password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void ChangePassword(string userName, string password)
        {
            this.ChangePassword(userName, password, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Create specified identity
        /// </summary>
        public IIdentity CreateIdentity(String userName, String password)
        {
            return this.CreateIdentity(Guid.NewGuid(), userName, password, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Create specified identity
        /// </summary>
        public IIdentity CreateIdentity(Guid sid, String userName, String password)
        {
            return this.CreateIdentity(sid, userName, password, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Creates an identity for the user
        /// </summary>
        public IIdentity CreateIdentity(Guid sid, string userName, string password, IPrincipal principal)
        {
            return this.CreateIdentity(new SecurityUser
            {
                Key = sid,
                UserName = userName
            }, password, principal);
        }

        /// <summary>
        /// Creates the identity.
        /// </summary>
        /// <param name="securityUser">The security user.</param>
        /// <param name="password">The password.</param>
        /// <param name="principal">The principal.</param>
        /// <returns>Returns the created user identity.</returns>
        /// <exception cref="PolicyViolationException"></exception>
        public IIdentity CreateIdentity(SecurityUser securityUser, string password, IPrincipal principal)
        {
            try
            {
                var pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();
                if (pdp.GetPolicyOutcome(principal ?? AuthenticationContext.Current.Principal, PolicyIdentifiers.AccessClientAdministrativeFunction) != PolicyGrantType.Grant)
                    throw new PolicyViolationException(PolicyIdentifiers.AccessClientAdministrativeFunction, PolicyGrantType.Deny);

                var conn = this.CreateConnection();
                IPasswordHashingService hash = ApplicationContext.Current.GetService<IPasswordHashingService>();

                using (conn.Lock())
                {
                    DbSecurityUser dbu = new DbSecurityUser()
                    {
                        PasswordHash = hash.ComputeHash(password),
                        SecurityHash = Guid.NewGuid().ToString(),
                        PhoneNumber = securityUser.PhoneNumber,
                        Email = securityUser.Email,
                        CreationTime = DateTime.Now,
                        CreatedByUuid = conn.Table<DbSecurityUser>().FirstOrDefault(o => o.UserName == AuthenticationContext.Current?.Principal?.Identity?.Name)?.Uuid ?? Guid.Parse("fadca076-3690-4a6e-af9e-f1cd68e8c7e8").ToByteArray(),
                        UserName = securityUser.UserName,
                        Key = securityUser.Key.Value
                    };
                    conn.Insert(dbu);
                    this.DataCreated?.Invoke(this, new AuditDataEventArgs(dbu));
                }
                return new SQLiteIdentity(securityUser.UserName, false);
            }
            catch
            {
                this.DataCreated?.Invoke(this, new AuditDataEventArgs(new SecurityUser()) { Success = false });
                throw;
            }
        }


        public void DeleteIdentity(string userName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an un-authenticated identity
        /// </summary>
        /// <returns>The identity.</returns>
        /// <param name="userName">User name.</param>
        public System.Security.Principal.IIdentity GetIdentity(string userName)
        {
            try
            {
                var conn = this.CreateConnection();
                using (conn.Lock())
                {
                    var userData = conn.Table<DbSecurityUser>().FirstOrDefault(o => o.UserName == userName);
                    if (userData == null)
                        return null;
                    else
                        return new SQLiteIdentity(userName, false);
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error getting identity {0}", e);
                throw;
            }
        }

        public void SetLockout(string userName, bool v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a connection to the local database
        /// </summary>
        /// <returns>The connection.</returns>
        private LockableSQLiteConnection CreateConnection()
        {
            return SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current.Configuration.GetConnectionString(this.m_configuration.MainDataSourceConnectionStringName).Value);
        }

        #endregion IIdentityProviderService implementation
    }

    /// <summary>
    /// SQLite identity
    /// </summary>
    public class SQLiteIdentity : IIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Security.SQLiteIdentity"/> class.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="authenticated">If set to <c>true</c> authenticated.</param>
        public SQLiteIdentity(String userName, bool authenticated)
        {
            this.Name = userName;
            this.IsAuthenticated = authenticated;
            if (authenticated)
                this.AuthenticationType = "LOCAL";
        }

        #region IIdentity implementation

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <returns>The type of authentication used to identify the user.</returns>
        /// <value>The type of the authentication.</value>
        public string AuthenticationType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>true if the user was authenticated; otherwise, false.</returns>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        public bool IsAuthenticated
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <returns>The name of the user on whose behalf the code is running.</returns>
        /// <value>The name.</value>
        public string Name
        {
            get;
            private set;
        }

        #endregion IIdentity implementation
    }

    /// <summary>
    /// SQLite principal.
    /// </summary>
    public class SQLitePrincipal : IPrincipal
    {
        private String[] m_roles;

        /// <summary>
        /// The time that the principal was issued
        /// </summary>
        public DateTime IssueTime { get; set; }

        /// <summary>
        /// Expiration time
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Security.SQLitePrincipal"/> class.
        /// </summary>
        public SQLitePrincipal(SQLiteIdentity identity, String[] roles)
        {
            this.m_roles = roles;
            this.Identity = identity;
        }

        #region IPrincipal implementation

        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <value>The identity.</value>
        public IIdentity Identity
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <returns>true if the current principal is a member of the specified role; otherwise, false.</returns>
        /// <param name="role">The name of the role for which to check membership.</param>
        public bool IsInRole(string role)
        {
            return this.m_roles.Contains(role);
        }

        #endregion IPrincipal implementation
    }
}