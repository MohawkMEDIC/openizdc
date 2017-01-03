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
 * Date: 2016-10-11
 */
using System;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Collection;
using System.Linq;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Diagnostics;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Acts;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using System.Collections.Generic;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Forecasting service
    /// </summary>
    [RestService("/__plan")]
    public class CarePlanService
    {

        private Tracer m_tracer = Tracer.GetTracer(typeof(CarePlanService));

        /// <summary>
        /// Gets the specified forecast
        /// </summary>
        [RestOperation(UriPath = "/patient", Method = "POST", FaultProvider = nameof(CarePlanFaultProvider))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Bundle CreatePlan([RestMessage(RestMessageFormat.SimpleJson)]Patient p)
        {
            // Additional instructions?
            this.m_tracer.TraceInfo(">>>> CALCULATING CARE PLAN ");
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var predicate = QueryExpressionParser.BuildLinqExpression<Act>(search);

            if(search.ContainsKey("_patientId"))
            {
                var patientSvc = ApplicationContext.Current.GetService<IPatientRepositoryService>();
                p = patientSvc.Get(Guid.Parse(search["_patientId"][0]), Guid.Empty).Clone() as Patient;
                p.Participations = new List<ActParticipation>(p.Participations);
            }
            if(p.Participations.Count == 0)
            {
                var actService = ApplicationContext.Current.GetService<IActRepositoryService>();
                int tr = 0;
                IEnumerable<Act> acts = null;
                if (actService is IPersistableQueryProvider && search.ContainsKey("_state"))
                    acts = (actService as IPersistableQueryProvider).Query<Act>(o => o.Participations.Any(guard => guard.ParticipationRole.Mnemonic == "RecordTarget" && guard.PlayerEntityKey == p.Key), 0, 200, out tr, Guid.Parse(search["_state"][0]));
                else
                    acts = actService.Find<Act>(o => o.Participations.Any(guard => guard.ParticipationRole.Mnemonic == "RecordTarget" && guard.PlayerEntityKey == p.Key), 0, 200, out tr);

                p.Participations = acts.Select(a=>new ActParticipation(ActParticipationKey.RecordTarget, p) { Act = a, ParticipationRole = new OpenIZ.Core.Model.DataTypes.Concept() { Mnemonic = "RecordTarget" } }).ToList();

            }
            var protocolService = ApplicationContext.Current.GetService<ICarePlanService>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var plan = new List<Act>(protocolService.CreateCarePlan(p));
            sw.Stop();
            this.m_tracer.TraceInfo(">>>> CARE PLAN CONSTRUCTED IN {0}", sw.Elapsed);
            // Instructions?
            if (search.Count > 0)
            {
                var pred = predicate.Compile();
                plan.RemoveAll(o => !pred(o));
            }

            return new Bundle() { Item = plan.OfType<IdentifiedData>().ToList() };
            //return plan;
        }

        /// <summary>
        /// Care plan fault provider
        /// </summary>
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public ErrorResult CarePlanFaultProvider(Exception e)
        {
            return new ErrorResult() { Error = e.Message, ErrorDescription = e.InnerException?.Message, ErrorType = e.GetType().Name };
        }

    }
}