/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-2-4
 */
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Data.Model.Concepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
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
        public override Concept ToModelInstance(object dataInstance, LocalDataContext context, bool loadFast)
        {
            var modelInstance = base.ToModelInstance(dataInstance, context, loadFast);

            // Set the concepts
            var dbInstance = dataInstance as DbConcept;
            modelInstance.ConceptSets = context.Connection.Query<DbConceptSet>("SELECT concept_set.* FROM concept_concept_set INNER JOIN concept_set ON (concept_concept_set.concept_set_uuid = concept_set.uuid) WHERE concept_concept_set.concept_uuid = ?", dbInstance.Uuid).Select(
                o => m_mapper.MapDomainInstance<DbConceptSet, ConceptSet>(o)
            ).ToList();

            // Set the concept names
            //modelInstance.ConceptNames = context.Table<DbConceptName>().Where(o => o.ConceptUuid == dbInstance.Uuid).Select(o => m_mapper.MapDomainInstance<DbConceptName, ConceptName>(o)).ToList();
            //modelInstance.StatusConcept = m_mapper.MapDomainInstance<DbConcept, Concept>(context.Table<DbConcept>().Where(o => o.Uuid == dbInstance.StatusUuid).FirstOrDefault());
            //modelInstance.Class = m_mapper.MapDomainInstance<DbConceptClass, ConceptClass>(context.Table<DbConceptClass>().Where(o => o.Uuid == dbInstance.ClassUuid).FirstOrDefault());
            //modelInstance.LoadAssociations(context);

            return modelInstance;
        }

        /// <summary>
        /// Insert concept 
        /// </summary>
        protected override Concept InsertInternal(LocalDataContext context, Concept data)
        {
            // Ensure exists
            if(data.Class != null) data.Class = data.Class?.EnsureExists(context);
            if(data.StatusConcept != null) data.StatusConcept?.EnsureExists(context);
            data.ClassKey = data.Class?.Key ?? data.ClassKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;

            data.StatusConceptKey = data.StatusConceptKey ?? StatusKeys.Active;
            data.ClassKey = data.ClassKey ?? ConceptClassKeys.Other;


            // Persist
            var retVal = base.InsertInternal(context, data);

            // Concept names
            if (retVal.ConceptNames != null)
                base.UpdateAssociatedItems<ConceptName, Concept>(
                    new List<ConceptName>(),
                    retVal.ConceptNames,
                    data.Key.Value,
                    context
                );

            if (retVal.ConceptSetsXml != null)
                foreach (var r in retVal.ConceptSetsXml)
                {
					// HACK: SQL lite has decided that there is no such function as "ToByteArray()"
					var conceptSetUuid = r.ToByteArray();
					var conceptUuid = retVal.Key.Value.ToByteArray();

                    if (context.Connection.Table<DbConceptSetConceptAssociation>().Where(o => o.ConceptSetUuid == conceptSetUuid &&
                     o.ConceptUuid == conceptUuid).Any())
                        continue;
					else
					{
						// HACK: SQL lite has decided that there is no such function as "ToByteArray()"
						var key = Guid.NewGuid().ToByteArray();
						context.Connection.Insert(new DbConceptSetConceptAssociation()
						{
							Uuid = key,
							ConceptSetUuid = conceptSetUuid,
							ConceptUuid = conceptUuid
						});
					}
                }

            return retVal;
        }

        /// <summary>
        /// Override update to handle associated items
        /// </summary>
        protected override Concept UpdateInternal(LocalDataContext context, Concept data)
        {
            if(data.Class != null) data.Class = data.Class?.EnsureExists(context);
            if(data.StatusConcept != null) data.StatusConcept = data.StatusConcept?.EnsureExists(context);
            data.ClassKey = data.Class?.Key ?? data.ClassKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;

            var retVal = base.UpdateInternal(context, data);

            var sourceKey = data.Key.Value.ToByteArray();
            if (retVal.ConceptNames != null)
                base.UpdateAssociatedItems<ConceptName, Concept>(
                    context.Connection.Table<DbConceptName>().Where(o => o.ConceptUuid == sourceKey).ToList().Select(o => m_mapper.MapDomainInstance<DbConceptName, ConceptName>(o)).ToList(),
                    data.ConceptNames,
                    retVal.Key,
                    context
                    );

            // Wipe and re-associate
            if (retVal.ConceptSetsXml != null && retVal.ConceptSetsXml.Count > 0)
            {
                context.Connection.Table<DbConceptSetConceptAssociation>().Delete(o => o.ConceptUuid == sourceKey);
                foreach (var r in retVal.ConceptSetsXml)
                {
                    context.Connection.Insert(new DbConceptSetConceptAssociation()
                    {
                        Uuid = Guid.NewGuid().ToByteArray(),
                        ConceptSetUuid = r.ToByteArray(),
                        ConceptUuid = retVal.Key.Value.ToByteArray()
                    });
                }

            }
            return retVal;
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        protected override Concept ObsoleteInternal(LocalDataContext context, Concept data)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.ObsoleteInternal(context, data);
        }
    }
}
