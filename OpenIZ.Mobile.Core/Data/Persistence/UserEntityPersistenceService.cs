﻿using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persister that can read/write user entities
    /// </summary>
    public class UserEntityPersistenceService : IdentifiedPersistenceService<UserEntity, DbUserEntity>
    {

        // Entity persisters
        private PersonPersistenceService m_personPersister = new PersonPersistenceService();
        protected EntityPersistenceService m_entityPersister = new EntityPersistenceService();

      
        /// <summary>
        /// To model instance
        /// </summary>
        public override UserEntity ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var userEntity = dataInstance as DbUserEntity;
            var dbe = context.Table<DbEntity>().Where(o => o.Uuid == userEntity.Uuid).First();
            var dbp = context.Table<DbPerson>().Where(o => o.Uuid == userEntity.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<UserEntity>(dbe, context);
            retVal.SecurityUserKey = new Guid(userEntity.SecurityUserUuid);
            retVal.DateOfBirth = dbp.DateOfBirth;
            // Reverse lookup
            if (!String.IsNullOrEmpty(dbp.DateOfBirthPrecision))
                retVal.DateOfBirthPrecision = PersonPersistenceService.PrecisionMap.Where(o => o.Value == dbp.DateOfBirthPrecision).Select(o => o.Key).First();

            return retVal;
        }
        
        /// <summary>
        /// Inserts the user entity
        /// </summary>
        public override UserEntity Insert(SQLiteConnection context, UserEntity data)
        {
            data.SecurityUser?.EnsureExists(context);
            data.SecurityUserKey = data.SecurityUser?.Key ?? data.SecurityUserKey;
            var inserted = this.m_personPersister.Insert(context, data);

            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified user entity
        /// </summary>
        public override UserEntity Update(SQLiteConnection context, UserEntity data)
        {
            data.SecurityUser?.EnsureExists(context);
            data.SecurityUserKey = data.SecurityUser?.Key ?? data.SecurityUserKey;
            this.m_personPersister.Update(context, data);
            return base.Update(context, data);
        }

        /// <summary>
        /// Obsolete the specified user instance
        /// </summary>
        public override UserEntity Obsolete(SQLiteConnection context, UserEntity data)
        {
            var retVal = this.m_personPersister.Obsolete(context, data);
            return data;
        }
    }
}