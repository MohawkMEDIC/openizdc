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
 * Date: 2016-10-31
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
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
    /// IMSI Service handler for acts
    /// </summary>
    public partial class ImsiService
    {

        /// <summary>
        /// Creates an act.
        /// </summary>
        /// <param name="actToInsert">The act to be inserted.</param>
        /// <returns>Returns the inserted act.</returns>
        [RestOperation(Method = "POST", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Act CreateAct([RestMessage(RestMessageFormat.SimpleJson)]Act actToInsert)
        {
            IActRepositoryService actService = ApplicationContext.Current.GetService<IActRepositoryService>();
            return actService.Insert(actToInsert);
        }

        /// <summary>
        /// Gets a list of acts.
        /// </summary>
        /// <returns>Returns a list of acts.</returns>
        [RestOperation(Method = "GET", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetAct()
        {
            var actRepositoryService = ApplicationContext.Current.GetService<IActRepositoryService>();
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Act), Guid.Parse(search["_id"].FirstOrDefault()));
                var act = actRepositoryService.Get<Act>(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                return act;
            }
            else
            {

                var queryId = search.ContainsKey("_state") ? Guid.Parse(search["_state"][0]) : Guid.NewGuid();

                int totalResults = 0,
                       offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                       count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                IEnumerable<Act> results = null;
                if (actRepositoryService is IPersistableQueryProvider)
                {
                    results = (actRepositoryService as IPersistableQueryProvider).Query<Act>(QueryExpressionParser.BuildLinqExpression<Act>(search, null, false), offset, count, out totalResults, queryId);
                }
                else
                {
                    results = actRepositoryService.Find(QueryExpressionParser.BuildLinqExpression<Act>(search, null, false), offset, count, out totalResults);
                }

                results.ToList().ForEach(a => a.Relationships.OrderBy(r => r.TargetAct.CreationTime));

                // 
                return new Bundle
                {
                    Key = queryId,
                    Count = results.Count(),
                    Item = results.OfType<IdentifiedData>().ToList(),
                    Offset = 0,
                    TotalResults = totalResults
                };
            }
        }

        /// <summary>
        /// Deletes the act
        /// </summary>
        [RestOperation(Method = "DELETE", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.DeleteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Act DeleteAct()
        {
            var actRepositoryService = ApplicationContext.Current.GetService<IActRepositoryService>();

            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                var keyid = Guid.Parse(search["_id"].FirstOrDefault());
                MemoryCache.Current.RemoveObject(typeof(Act), keyid);
                var act = actRepositoryService.Get<Act>(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                if (act == null) throw new KeyNotFoundException();

                return actRepositoryService.Obsolete<Act>(keyid);
            }
            else
                throw new ArgumentNullException("_id");
        }


        /// <summary>
        /// Updates an act.
        /// </summary>
        /// <param name="act">The act to update.</param>
        /// <returns>Returns the updated act.</returns>
        [RestOperation(Method = "PUT", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Act UpdateAct([RestMessage(RestMessageFormat.SimpleJson)] Act act)
        {
            var query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            Guid actKey = Guid.Empty;
            Guid actVersionKey = Guid.Empty;

            if (query.ContainsKey("_id") && Guid.TryParse(query["_id"][0], out actKey) && query.ContainsKey("_versionId") && Guid.TryParse(query["_versionId"][0], out actVersionKey))
            {
                if (act.Key == actKey && act.VersionKey == actVersionKey)
                {
                    var actRepositoryService = ApplicationContext.Current.GetService<IActRepositoryService>();
                    if(act.ObsoletionTime == null)
                    {
                        return actRepositoryService.Save(act);
                    }
                    return actRepositoryService.Obsolete<Act>(act.Key.Value);
                }
                else
                {
                    throw new ArgumentException("Act not found");
                }
            }
            else
            {
                throw new ArgumentException("Act not found");
            }
        }
    }
}
