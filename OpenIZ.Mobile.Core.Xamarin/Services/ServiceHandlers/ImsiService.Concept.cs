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
