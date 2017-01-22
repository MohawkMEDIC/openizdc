using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Interacts with the ad-hoc data warehouse
    /// </summary>
    [RestService("/__zombo")]
    public class AdhocWarehouseService
    {

        /// <summary>
        /// Performs an ad-hoc query against the datawarehouse
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/storedQuery", FaultProvider = nameof(AdhocWarehouseFaultProvider))]
        public object StoredQuery()
        {
            var warehouseSvc = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();

            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            int totalResults = 0,
                   offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                   count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

            if (!search.ContainsKey("martId")) throw new ArgumentNullException("Missing datamart identifier");
            else if (!search.ContainsKey("queryId")) throw new ArgumentNullException("Query identifier");
            else
            {
                var dataMart = warehouseSvc.GetDatamart(search["martId"][0]);
                var queryName = search["queryId"][0];
                search.Remove("martId");
                search.Remove("queryId");
                search.Remove("_");
                return warehouseSvc.StoredQuery(dataMart.Id, queryName, search);
            }
        }

        /// <summary>
        /// Adhoc warehouse fault provider
        /// </summary>
        public ErrorResult AdhocWarehouseFaultProvider(Exception e)
        {
            return new ErrorResult()
            {
                Error = e.Message,
                ErrorDescription = e.ToString(),
                ErrorType = e.GetType().Name
            };
        }

    }
}
