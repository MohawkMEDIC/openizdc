using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using SQLite;
using OpenIZ.Mobile.Core.Data.Model.Security;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Core.Diagnostics;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Local role provider service
    /// </summary>
    public class LocalRoleProviderService : IRoleProviderService
    {
        // Local tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalIdentityService));

        // Configuration
        private DataConfigurationSection m_configuration = ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>();

        /// <summary>
        /// Creates a connection to the local database
        /// </summary>
        /// <returns>The connection.</returns>
        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(ApplicationContext.Current.Configuration.GetConnectionString(this.m_configuration.MainDataSourceConnectionStringName).Value);
        }

        /// <summary>
        /// Finds users in the specified role
        /// </summary>
        public string[] FindUsersInRole(string role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            using (var conn = this.CreateConnection())
                return conn.Table<DbSecurityUserRole>().Join<DbSecurityRole, byte[], DbSecurityRole>(
                            conn.Table<DbSecurityRole>(),
                            r => r.RoleUuid,
                            r => r.Uuid,
                            (a, b) => b)
                            .Select(o => o.Name).ToArray();
        }

        /// <summary>
        /// Gets all roles
        /// </summary>
        public string[] GetAllRoles()
        {
            using (var conn = this.CreateConnection())
                return conn.Table<DbSecurityRole>().Select(o=>o.Name).ToArray();
        }

        /// <summary>
        /// Get all roles for the specified user
        /// </summary>
        public string[] GetAllRoles(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            using (var conn = this.CreateConnection())
                return conn.Table<DbSecurityUserRole>().Join(
                    conn.Table<DbSecurityUser>(),
                    ru => ru.UserUuid,
                    u => u.Uuid,
                    (a, b) => a)
                .Join(
                    conn.Table<DbSecurityRole>(),
                    ru => ru.RoleUuid,
                    r => r.Uuid,
                    (a, b) => b)
                .Select(p => p.Name)
                .ToArray();
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
        /// Add the specified users to the specified roles
        /// </summary>
        public void AddUsersToRoles(string[] userNames, string[] roleNames)
        {
            if (userNames == null)
                throw new ArgumentNullException(nameof(userNames));
            if (roleNames == null)
                throw new ArgumentNullException(nameof(roleNames));

            using(var conn = this.CreateConnection())
                foreach(var un in userNames)
                {
                    var dbu = conn.Table<DbSecurityUser>().FirstOrDefault(o => o.UserName == un);
                    if (dbu == null)
                        this.m_tracer.TraceWarning("User {0} not found skipping", un);
                    foreach(var rn in roleNames)
                    {
                        var dbr = conn.Table<DbSecurityRole>().FirstOrDefault(o => o.Name == rn);
                        if (dbr == null)
                            this.m_tracer.TraceWarning("Role {0} not found skipping", rn);
                        else if (conn.Table<DbSecurityUserRole>().Where(o => o.RoleUuid == dbr.Uuid && o.UserUuid == dbu.Uuid).Count() == 0)
                            conn.Insert(new DbSecurityUserRole()
                            {
                                RoleUuid = dbr.Uuid,
                                UserUuid = dbu.Uuid
                            });
                    }
                }
        }
    }
}
