using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using SQLite;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Person persistence service
    /// </summary>
    /// <remarks>This is a little different than the other persisters as we have to 
    /// persist half the object in one set of tables ane the other fields in this
    /// table</remarks>
    public class PersonPersistenceService : IdentifiedPersistenceService<Person, DbPerson>
    {
        // Map
        public static readonly Dictionary<DatePrecision, String> PrecisionMap = new Dictionary<DatePrecision, String>()
        {
            { DatePrecision.Day, "d" },
            { DatePrecision.Hour, "H" },
            { DatePrecision.Minute, "M" },
            { DatePrecision.Month, "m" },
            { DatePrecision.Second, "s" },
            { DatePrecision.Year, "Y" }
        };

        // Entity persister
        private EntityPersistenceService m_entityPersister = new EntityPersistenceService();

        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Person modelInstance, SQLiteConnection context)
        {
            var dbPerson = base.FromModelInstance(modelInstance, context) as DbPerson;

            if (modelInstance.DateOfBirthPrecision.HasValue)
                dbPerson.DateOfBirthPrecision = PrecisionMap[modelInstance.DateOfBirthPrecision.Value];
            return dbPerson;
        }

        /// <summary>
        /// Model instance
        /// </summary>
        public override Person ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var person = dataInstance as DbPerson;
            var dbe = context.Table<DbEntity>().Where(o => o.Uuid == person.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<Person>(dbe, context);
            retVal.DateOfBirth = person.DateOfBirth;

            // Reverse lookup
            if (!String.IsNullOrEmpty(person.DateOfBirthPrecision))
                retVal.DateOfBirthPrecision = PrecisionMap.Where(o => o.Value == person.DateOfBirthPrecision).Select(o => o.Key).First();

            return retVal;
        }

        /// <summary>
        /// Insert the specified person into the database
        /// </summary>
        public override Person Insert(SQLiteConnection context, Person data)
        {
            var inserted = this.m_entityPersister.Insert(context, data);
            data.Key = inserted.Key;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified person
        /// </summary>
        public override Person Update(SQLiteConnection context, Person data)
        {
            this.m_entityPersister.Update(context, data);
            return base.Update(context, data);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override Person Obsolete(SQLiteConnection context, Person data)
        {
            var retVal = this.m_entityPersister.Obsolete(context, data);
            data.StatusConceptKey = retVal.StatusConceptKey;
            data.ObsoletedByKey = retVal.ObsoletedByKey;
            data.ObsoletionTime = retVal.ObsoletionTime;
            return data;
        }
        
    }
}
