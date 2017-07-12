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
using OpenIZ.Mobile.Core.Services;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Local role provider service
    /// </summary>
    public class LocalRoleProviderService : IRoleProviderService, ISecurityAuditEventSource
    {
        // Configuration
        private DataConfigurationSection m_configuration = ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>();

        // Local tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalIdentityService));

        public event EventHandler<AuditDataEventArgs> DataCreated;
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        public event EventHandler<AuditDataEventArgs> DataUpdated;
        public event EventHandler<SecurityAuditDataEventArgs> SecurityAttributesChanged;

        /// <summary>
        /// Add specified roles to the specified groups
        /// </summary>
        public void AddPoliciesToRoles(IPolicyInstance[] policyInstance, string[] roles, IPrincipal principal = null)
        {
            // Demand local admin
            try
            {
                var pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();
                if (pdp.GetPolicyOutcome(principal ?? AuthenticationContext.Current.Principal, PolicyIdentifiers.AccessClientAdministrativeFunction) != PolicyGrantType.Grant)
                    throw new PolicyViolationException(PolicyIdentifiers.AccessClientAdministrativeFunction, PolicyGrantType.Deny);

                var conn = this.CreateConnection();
                using (conn.Lock())
                {
                    foreach (var rn in roles)
                    {
                        try
                        {
                            conn.BeginTransaction();
                            var dbr = conn.Table<DbSecurityRole>().FirstOrDefault(o => o.Name == rn);
                            if (dbr == null)
                                throw new KeyNotFoundException(String.Format("Role {0} not found", rn));
                            var currentPolicies = conn.Query<DbSecurityPolicy>("select security_policy.* from security_policy inner join security_role_policy on (security_policy.uuid = security_role_policy.policy_id) where security_role_policy.role_id = ?", dbr.Uuid).ToList();
                            var toBeInserted = policyInstance.Where(o => !currentPolicies.Any(p => p.Oid == o.Policy.Oid)).Select(
                                o => new DbSecurityRolePolicy()
                                {
                                    GrantType = (int)o.Rule,
                                    PolicyId = conn.Table<DbSecurityPolicy>().Where(p => p.Oid == o.Policy.Oid).First().Uuid,
                                    RoleId = dbr.Uuid,
                                    Key = Guid.NewGuid()
                                }).ToList();
                            foreach (var itm in toBeInserted)
                                conn.Insert(itm);

                            this.SecurityAttributesChanged?.Invoke(this, new SecurityAuditDataEventArgs(dbr, $"policy=[{String.Join(",", policyInstance.Select(o => $"{o.Policy.Name}:{o.Rule}"))}]"));
                            conn.Commit();
                        }
                        catch (Exception e)
                        {
                            this.m_tracer.TraceError("Error assigning policies to role {0}: {1}", rn, e);
                            conn.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                this.SecurityAttributesChanged?.Invoke(this, new SecurityAuditDataEventArgs(roles.Select(o => new SecurityRole() { Name = o }), $"policy=[{String.Join(",", policyInstance.Select(o => $"{o.Policy.Name}:{o.Rule}"))}]") { Success = false });
            }
        }

        /// <summary>
        /// Add the specified users to the specified roles
        /// </summary>
        public void AddUsersToRoles(string[] userNames, string[] roleNames, IPrincipal principal = null)
        {
            try
            {
                if (userNames == null)
                    throw new ArgumentNullException(nameof(userNames));
                if (roleNames == null)
                    throw new ArgumentNullException(nameof(roleNames));

                // Demand local admin
                var pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();

                if (pdp.GetPolicyOutcome(principal ?? AuthenticationContext.Current.Principal, PolicyIdentifiers.AccessClientAdministrativeFunction) != PolicyGrantType.Grant)
                {
                    throw new PolicyViolationException(PolicyIdentifiers.AccessClientAdministrativeFunction, PolicyGrantType.Deny);
                }

                var conn = this.CreateConnection();
                using (conn.Lock())
                {
                    foreach (var userName in userNames)
                    {
                        var un = userName.ToLower();
                        var dbu = conn.Table<DbSecurityUser>().FirstOrDefault(o => o.UserName.ToLower() == un);
                        if (dbu == null)
                            throw new KeyNotFoundException(String.Format("User {0} not found", un));
                        foreach (var rn in roleNames)
                        {
                            var dbr = conn.Table<DbSecurityRole>().FirstOrDefault(o => o.Name.ToLower() == rn.ToLower());
                            if (dbr == null)
                                throw new KeyNotFoundException(String.Format("Role {0} not found", rn));
                            else if (conn.Table<DbSecurityUserRole>().Where(o => o.RoleUuid == dbr.Uuid && o.UserUuid == dbu.Uuid).Count() == 0)
                                conn.Insert(new DbSecurityUserRole()
                                {
                                    RoleUuid = dbr.Uuid,
                                    UserUuid = dbu.Uuid,
                                    Key = Guid.NewGuid()
                                });
                        }

                        this.SecurityAttributesChanged?.Invoke(this, new SecurityAuditDataEventArgs(dbu, $"role={String.Join(",", roleNames)}"));

                    }
                }
            }
            catch
            {

                this.SecurityAttributesChanged?.Invoke(this, new SecurityAuditDataEventArgs(userNames.Select(o => new SecurityUser() { UserName = o }), $"roles=[{String.Join(",", roleNames.Select(o => $"{o}"))}]") { Success = false });
                
                throw;
            }
        }

        /// <summary>
        /// Create the specified role
        /// </summary>
        public void CreateRole(string value, IPrincipal principal = null)
        {
            try
            {
                // Demand local admin
                var pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();
                if (pdp.GetPolicyOutcome(principal ?? AuthenticationContext.Current.Principal, PolicyIdentifiers.AccessClientAdministrativeFunction) != PolicyGrantType.Grant)
                    throw new PolicyViolationException(PolicyIdentifiers.AccessClientAdministrativeFunction, PolicyGrantType.Deny);

                var conn = this.CreateConnection();
                using (conn.Lock())
                {
                    try
                    {
                        var pk = Guid.NewGuid();
                        conn.Insert(new DbSecurityRole() { Name = value, Key = pk });
                        this.DataCreated?.Invoke(this, new AuditDataEventArgs(new SecurityRole() { Key = pk, Name = value }));
                    }
                    catch(Exception e)
                    {
						this.m_tracer.TraceError($"Unable to create role: {e}");
                    }
                }
            }
            catch
            {
                this.DataCreated?.Invoke(this, new AuditDataEventArgs(new SecurityRole() { Key = Guid.Empty, Name = value }) { Success = false });
                throw;
            }
        }

        /// <summary>
        /// Finds users in the specified role
        /// </summary>
        public string[] FindUsersInRole(string role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            var conn = this.CreateConnection();
            using (conn.Lock())
            {
                return conn.Query<DbSecurityUser>("SELECT security_user.* FROM security_user_role INNER JOIN security_user ON (security_user.uuid = security_user_role.user_Id) INNER JOIN security_role ON (security_user_role.role_Id = security_role.uuid) WHERE security_role.name = ?", role)
                            .Select(o => o.UserName)
                            .ToArray();
            }
        }

        /// <summary>
        /// Gets all roles
        /// </summary>
        public string[] GetAllRoles()
        {
            var conn = this.CreateConnection();
            using (conn.Lock())
            {
                return conn.Table<DbSecurityRole>().ToList().Select(o => o.Name).ToArray();
            }
        }

        /// <summary>
        /// Get all roles for the specified user
        /// </summary>
        public string[] GetAllRoles(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            var conn = this.CreateReadonlyConnection();
            using (conn.Lock())
            {
                return conn.Query<DbSecurityRole>("SELECT security_role.* FROM security_user_role INNER JOIN security_role ON (security_role.uuid = security_user_role.role_id) INNER JOIN security_user ON (security_user.uuid = security_user_role.user_id) WHERE security_user.username = ?", userName)
                    .Select(p => p.Name)
                    .ToArray();
            }
        }

        /// <summary>
        /// Determine if the user in the role
        /// </summary>
        public bool IsUserInRole(string userName, string roleName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determine if the principle in the role
        /// </summary>
        public bool IsUserInRole(IPrincipal principal, string roleName)
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

        /// <summary>
        /// Creates a connection to the local database
        /// </summary>
        /// <returns>The connection.</returns>
        private LockableSQLiteConnection CreateReadonlyConnection()
        {
            return SQLiteConnectionManager.Current.GetReadonlyConnection(ApplicationContext.Current.Configuration.GetConnectionString(this.m_configuration.MainDataSourceConnectionStringName).Value);
        }
    }
}