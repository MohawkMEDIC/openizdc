using System;
using System.Collections.Generic;

using OpenIZ.Mobile.Core.Android.Services.Attributes;
using OpenIZ.Core.Model.Entities;
using System.IO;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Core.Model.Collection;
using System.Text;

namespace OpenIZ.Mobile.Core.Android.Services.ServiceHandlers
{

    /// <summary>
    /// General IMSI error result
    /// </summary>
    public class ErrorResult
    {
        [JsonProperty("error")]
        public String Error { get; set; }
        [JsonProperty("error_description")]
        public String ErrorDescription { get; set; }
    }

    /// <summary>
    /// Represents an IMS service handler
    /// </summary>
    [RestService("/__ims")]
    public class ImsiService
    {
        /// <summary>
        /// Create a simplified bundle
        /// </summary>
        private Stream CreateSimplifiedBundle(IEnumerable<Place> retVal, int offset, int count, int totalResults)
        {
            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
            {
                var bundle = new Bundle()
                {
                    Item = new List<OpenIZ.Core.Model.IdentifiedData>(retVal),
                    Offset = offset,
                    Count = count,
                    TotalResults = totalResults
                };
                sw.Write(JsonViewModelSerializer.Serialize(bundle.GetLocked()));
            }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// Search places
        /// </summary>
        [RestOperation(Method ="GET", UriPath = "/Place", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.Raw)]
        public Stream SearchPlace()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var predicate = QueryExpressionParser.BuildLinqExpression<Place>(search);
            var placeService = ApplicationContext.Current.GetService<IPlaceRepositoryService>();
            int totalResults = 0,
                offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;
            var retVal = placeService.Find(predicate, offset, count, out totalResults);

            // Serialize the response
            return this.CreateSimplifiedBundle(retVal, offset, count, totalResults);
        }


        /// <summary>
        /// Handle a fault
        /// </summary>
        public ErrorResult ImsiFault(Exception e)
        {
            return new ErrorResult()
            {
                Error = e.Message,
                ErrorDescription = e.InnerException?.Message
            };
        }
        
    }
}