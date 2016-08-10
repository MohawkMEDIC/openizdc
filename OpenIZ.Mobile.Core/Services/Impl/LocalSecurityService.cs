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
 * Date: 2016-7-8
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Synchronization;

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

            // Create an admin queue entry that will change the password
            SynchronizationQueue.Admin.Enqueue(new SecurityUser() { Key = userId, PasswordHash = password }, Synchronization.Model.DataOperationType.Update);

            return securityUser;
        }

        public SecurityDevice CreateDevice(SecurityDevice device)
        {
            throw new NotImplementedException();
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

            // Communicate the retVal to the AMI
            var commAdmin = retVal.Clone() as SecurityUser;
            commAdmin.PasswordHash = password;
            SynchronizationQueue.Admin.Enqueue(commAdmin, Synchronization.Model.DataOperationType.Insert);

            // Return value
            return retVal;
        }

        /// <summary>
        /// Creates the specified user entity
        /// </summary>
        public UserEntity CreateUserEntity(UserEntity userEntity)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service missing");
            return persistence.Insert(userEntity);
        }


        public IEnumerable<SecurityDevice> FindDevices(Expression<Func<SecurityDevice, bool>> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SecurityDevice> FindDevices(Expression<Func<SecurityDevice, bool>> query, int offset, int? count, out int totalResults)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find policies that match the specified data
        /// </summary>
        public IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find policies that match the specified data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> filter, int offset, int? count, out int totalResults)
        {
            throw new NotImplementedException();
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
        /// Find the specified user entity data
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerable<UserEntity> FindUserEntity(Expression<Func<UserEntity, bool>> expression)
        {
            int td;
            return this.FindUserEntity(expression, 0, null, out td);
        }

        /// <summary>
        /// Find user entity
        /// </summary>
        public IEnumerable<UserEntity> FindUserEntity(Expression<Func<UserEntity, bool>> expression, int offset, int? count, out int totalCount)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service missing");
            return persistence.Query(expression, offset, count, out totalCount);
        }

        /// <summary>
        /// Find the specified user entity with constraints
        /// </summary>
        public IEnumerable<UserEntity> FindUserEntity(Expression<Func<UserEntity, bool>> expression, int offset, int count, out int totalCount)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service missing");
            return persistence.Query(expression, offset, offset, out totalCount);
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

        public SecurityDevice GetDevice(Guid deviceId)
        {
            throw new NotImplementedException();
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
        /// Gets the specified user entity
        /// </summary>
        public UserEntity GetUserEntity(Guid id, Guid versionId)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service missing");
            return persistence.Get(id);
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

        public SecurityDevice ObsoleteDevice(Guid deviceId)
        {
            throw new NotImplementedException();
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
        /// Obsoletes the specified user entity
        /// </summary>
        public UserEntity ObsoleteUserEntity(Guid id)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service not found");
            return persistence.Obsolete(new UserEntity() { Key = id });
        }

        public SecurityDevice SaveDevice(SecurityDevice device)
        {
            throw new NotImplementedException();
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
            // Save user
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");
            
            var retVal = pers.Update(user);

            // Notify admin queue
            var commUser = retVal.Clone() as SecurityUser;
            commUser.PasswordHash = null; // Don't set password
            SynchronizationQueue.Admin.Enqueue(commUser, Synchronization.Model.DataOperationType.Update);

            return retVal;
        }

        /// <summary>
        /// Saves the specified user entity
        /// </summary>
        public UserEntity SaveUserEntity(UserEntity userEntity)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service not found");
            try
            {
                return persistence.Update(userEntity);
            }
            catch
            {
                return persistence.Insert(userEntity);

            }
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
