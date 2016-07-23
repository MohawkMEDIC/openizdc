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
using System.Linq;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Diagnostics;
using SQLite;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Data.Model.Security;
using System.Security;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Serices;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Security
{

    /// <summary>
    /// SQLite principal.
    /// </summary>
    public class SQLitePrincipal : IPrincipal
    {
        private String[] m_roles;

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
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <returns>true if the current principal is a member of the specified role; otherwise, false.</returns>
        /// <param name="role">The name of the role for which to check membership.</param>
        public bool IsInRole(string role)
        {
            return this.m_roles.Contains(role);
        }
        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <value>The identity.</value>
        public IIdentity Identity
        {
            get;
            private set;
        }
        #endregion
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

        #endregion

    }

    /// <summary>
    /// Local identity service.
    /// </summary>
    public class LocalIdentityService : IIdentityProviderService
    {

        // Local tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalIdentityService));

        // Configuration
        private DataConfigurationSection m_configuration = ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>();

        #region IIdentityProviderService implementation

        /// <summary>
        /// Fired on authenticating
        /// </summary>
        public event EventHandler<AuthenticatingEventArgs> Authenticating;

        /// <summary>
        /// Occurs when authenticated.
        /// </summary>
        public event EventHandler<AuthenticatedEventArgs> Authenticated;

        /// <summary>
        /// Creates a connection to the local database
        /// </summary>
        /// <returns>The connection.</returns>
        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(ApplicationContext.Current.Configuration.GetConnectionString(this.m_configuration.MainDataSourceConnectionStringName).Value);
        }


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
                throw new ArgumentNullException(nameof(password));

            // Pre-event
            AuthenticatingEventArgs e = new AuthenticatingEventArgs(principal.Identity.Name, password) { Principal = principal };
            this.Authenticating?.Invoke(this, e);
            if (e.Cancel)
            {
                this.m_tracer.TraceWarning("Pre-Event hook indicates cancel {0}", principal.Identity.Name);
                return e.Principal;
            }

            IPrincipal retVal = null;

            // Connect to the db
            using (SQLiteConnection connection = this.CreateConnection())
            {

                // Password service
                IPasswordHashingService passwordHash = ApplicationContext.Current.GetService(typeof(IPasswordHashingService)) as IPasswordHashingService;

                DbSecurityUser dbs = connection.Table<DbSecurityUser>().FirstOrDefault(o => o.UserName == principal.Identity.Name);
                if (dbs == null)
                    throw new SecurityException("Invalid username/password");
                else if (dbs.Lockout.HasValue && dbs.Lockout > DateTime.Now)
                    throw new SecurityException("Account is currently locked");
                else if (dbs.ObsoletionTime != null)
                    throw new SecurityException("Account is obsolete");
                else if (passwordHash.ComputeHash(password) != dbs.PasswordHash)
                {
                    dbs.InvalidLoginAttempts++;
                    connection.Update(dbs);
                }
                else if (dbs.InvalidLoginAttempts > 3)
                { //s TODO: Make this configurable
                    dbs.Lockout = DateTime.Now.AddSeconds(30 * (dbs.InvalidLoginAttempts - 3));
                    connection.Update(dbs);
                    throw new SecurityException("Account is currently locked");
                } // TODO: Lacks login permission
                else
                {
                    dbs.LastLoginTime = DateTime.Now;
                    dbs.InvalidLoginAttempts = 0;
                    connection.Update(dbs);

                    // Create the principal
                    retVal = new SQLitePrincipal(new SQLiteIdentity(dbs.UserName, true),
                        connection.Query<DbSecurityRole>("SELECT security_role.* FROM security_user_role INNER JOIN security_role ON (security_role.uuid = security_user_role.role_id) WHERE security_user_role.user_id = ?", 
                        dbs.Uuid).Select(o => o.Name).ToArray());
                }
            }

            // Post-event
            this.Authenticated?.Invoke(e, new AuthenticatedEventArgs(principal.Identity.Name, password) { Principal = retVal });
            return retVal;
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
                using (SQLiteConnection conn = this.CreateConnection())
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
                using (SQLiteConnection conn = this.CreateConnection())
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
                    }
                }
            }
            catch (Exception e)
            {
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
            this.ChangePassword(userName, password, ApplicationContext.Current.Principal);
        }

        /// <summary>
        /// Creates an identity for the user
        /// </summary>
        public IIdentity CreateIdentity(string userName, string password)
        {
            using (SQLiteConnection conn = this.CreateConnection())
            {
                IPasswordHashingService hash = ApplicationContext.Current.GetService<IPasswordHashingService>();
                DbSecurityUser dbu = new DbSecurityUser()
                {
                    PasswordHash = hash.ComputeHash(password),
                    SecurityHash = Guid.NewGuid().ToString(),
                    CreationTime = DateTime.Now,
                    CreatedByUuid = conn.Table<DbSecurityUser>().FirstOrDefault(o => o.UserName == ApplicationContext.Current?.Principal?.Identity?.Name)?.Uuid ?? Guid.Parse("fadca076-3690-4a6e-af9e-f1cd68e8c7e8").ToByteArray(),
                    UserName = userName,
                    Key = Guid.NewGuid()
                };
                conn.Insert(dbu);
                return new SQLiteIdentity(userName, false);
            }
        }

        public void SetLockout(string userName, bool v)
        {
            throw new NotImplementedException();
        }

        public void DeleteIdentity(string userName)
        {
            throw new NotImplementedException();
        }

        #endregion




    }
}

