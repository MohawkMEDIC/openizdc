﻿/*
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
            ConceptPersistenceService cps = new ConceptPersistenceService();

            modelInstance.Concepts = context.Query<DbConcept>("SELECT concept.* FROM concept_concept_set INNER JOIN concept ON (concept_concept_set.concept_uuid = concept.uuid) WHERE concept_concept_set.concept_set_uuid = ?", dbInstance.Uuid).Select(
                o=>cps.ToModelInstance(o, context)
            ).ToList();

            modelInstance.LoadAssociations(context);

            return modelInstance;
        }
    }
}

