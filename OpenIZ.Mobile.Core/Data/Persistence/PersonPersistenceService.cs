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
    public class PersonPersistenceService : EntityDerivedPersistenceService<Person, DbPerson>
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
        /// Inserts the specified person
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Person Insert(SQLiteConnection context, Person data)
        {
            var retVal = base.Insert(context, data);
            byte[] sourceKey = retVal.Key.ToByteArray();

            if (data.LanguageCommunication != null)
                base.UpdateAssociatedItems<PersonLanguageCommunication, Entity>(
                    new List<PersonLanguageCommunication>(),
                    data.LanguageCommunication,
                    retVal.Key,
                    context);
            return retVal;
        }

        /// <summary>
        /// Update the person entity
        /// </summary>
        public override Person Update(SQLiteConnection context, Person data)
        {
            var retVal = base.Update(context, data);
            var sourceKey = retVal.Key.ToByteArray();

            // Language communication
            if (data.LanguageCommunication != null)
                base.UpdateAssociatedItems<PersonLanguageCommunication, Entity>(
                    context.Table<DbPersonLanguageCommunication>().Where(o=>o.EntityUuid == sourceKey).ToList().Select(o=>m_mapper.MapDomainInstance<DbPersonLanguageCommunication, PersonLanguageCommunication>(o)).ToList(),
                    data.LanguageCommunication,
                    retVal.Key,
                    context);
            return retVal;
        }
    }
}
