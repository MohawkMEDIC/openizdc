using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Data.Model.Concepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Concept persistence service
    /// </summary>
    public class ConceptPersistenceService : VersionedDataPersistenceService<Concept, DbConcept>
    {

        /// <summary>
        /// To model instance
        /// </summary>
        public override Concept ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var modelInstance = base.ToModelInstance(dataInstance, context);

            // Set the concepts
            var dbInstance = dataInstance as DbConcept;
            modelInstance.ConceptSets = context.Query<DbConceptSet>("SELECT concept_set.* FROM concept_concept_set INNER JOIN concept_set ON (concept_concept_set.concept_set_uuid = concept_set.uuid) WHERE concept_concept_set.concept_uuid = ?", dbInstance.Uuid).Select(
                o => m_mapper.MapDomainInstance<DbConceptSet, ConceptSet>(o)
            ).ToList();

            // Set the concept names
            //modelInstance.ConceptNames = context.Table<DbConceptName>().Where(o => o.ConceptUuid == dbInstance.Uuid).Select(o => m_mapper.MapDomainInstance<DbConceptName, ConceptName>(o)).ToList();
            
            return modelInstance;
        }

        /// <summary>
        /// Insert concept 
        /// </summary>
        internal override Concept Insert(SQLiteConnection context, Concept data)
        {
            data.StatusConceptKey = data.StatusConceptKey ?? StatusKeys.Active;
            var retVal = base.Insert(context, data);

            // Concept names
            if (retVal.ConceptNames != null)
                base.UpdateAssociatedItems<ConceptName, Concept>(
                    new List<ConceptName>(),
                    retVal.ConceptNames,
                    data.Key,
                    context
                );

            return retVal;
        }

        /// <summary>
        /// Override update to handle associated items
        /// </summary>
        internal override Concept Update(SQLiteConnection context, Concept data)
        {
            var retVal = base.Update(context, data);

            var sourceKey = data.Key.ToByteArray();
            if (retVal.ConceptNames != null)
                base.UpdateAssociatedItems<ConceptName, Concept>(
                    context.Table<DbConceptName>().Where(o => o.ConceptUuid == sourceKey).ToList().Select(o=>m_mapper.MapDomainInstance<DbConceptName, ConceptName>(o)).ToList(),
                    data.ConceptNames,
                    retVal.Key,
                    context
                    );

            return retVal;
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        internal override Concept Obsolete(SQLiteConnection context, Concept data)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Obsolete(context, data);
        }
    }
}
