using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Services;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
    /// Represents a security repository service that uses the direct local services
    /// </summary>
    public class LocalSecurityService : ISecurityRepositoryService
    {

        /// <summary>
        /// Change user's password
        /// </summary>
        public SecurityUser ChangePassword(Guid userId, string password)
        {
            var securityUser = this.GetUser(userId);
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            iids.ChangePassword(securityUser.UserName, password);
            return securityUser;
        }

        /// <summary>
        /// Creates the provided role
        /// </summary>
        public SecurityRole CreateRole(SecurityRole roleInfo)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();
            if (pers == null)
                throw new InvalidOperationException("Misisng role provider service");
            return pers.Insert(roleInfo);
        }

        /// <summary>
        /// Create a user
        /// </summary>
        public SecurityUser CreateUser(SecurityUser userInfo, string password)
        {
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();

            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");
            else if (iids == null)
                throw new InvalidOperationException("Missing identity provider service");

            // Create the identity
            var id = iids.CreateIdentity(userInfo.UserName, password);
            // Now ensure local db record exists
            var retVal = this.GetUser(id);
            if (retVal == null)
                retVal = pers.Insert(userInfo);
            else
            {
                retVal.Email = userInfo.Email;
                retVal.EmailConfirmed = userInfo.EmailConfirmed;
                retVal.InvalidLoginAttempts = userInfo.InvalidLoginAttempts;
                retVal.LastLoginTime = userInfo.LastLoginTime;
				retVal.Lockout = userInfo.Lockout;
                retVal.PhoneNumber = userInfo.PhoneNumber;
                retVal.PhoneNumberConfirmed = userInfo.PhoneNumberConfirmed;
                retVal.SecurityHash = userInfo.SecurityHash;
                retVal.TwoFactorEnabled = userInfo.TwoFactorEnabled;
                retVal.UserPhoto = userInfo.UserPhoto;
                pers.Update(retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Finds the roles matching the specified queried
        /// </summary>
        public IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query)
        {
            int total = 0;
            return this.FindRoles(query, 0, null, out total);
        }

        /// <summary>
        /// Find all roles specified 
        /// </summary>
        public IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query, int offset, int? count, out int total)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();
            if (pers == null)
                throw new InvalidOperationException("Missing role persistence service");
            return pers.Query(query, offset, count, out total);
        }

        /// <summary>
        /// Find users matching the specified query
        /// </summary>
        public IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query)
        {
            int total = 0;
            return this.FindUsers(query, 0, null, out total);
        }

        /// <summary>
        /// Find the specified users
        /// </summary>
        public IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query, int offset, int? count, out int total)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");
            return pers.Query(query, offset, count, out total);
        }

        /// <summary>
        /// Gets the specified role
        /// </summary>
        public SecurityRole GetRole(Guid roleId)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();
            if (pers == null)
                throw new InvalidOperationException("Missing role persistence service");
            return pers.Get(roleId);
        }

        /// <summary>
        /// Get the specified user
        /// </summary>
        public SecurityUser GetUser(Guid userId)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");
            return pers.Get(userId);
        }

        /// <summary>
        /// Get the specified user based on identity
        /// </summary>
        public SecurityUser GetUser(IIdentity identity)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");
            return pers.Query(o=>o.UserName == identity.Name && o.ObsoletionTime == null).FirstOrDefault();
        }

        /// <summary>
        /// Lock the specified user
        /// </summary>
        public void LockUser(Guid userId)
        {
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            if (iids == null)
                throw new InvalidOperationException("Missing identity provider service");

            var securityUser = this.GetUser(userId);
            iids.SetLockout(securityUser.UserName, true);
            
        }

        /// <summary>
        /// Obsoletes the specified role
        /// </summary>
        public SecurityRole ObsoleteRole(Guid roleId)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();
            if (pers == null)
                throw new InvalidOperationException("Missing role provider service");
            return pers.Obsolete(this.GetRole(roleId));

        }

        /// <summary>
        /// Obsolete the specified user
        /// </summary>
        public SecurityUser ObsoleteUser(Guid userId)
        {
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();

            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");
            else if (iids == null)
                throw new InvalidOperationException("Missing identity provider service");

            var retVal = pers.Obsolete(this.GetUser(userId));
            iids.DeleteIdentity(retVal.UserName);
            return retVal;
        }

        /// <summary>
        /// Saves the specified role
        /// </summary>
        public SecurityRole SaveRole(SecurityRole role)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();
            if (pers == null)
                throw new InvalidOperationException("Missing role persistence service");
            return pers.Update(role);
        }

        /// <summary>
        /// Save the specified user
        /// </summary>
        public SecurityUser SaveUser(SecurityUser user)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");

            return pers.Update(user);
        }

        /// <summary>
        /// Unlock the specified user
        /// </summary>
        public void UnlockUser(Guid userId)
        {
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            if (iids == null)
                throw new InvalidOperationException("Missing identity provider service");

            var securityUser = this.GetUser(userId);
            iids.SetLockout(securityUser.UserName, false);
        }
    }
}
