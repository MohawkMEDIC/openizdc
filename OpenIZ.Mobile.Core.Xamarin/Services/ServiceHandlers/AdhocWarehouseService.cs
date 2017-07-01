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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Data.Warehouse;
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
        [RestOperation(Method = "GET", UriPath = "/adhocQuery", FaultProvider = nameof(AdhocWarehouseFaultProvider))]
        public object AdHocQuery()
        {
            var warehouseSvc = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();

            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            int totalResults = 0,
                   offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                   count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

            if (!search.ContainsKey("martId")) throw new ArgumentNullException("Missing datamart identifier");
            else
            {
                var dataMart = warehouseSvc.GetDatamart(search["martId"][0]);
                search.Remove("martId");
                search.Remove("_");
                return warehouseSvc.AdhocQuery(dataMart.Id, search);
            }
        }

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
                var results = warehouseSvc.StoredQuery(dataMart.Id, queryName, search);

                // HACK: Sometimes the mart needs to be refreshed
                if (dataMart.Name == "oizcp" && results.Count() == 0)
                {
                    ApplicationContext.Current.GetService<CarePlanManagerService>().RefreshCarePlan(true);
                    throw new Exception("locale.careplan.refreshing");
                }
                return results;
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
                ErrorDescription = e.InnerException?.Message,
                ErrorType = e.GetType().Name
            };
        }

    }
}
