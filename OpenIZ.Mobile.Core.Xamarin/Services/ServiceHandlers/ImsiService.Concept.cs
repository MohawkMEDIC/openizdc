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
 * Date: 2017-3-31
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Concept service handlers for the IMSI
    /// </summary>
    public partial class ImsiService
    {
        /// <summary>
        /// Gets a list of acts.
        /// </summary>
        /// <returns>Returns a list of acts.</returns>
        [RestOperation(Method = "GET", UriPath = "/Concept", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.ReadMetadata)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetConcept()
        {
            var conceptRepositoryService = ApplicationContext.Current.GetService<IConceptRepositoryService>();
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Concept), Guid.Parse(search["_id"].FirstOrDefault()));
                var concept = conceptRepositoryService.GetConcept(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                return concept;
            }
            else
            {
                int totalResults = 0,
                       offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                       count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                var results = conceptRepositoryService.FindConcepts(QueryExpressionParser.BuildLinqExpression<Concept>(search, null, false), offset, count, out totalResults);

                // 
                return new Bundle
                {
                    Count = results.Count(),
                    Item = results.OfType<IdentifiedData>().ToList(),
                    Offset = 0,
                    TotalResults = totalResults
                };
            }
        }

        /// <summary>
        /// Gets a list of acts.
        /// </summary>
        /// <returns>Returns a list of acts.</returns>
        [RestOperation(Method = "GET", UriPath = "/ConceptSet", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.ReadMetadata)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetConceptSet()
        {
            var conceptRepositoryService = ApplicationContext.Current.GetService<IConceptRepositoryService>();
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Concept), Guid.Parse(search["_id"].FirstOrDefault()));
                var concept = conceptRepositoryService.GetConceptSet(Guid.Parse(search["_id"].FirstOrDefault()));
                return concept;
            }
            else
            {
                int totalResults = 0,
                       offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                       count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                var results = conceptRepositoryService.FindConceptSets(QueryExpressionParser.BuildLinqExpression<ConceptSet>(search, null, false), offset, count, out totalResults);

                // 
                return new Bundle
                {
                    Count = results.Count(),
                    Item = results.OfType<IdentifiedData>().ToList(),
                    Offset = 0,
                    TotalResults = totalResults
                };
            }
        }
    }
}
