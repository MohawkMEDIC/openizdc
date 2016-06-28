using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents an organization persistence service
    /// </summary>
    public class OrganizationPersistenceService : EntityDerivedPersistenceService<Organization, DbOrganization>
    {

       
        /// <summary>
        /// Model instance
        /// </summary>
        public override Organization ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var organization = dataInstance as DbOrganization;
            var dbe = context.Table<DbEntity>().Where(o => o.Uuid == organization.Uuid).First();
            var retVal = m_entityPersister.ToModelInstance<Organization>(dbe, context);
            retVal.IndustryConceptKey = new Guid(organization.IndustryConceptUuid);
            return retVal;
        }

        /// <summary>
        /// Insert the organization
        /// </summary>
        public override Organization Insert(SQLiteConnection context, Organization data)
        {
            // ensure industry concept exists
            data.IndustryConcept?.EnsureExists(context);
            data.IndustryConceptKey = data.IndustryConcept?.Key ?? data.IndustryConceptKey;

            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the organization
        /// </summary>
        public override Organization Update(SQLiteConnection context, Organization data)
        {
            data.IndustryConcept?.EnsureExists(context);
            data.IndustryConceptKey = data.IndustryConcept?.Key ?? data.IndustryConceptKey;
            return base.Update(context, data);
        }

    }
}
