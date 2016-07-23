using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Data.Model.Security;
using SQLite;
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
        public override SecurityUser ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var dbUser = dataInstance as DbSecurityUser;
            var retVal = base.ToModelInstance(dataInstance, context);
            retVal.Roles = context.Query<DbSecurityRole>("SELECT security_role.* FROM security_user_role INNER JOIN security_role ON (security_role.uuid = security_user_role.role_id) WHERE security_user_role.user_id = ?", dbUser.Uuid).Select(o => m_mapper.MapDomainInstance<DbSecurityRole, SecurityRole>(o)).ToList();
            foreach (var itm in retVal.Roles)
            {
                var ruuid = itm.Key.Value.ToByteArray();
                itm.Policies = context.Table<DbSecurityRolePolicy>().Where(o => o.RoleId == ruuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityRolePolicy, SecurityPolicyInstance>(o, null)).ToList();
            }
            return retVal;
        }
        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override SecurityUser Insert(SQLiteConnection context, SecurityUser data)
        {
            var retVal = base.Insert(context, data);

            // Roles
            if (retVal.Roles != null)
                foreach (var r in retVal.Roles)
                {
                    r.EnsureExists(context);

                    context.Insert(new DbSecurityUserRole()
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
        public override SecurityUser Update(SQLiteConnection context, SecurityUser data)
        {
            var retVal = base.Update(context, data);
            byte[] keyuuid = retVal.Key.Value.ToByteArray();

            if (retVal.Roles != null)
            {
                context.Table<DbSecurityUserRole>().Delete(o => o.UserUuid == keyuuid);
                foreach (var r in retVal.Roles)
                {
                    r.EnsureExists(context);
                    context.Insert(new DbSecurityUserRole()
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
        public override SecurityRole ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var retVal = base.ToModelInstance(dataInstance, context);
            var dbRole = dataInstance as DbSecurityRole;
            retVal.Policies = context.Table<DbSecurityRolePolicy>().Where(o => o.RoleId == dbRole.Uuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityRolePolicy, SecurityPolicyInstance>(o, null)).ToList();
            retVal.Users = context.Query<DbSecurityUser>("SELECT security_user.* FROM security_user_role INNER JOIN security_user ON (security_user.uuid = security_user_role.user_id) WHERE security_user_role.role_id = ?", dbRole.Uuid).Select(o => m_mapper.MapDomainInstance<DbSecurityUser, SecurityUser>(o)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override SecurityRole Insert(SQLiteConnection context, SecurityRole data)
        {
            var retVal = base.Insert(context, data);

            // Roles
            if (retVal.Policies != null)
                base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityRole>(
                    new List<SecurityPolicyInstance>(),
                    retVal.Policies,
                    retVal.Key,
                    context);

            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        public override SecurityRole Update(SQLiteConnection context, SecurityRole data)
        {
            var retVal = base.Update(context, data);
            var entityUuid = retVal.Key.Value.ToByteArray();

            // Roles
            if (retVal.Policies != null)
                base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityRole>(
                    context.Table<DbSecurityRolePolicy>().Where(o => o.RoleId == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityRolePolicy, SecurityPolicyInstance>(o)).ToList(),
                    retVal.Policies,
                    retVal.Key,
                    context);

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
        public override SecurityDevice ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var retVal = base.ToModelInstance(dataInstance, context);
            var dbDevice = dataInstance as DbSecurityDevice;
            retVal.Policies = context.Table<DbSecurityDevicePolicy>().Where(o=>o.DeviceId == dbDevice.Uuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityDevicePolicy, SecurityPolicyInstance>(o, null)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override SecurityDevice Insert(SQLiteConnection context, SecurityDevice data)
        {
            var retVal = base.Insert(context, data);

            // Roles
            if (retVal.Policies != null)
                base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityDevice>(
                    new List<SecurityPolicyInstance>(),
                    retVal.Policies,
                    retVal.Key,
                    context);


            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        public override SecurityDevice Update(SQLiteConnection context, SecurityDevice data)
        {
            var retVal = base.Update(context, data);
            var entityUuid = retVal.Key.Value.ToByteArray();

            // Roles
            if (retVal.Policies != null)
                base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityDevice>(
                    context.Table<DbSecurityDevicePolicy>().Where(o => o.DeviceId == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityDevicePolicy, SecurityPolicyInstance>(o)).ToList(),
                    retVal.Policies,
                    retVal.Key,
                    context);


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
        public override SecurityApplication ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var retVal = base.ToModelInstance(dataInstance, context);
            var dbApplication = dataInstance as DbSecurityApplication;
            retVal.Policies = context.Table<DbSecurityApplicationPolicy>().Where(o => o.ApplicationId == dbApplication.Uuid).Select(o => m_mapper.MapDomainInstance<DbSecurityApplicationPolicy, SecurityPolicyInstance>(o, null)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override SecurityApplication Insert(SQLiteConnection context, SecurityApplication data)
        {
            var retVal = base.Insert(context, data);

            // Roles
            if (retVal.Policies != null)
                base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityApplication>(
                    new List<SecurityPolicyInstance>(),
                    retVal.Policies,
                    retVal.Key,
                    context);


            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        public override SecurityApplication Update(SQLiteConnection context, SecurityApplication data)
        {
            var retVal = base.Update(context, data);

            var entityUuid = retVal.Key.Value.ToByteArray();
            // Roles
            if (retVal.Policies != null)
                base.UpdateAssociatedItems<SecurityPolicyInstance, SecurityApplication>(
                    context.Table<DbSecurityApplicationPolicy>().Where(o => o.ApplicationId == entityUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbSecurityApplicationPolicy, SecurityPolicyInstance>(o)).ToList(),
                    retVal.Policies,
                    retVal.Key,
                    context);


            return retVal;
        }

    }
}
