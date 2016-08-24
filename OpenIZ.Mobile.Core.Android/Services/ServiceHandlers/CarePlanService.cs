using System;
using OpenIZ.Mobile.Core.Android.Services.Attributes;
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
using OpenIZ.Core.Model.Reflection;
using OpenIZ.Mobile.Core.Android.Services.Model;

namespace OpenIZ.Mobile.Core.Android.Services.ServiceHandlers
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
        public Patient CreatePlan([RestMessage(RestMessageFormat.SimpleJson)]Patient p)
        {
            // Additional instructions?
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var predicate = QueryExpressionParser.BuildLinqExpression<Act>(search);

            var protocolService = ApplicationContext.Current.GetService<ICarePlanService>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var plan = protocolService.CreateCarePlan(p);
            sw.Stop();
            this.m_tracer.TraceInfo(">>>> CARE PLAN CONSTRUCTED IN {0}", sw.Elapsed);
            // Instructions?
            if (search.Count > 0)
            {
                var pred = predicate.Compile();
                p.Participations.RemoveAll(o => !pred(o.Act));
            }

            //return new Bundle() { Item = plan.OfType<IdentifiedData>().ToList() };
            return p;
        }

        /// <summary>
        /// Care plan fault provider
        /// </summary>
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public ErrorResult CarePlanFaultProvider(Exception e)
        {
            return new ErrorResult() { Error = e.Message, ErrorDescription = e.InnerException?.Message };
        }

    }
}