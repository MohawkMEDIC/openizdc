using OpenIZ.Core.Model.Roles;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Roles;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Persistence service which is used to persist patients
    /// </summary>
    public class PatientPersistenceService : IdentifiedPersistenceService<Patient, DbPatient>
    {
        // Entity persisters
        private PersonPersistenceService m_personPersister = new PersonPersistenceService();
        protected EntityPersistenceService m_entityPersister = new EntityPersistenceService();

        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Patient modelInstance, SQLiteConnection context)
        {
            var dbPatient = base.FromModelInstance(modelInstance, context) as DbPatient;

            if (modelInstance.DeceasedDatePrecision.HasValue)
                dbPatient.DeceasedDatePrecision = PersonPersistenceService.PrecisionMap[modelInstance.DeceasedDatePrecision.Value];
            return dbPatient;
        }

        /// <summary>
        /// Model instance
        /// </summary>
        public override Patient ToModelInstance(object dataInstance, SQLiteConnection context)
        {

            var iddat = dataInstance as DbVersionedData;
            var patient = dataInstance as DbPatient ?? context.Table<DbPatient>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbe = dataInstance as DbEntity ?? context.Table<DbEntity>().Where(o => o.Uuid == patient.Uuid).First();
            var dbp = context.Table<DbPerson>().Where(o => o.Uuid == patient.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<Patient>(dbe, context);
            retVal.DateOfBirth = dbp.DateOfBirth;
            // Reverse lookup
            if (!String.IsNullOrEmpty(dbp.DateOfBirthPrecision))
                retVal.DateOfBirthPrecision = PersonPersistenceService.PrecisionMap.Where(o => o.Value == dbp.DateOfBirthPrecision).Select(o => o.Key).First();

            retVal.DeceasedDate = patient.DeceasedDate;
            // Reverse lookup
            if (!String.IsNullOrEmpty(patient.DeceasedDatePrecision))
                retVal.DeceasedDatePrecision = PersonPersistenceService.PrecisionMap.Where(o => o.Value == patient.DeceasedDatePrecision).Select(o => o.Key).First();
            retVal.MultipleBirthOrder = patient.MultipleBirthOrder;
            retVal.GenderConceptKey = new Guid(patient.GenderConceptUuid);

            retVal.LoadAssociations(context);

            return retVal;
        }

        /// <summary>
        /// Insert the specified person into the database
        /// </summary>
        public override Patient Insert(SQLiteConnection context, Patient data)
        {
            data.GenderConcept?.EnsureExists(context);
            data.GenderConceptKey = data.GenderConcept?.Key ?? data.GenderConceptKey;

            var inserted = this.m_personPersister.Insert(context, data);
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified person
        /// </summary>
        public override Patient Update(SQLiteConnection context, Patient data)
        {
            // Ensure exists
            data.GenderConcept?.EnsureExists(context);
            data.GenderConceptKey = data.GenderConcept?.Key ?? data.GenderConceptKey;

            this.m_personPersister.Update(context, data);
            return base.Update(context, data);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override Patient Obsolete(SQLiteConnection context, Patient data)
        {
            var retVal = this.m_personPersister.Obsolete(context, data);
            return data;
        }
    }
}
