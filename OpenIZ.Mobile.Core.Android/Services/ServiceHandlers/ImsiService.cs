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
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Constants;
using System.Linq;
using OpenIZ.Core.Model.Acts;

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
        /// Create a simplified object
        /// </summary>
        private Stream CreateSimplifiedObject(IdentifiedData obj)
        {
            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
                sw.Write(JsonViewModelSerializer.Serialize(obj));
            return ms;
        }

        /// <summary>
        /// Create a simplified bundle
        /// </summary>
        private Stream CreateSimplifiedBundle(IEnumerable<IdentifiedData> retVal, int offset, int count, int totalResults)
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
        /// Parses a simplified JSON object from the request stream
        /// </summary>
        private TObject ParseSimplifiedObject<TObject>() where TObject : IdentifiedData, new()
        {
            using (var sr = new StreamReader(MiniImsServer.CurrentContext.Request.InputStream))
            {
                var json = sr.ReadToEnd();
                return JsonViewModelSerializer.DeSerialize<TObject>(json);
            }
        }

        /// <summary>
        /// Search places
        /// </summary>
        [RestOperation(Method ="GET", UriPath = "/Place", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Bundle SearchPlace()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var predicate = QueryExpressionParser.BuildLinqExpression<Place>(search);
            var placeService = ApplicationContext.Current.GetService<IPlaceRepositoryService>();
            int totalResults = 0,
                offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;
            var retVal = placeService.Find(predicate, offset, count, out totalResults);

            // Serialize the response
            return new Bundle()
            {
                Item = retVal.OfType<IdentifiedData>().ToList(),
                Offset = offset,
                Count = count,
                TotalResults = totalResults
            };
        }

        /// <summary>
        /// Create patient
        /// </summary>
        [RestOperation(Method = "POST", UriPath = "/Patient", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Patient CreatePatient([RestMessage(RestMessageFormat.SimpleJson)]Patient patientToInsert)
        {
            // Insert the patient
            IPatientRepositoryService repository = ApplicationContext.Current.GetService<IPatientRepositoryService>();
            return repository.Insert(patientToInsert);
        }

        /// <summary>
        /// Get a patient
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Patient", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Bundle GetPatient()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var predicate = QueryExpressionParser.BuildLinqExpression<Patient>(search);
            var patientService = ApplicationContext.Current.GetService<IPatientRepositoryService>();
            int totalResults = 0,
                offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;
            var retVal = patientService.Find(predicate, offset, count, out totalResults);

            // Serialize the response
            return new Bundle()
            {
                Item = retVal.OfType<IdentifiedData>().ToList(),
                Offset = offset,
                Count = count,
                TotalResults = totalResults
            };
        }

        /// <summary>
        /// Get a patient
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Empty/Patient", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Patient GetEmptyPatient()
        {
            // Return the patient
            var retVal = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(null, null)
                },
                Relationships = new List<EntityRelationship>()
                {
                    new EntityRelationship(EntityRelationshipTypeKeys.Mother, new Person() {
                        Telecoms = new List<EntityTelecomAddress>()
                        {
                            new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, null)
                        }
                    }),
                    new EntityRelationship(EntityRelationshipTypeKeys.NextOfKin, new Person() {
                        Telecoms = new List<EntityTelecomAddress>()
                        {
                            new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, null)
                        }
                    }),
                    new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, Guid.Parse("592a4725-dca5-44d1-befb-af7954721317")) // TODO: Get user's current location
                }
            };

            // Serialize the response
            return retVal;
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