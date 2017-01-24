/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-14
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
        public override Concept ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var modelInstance = base.ToModelInstance(dataInstance, context, loadFast);

            // Set the concepts
            var dbInstance = dataInstance as DbConcept;
            modelInstance.ConceptSets = context.Query<DbConceptSet>("SELECT concept_set.* FROM concept_concept_set INNER JOIN concept_set ON (concept_concept_set.concept_set_uuid = concept_set.uuid) WHERE concept_concept_set.concept_uuid = ?", dbInstance.Uuid).Select(
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
        public override Concept Insert(SQLiteConnectionWithLock context, Concept data)
        {
            // Ensure exists
            data.Class?.EnsureExists(context);
            //data.StatusConcept?.EnsureExists(context);
            data.ClassKey = data.Class?.Key ?? data.ClassKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;

            data.StatusConceptKey = data.StatusConceptKey ?? StatusKeys.Active;
            data.ClassKey = data.ClassKey ?? ConceptClassKeys.Other;


            // Persist
            var retVal = base.Insert(context, data);

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
                    if (context.Table<DbConceptSetConceptAssociation>().Where(o => o.ConceptSetUuid == r.ToByteArray() &&
                     o.ConceptUuid == retVal.Key.Value.ToByteArray()).Any())
                        continue;
                    else
                        context.Insert(new DbConceptSetConceptAssociation()
                        {
                            Uuid = Guid.NewGuid().ToByteArray(),
                            ConceptSetUuid = r.ToByteArray(),
                            ConceptUuid = retVal.Key.Value.ToByteArray()
                        });
                }

            return retVal;
        }

        /// <summary>
        /// Override update to handle associated items
        /// </summary>
        public override Concept Update(SQLiteConnectionWithLock context, Concept data)
        {
            data.Class?.EnsureExists(context);
            //data.StatusConcept?.EnsureExists(context);
            data.ClassKey = data.Class?.Key ?? data.ClassKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;

            var retVal = base.Update(context, data);

            var sourceKey = data.Key.Value.ToByteArray();
            if (retVal.ConceptNames != null)
                base.UpdateAssociatedItems<ConceptName, Concept>(
                    context.Table<DbConceptName>().Where(o => o.ConceptUuid == sourceKey).ToList().Select(o => m_mapper.MapDomainInstance<DbConceptName, ConceptName>(o)).ToList(),
                    data.ConceptNames,
                    retVal.Key,
                    context
                    );

            // Wipe and re-associate
            if (retVal.ConceptSetsXml != null && retVal.ConceptSetsXml.Count > 0)
            {
                context.Table<DbConceptSetConceptAssociation>().Delete(o => o.ConceptUuid == sourceKey);
                foreach (var r in retVal.ConceptSetsXml)
                {
                    context.Insert(new DbConceptSetConceptAssociation()
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
        public override Concept Obsolete(SQLiteConnectionWithLock context, Concept data)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Obsolete(context, data);
        }
    }
}
