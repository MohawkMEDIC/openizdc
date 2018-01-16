/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Data.Model.Security;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Security user persistence
    /// </summary>
    public class SecurityUserPersistenceService : BaseDataPersistenceService<SecurityUser, DbSecurityUser>
    {
        public override SecurityUser ToModelInstance(object dataInstance, LocalDataContext context)
        {
            var dbUser = dataInstance as DbSecurityUser;
            var retVal = base.ToModelInstance(dataInstance, context);
            retVal.Roles = context.Connection.Query<DbSecurityRole>("SELECT security_role.* FROM security_user_role INNER JOIN security_role ON (security_role.uuid = security_user_role.role_id) WHERE security_user_role.user_id = ?", dbUser.Uuid).Select(o => m_mapper.MapDomainInstance<DbSecurityRole, SecurityRole>(o)).ToList();
            foreach (var itm in retVal.Roles)
            {
                var ruuid = itm.Key.Value.ToByteArray();
                itm.Policies = context.Connection.Table<DbSecurityRolePolicy>().Where(o => o.RoleId == ruuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityRolePolicy, SecurityPolicyInstance>(o)).ToList();
            }
            return retVal;
        }
        /// <summary>
        /// Insert the specified object
        /// </summary>
        protected override SecurityUser InsertInternal(LocalDataContext context, SecurityUser data)
        {
            var retVal = base.InsertInternal(context, data);

            // Roles
            if (retVal.Roles != null)
                foreach (var r in retVal.Roles)
                {
                    r.EnsureExists(context);

                    context.Connection.Insert(new DbSecurityUserRole()
                    {
                        Uuid = Guid.NewGuid().ToByteArray(),
                        UserUuid = retVal.Key.Value.ToByteArray(),
                        RoleUuid = r.Key.Value.ToByteArray()
                    });
                }

            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        protected override SecurityUser UpdateInternal(LocalDataContext context, SecurityUser data)
        {
            var retVal = base.UpdateInternal(context, data);
            byte[] keyuuid = retVal.Key.Value.ToByteArray();

            if (retVal.Roles != null)
            {
                var existingRoles = context.Connection.Table<DbSecurityUserRole>().Where(o => o.UserUuid == keyuuid).Select(o=>o.RoleUuid);
                foreach (var r in retVal.Roles.Where(r=>!existingRoles.Any(o=>new Guid(o) == r.Key)))
                {
                    r.EnsureExists(context);
                    context.Connection.Insert(new DbSecurityUserRole()
                    {
                        Uuid = Guid.NewGuid().ToByteArray(),
                        UserUuid = retVal.Key.Value.ToByteArray(),
                        RoleUuid = r.Key.Value.ToByteArray()
                    });
                }
            }

            return retVal;
        }

    }

    /// <summary>
    /// Security user persistence
    /// </summary>
    public class SecurityRolePersistenceService : IdentifiedPersistenceService<SecurityRole, DbSecurityRole>
    {

        /// <summary>
        /// Represent as model instance
        /// </summary>
        public override SecurityRole ToModelInstance(object dataInstance, LocalDataContext context)
        {
            var retVal = base.ToModelInstance(dataInstance, context);
            var dbRole = dataInstance as DbSecurityRole;
            retVal.Policies = context.Connection.Table<DbSecurityRolePolicy>().Where(o => o.RoleId == dbRole.Uuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityRolePolicy, SecurityPolicyInstance>(o)).ToList();

            retVal.Users = context.Connection.Query<DbSecurityUser>("SELECT security_user.* FROM security_user_role INNER JOIN security_user ON (security_user.uuid = security_user_role.user_id) WHERE security_user_role.role_id = ?", dbRole.Uuid).Select(o => m_mapper.MapDomainInstance<DbSecurityUser, SecurityUser>(o)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        protected override SecurityRole InsertInternal(LocalDataContext context, SecurityRole data)
        {
            var retVal = base.InsertInternal(context, data);

            // Roles
            //if (retVal.Policies != null)
            //    base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityRole>(
            //        new List<SecurityPolicyInstance>(),
            //        retVal.Policies,
            //        retVal.Key,
            //        context);

            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        protected override SecurityRole UpdateInternal(LocalDataContext context, SecurityRole data)
        {
            var retVal = base.UpdateInternal(context, data);
            var entityUuid = retVal.Key.Value.ToByteArray();

            // Roles
            //if (retVal.Policies != null)
            //    base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityRole>(
            //        context.Table<DbSecurityRolePolicy>().Where(o => o.RoleId == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityRolePolicy, SecurityPolicyInstance>(o)).ToList(),
            //        retVal.Policies,
            //        retVal.Key,
            //        context);

            return retVal;
        }

    }

    /// <summary>
    /// Security user persistence
    /// </summary>
    public class SecurityDevicePersistenceService : BaseDataPersistenceService<SecurityDevice, DbSecurityDevice>
    {
        /// <summary>
        /// Represent as model instance
        /// </summary>
        public override SecurityDevice ToModelInstance(object dataInstance, LocalDataContext context)
        {
            var retVal = base.ToModelInstance(dataInstance, context);
            var dbDevice = dataInstance as DbSecurityDevice;
            retVal.Policies = context.Connection.Table<DbSecurityDevicePolicy>().Where(o=>o.DeviceId == dbDevice.Uuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityDevicePolicy, SecurityPolicyInstance>(o)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        protected override SecurityDevice InsertInternal(LocalDataContext context, SecurityDevice data)
        {
            var retVal = base.InsertInternal(context, data);

            // Roles
            //if (retVal.Policies != null)
            //    base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityDevice>(
            //        new List<SecurityPolicyInstance>(),
            //        retVal.Policies,
            //        retVal.Key,
            //        context);


            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        protected override SecurityDevice UpdateInternal(LocalDataContext context, SecurityDevice data)
        {
            var retVal = base.UpdateInternal(context, data);
            var entityUuid = retVal.Key.Value.ToByteArray();

            // Roles
            //if (retVal.Policies != null)
            //    base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityDevice>(
            //        context.Table<DbSecurityDevicePolicy>().Where(o => o.DeviceId == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityDevicePolicy, SecurityPolicyInstance>(o)).ToList(),
            //        retVal.Policies,
            //        retVal.Key,
            //        context);


            return retVal;
        }

    }

    /// <summary>
    /// Security user persistence
    /// </summary>
    public class SecurityApplicationPersistenceService : BaseDataPersistenceService<SecurityApplication, DbSecurityApplication>
    {
        /// <summary>
        /// Represent as model instance
        /// </summary>
        public override SecurityApplication ToModelInstance(object dataInstance, LocalDataContext context)
        {
            var retVal = base.ToModelInstance(dataInstance, context);
            var dbApplication = dataInstance as DbSecurityApplication;
            retVal.Policies = context.Connection.Table<DbSecurityApplicationPolicy>().Where(o => o.ApplicationId == dbApplication.Uuid).Select(o => m_mapper.MapDomainInstance<DbSecurityApplicationPolicy, SecurityPolicyInstance>(o)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        protected override SecurityApplication InsertInternal(LocalDataContext context, SecurityApplication data)
        {
            var retVal = base.InsertInternal(context, data);

            // Roles
            //if (retVal.Policies != null)
            //    base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityApplication>(
            //        new List<SecurityPolicyInstance>(),
            //        retVal.Policies,
            //        retVal.Key,
            //        context);


            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        protected override SecurityApplication UpdateInternal(LocalDataContext context, SecurityApplication data)
        {
            var retVal = base.UpdateInternal(context, data);

            var entityUuid = retVal.Key.Value.ToByteArray();
            // Roles
            //if (retVal.Policies != null)
            //    base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityApplication>(
            //        context.Table<DbSecurityApplicationPolicy>().Where(o => o.ApplicationId == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityApplicationPolicy, SecurityPolicyInstance>(o)).ToList(),
            //        retVal.Policies,
            //        retVal.Key,
            //        context);


            return retVal;
        }

    }
}
