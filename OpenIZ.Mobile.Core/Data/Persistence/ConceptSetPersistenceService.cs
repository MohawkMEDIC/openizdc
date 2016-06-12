using System;
using System.Linq;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Data.Model.Concepts;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
	/// <summary>
	/// Persistence service for ConceptSets
	/// </summary>
	public class ConceptSetPersistenceService : IdentifiedPersistenceService<ConceptSet, DbConceptSet>
	{

        /// <summary>
        /// Convert the concept set to model
        /// </summary>
        public override ConceptSet ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            
            var modelInstance = base.ToModelInstance(dataInstance, context);

            // Set the concepts
            var dbInstance = dataInstance as DbConceptSet;
            modelInstance.Concepts = context.Query<DbConcept>("SELECT concept.* FROM concept_concept_set INNER JOIN concept ON (concept_concept_set.concept = concept.uuid) WHERE concept_concept_set.concept_set = ?", dbInstance.Uuid).Select(
                o=>m_mapper.MapDomainInstance<DbConcept, Concept>(o)
            ).ToList();

            return modelInstance;
        }
    }
}

