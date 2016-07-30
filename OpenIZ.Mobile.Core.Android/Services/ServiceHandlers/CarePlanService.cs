using System;
using OpenIZ.Mobile.Core.Android.Services.Attributes;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Collection;
using System.Linq;
using OpenIZ.Core.Model;

namespace OpenIZ.Mobile.Core.Android.Services.ServiceHandlers
{
    /// <summary>
    /// Forecasting service
    /// </summary>
    [RestService("/__plan")]
    public class CarePlanService
    {

        /// <summary>
        /// Gets the specified forecast
        /// </summary>
        [RestOperation(UriPath = "/patient", Method = "POST", FaultProvider = nameof(CarePlanFaultProvider))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Patient CreatePlan([RestMessage(RestMessageFormat.SimpleJson)]Patient p)
        {
            
            var protocolService = ApplicationContext.Current.GetService<ICarePlanService>();
            var plan = protocolService.CreateCarePlan(p);
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