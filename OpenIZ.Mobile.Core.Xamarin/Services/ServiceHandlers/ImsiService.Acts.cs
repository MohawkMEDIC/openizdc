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
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
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
        public Guid? ActParticipationKeys { get; private set; }

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
            return this.GetAct<Act>();
        }

        /// <summary>
        /// Gets a list of acts.
        /// </summary>
        /// <returns>Returns a list of acts.</returns>
        [RestOperation(Method = "GET", UriPath = "/SubstanceAdministration", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetSubstanceAdministration()
        {
            return this.GetAct<SubstanceAdministration>();
        }

        /// <summary>
        /// Gets a list of acts.
        /// </summary>
        /// <returns>Returns a list of acts.</returns>
        [RestOperation(Method = "GET", UriPath = "/QuantityObservation", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetQuantityObservation()
        {
            return this.GetAct<QuantityObservation>();
        }

        /// <summary>
        /// Gets a list of acts.
        /// </summary>
        /// <returns>Returns a list of acts.</returns>
        [RestOperation(Method = "GET", UriPath = "/TextObservation", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetTextObservation()
        {
            return this.GetAct<TextObservation>();
        }

        /// <summary>
        /// Gets a list of acts.
        /// </summary>
        /// <returns>Returns a list of acts.</returns>
        [RestOperation(Method = "GET", UriPath = "/CodedObservation", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetCodedObservation()
        {
            return this.GetAct<CodedObservation>();
        }

        /// <summary>
        /// Gets a list of acts.
        /// </summary>
        /// <returns>Returns a list of acts.</returns>
        [RestOperation(Method = "GET", UriPath = "/PatientEncounter", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetPatientEncounter()
        {
            return this.GetAct<PatientEncounter>();
        }

        /// <summary>
        /// Get specified service
        /// </summary>
        private IdentifiedData GetAct<TAct>() where TAct : IdentifiedData
        {
            var actRepositoryService = ApplicationContext.Current.GetService<IRepositoryService<TAct>>();
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                ApplicationContext.Current.GetService<IDataCachingService>().Remove(Guid.Parse(search["_id"].FirstOrDefault()));
                var act = actRepositoryService.Get(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                return act;
            }
            else
            {

                var queryId = search.ContainsKey("_state") ? Guid.Parse(search["_state"][0]) : Guid.NewGuid();

                int totalResults = 0,
                       offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                       count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                IEnumerable<TAct> results = null;
                if (search.ContainsKey("_onlineOnly") && search["_onlineOnly"][0] == "true")
                {
                    var integrationService = ApplicationContext.Current.GetService<IClinicalIntegrationService>();
                    var bundle = integrationService.Find<Act>(QueryExpressionParser.BuildLinqExpression<Act>(search, null, false), offset, count);
                    totalResults = bundle.TotalResults;
                    bundle.Reconstitute();
                    bundle.Item.OfType<Act>().ToList().ForEach(o => o.Tags.Add(new ActTag("onlineResult", "true")));
                    results = bundle.Item.OfType<TAct>();

                }
                else if (actRepositoryService is IPersistableQueryRepositoryService)
                {
                    results = (actRepositoryService as IPersistableQueryRepositoryService).Find<TAct>(QueryExpressionParser.BuildLinqExpression<TAct>(search, null, false), offset, count, out totalResults, queryId);
                }
                else
                {
                    results = actRepositoryService.Find(QueryExpressionParser.BuildLinqExpression<TAct>(search, null, false), offset, count, out totalResults);
                }

                //results.ToList().ForEach(a => a.Relationships.OrderBy(r => r.TargetAct.CreationTime));

                this.m_tracer.TraceVerbose("Returning ACT bundle {0}..{1} / {2}", offset, offset + count, totalResults);
                // 
                return new Bundle
                {
                    Key = queryId,
                    Count = results.Count(),
                    Item = results.OfType<IdentifiedData>().ToList(),
                    Offset = offset,
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
                ApplicationContext.Current.GetService<IDataCachingService>().Remove(keyid);
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
        [RestOperation(Method = "DELETE", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Act DeleteAct([RestMessage(RestMessageFormat.SimpleJson)] Act act)
        {
            var query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            Guid actKey = Guid.Empty;
            Guid actVersionKey = Guid.Empty;

            if (query.ContainsKey("_id") && Guid.TryParse(query["_id"][0], out actKey) && query.ContainsKey("_versionId") && Guid.TryParse(query["_versionId"][0], out actVersionKey))
            {
                if (act.Key == actKey && act.VersionKey == actVersionKey)
                {
                    var actRepositoryService = ApplicationContext.Current.GetService<IActRepositoryService>();
                    return actRepositoryService.Obsolete<Act>(act.Key.Value);
                }
                else
                {
                    throw new FileNotFoundException("Act not found");
                }
            }
            else
            {
                throw new FileNotFoundException("Act not found");
            }
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

            if (query.ContainsKey("_id") && Guid.TryParse(query["_id"][0], out actKey))
            {
                if (act.Key == actKey)
                {
                    var actRepositoryService = ApplicationContext.Current.GetService<IActRepositoryService>();
                    return actRepositoryService.Save(act);
                }
                else
                {
                    throw new FileNotFoundException("Act not found");
                }
            }
            else
            {
                throw new FileNotFoundException("Act not found");
            }
        }
    }
}
