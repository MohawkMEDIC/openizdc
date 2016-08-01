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
    [JsonObject]
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
        /// Search places
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Place", FaultProvider = nameof(ImsiFault))]
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
            // Clean up the patientPatient
            patientToInsert = patientToInsert.Clean() as Patient ;

            IPatientRepositoryService repository = ApplicationContext.Current.GetService<IPatientRepositoryService>();
            // Persist the acts 
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
            // TODO: Select a default identifier domain 
            // Return the patient
            var retVal = new Patient()
            {
                Key = Guid.NewGuid(),
                Names = new List<EntityName>()
                        {
                            new EntityName() { NameUseKey = NameUseKeys.OfficialRecord }
                        },
                Addresses = new List<EntityAddress>()
                {
                    new EntityAddress() {
                        AddressUseKey = AddressUseKeys.HomeAddress,
                        Component = new List<EntityAddressComponent>() {
                            new EntityAddressComponent(AddressComponentKeys.CensusTract, "373a1702-72d8-40a8-b0f5-0e1fd7d86d97")
                        }
                    }
                },
                DateOfBirth = DateTime.Now,
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(null, null)
                    {
                        Authority = new AssigningAuthority() { DomainName = "NEW" },
                        Value = ""
                    }
                },
                Relationships = new List<EntityRelationship>()
                {
                    new EntityRelationship(EntityRelationshipTypeKeys.Mother, new Person() {
                        Telecoms = new List<EntityTelecomAddress>()
                        {
                            new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, null)
                        },
                        Names = new List<EntityName>()
                        {
                            new EntityName() { NameUseKey = NameUseKeys.OfficialRecord }
                        }
                    }),
                    new EntityRelationship(null, new Person() {
                        Telecoms = new List<EntityTelecomAddress>()
                        {
                            new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, null)
                        },
                        Names = new List<EntityName>()
                        {
                            new EntityName() { NameUseKey = NameUseKeys.OfficialRecord }
                        }
                    }),
                    new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, Guid.Parse("d0f4a878-13cb-4509-9b4f-80b2c1548d2b")) // TODO: Get user's current location
                }
            };
            retVal.SetDelayLoad(true);
            // HACK: For form which is expecting $0ther
            (retVal.Relationships[1].TargetEntity as Entity).Telecoms[0].SetDelayLoad(false);
            (retVal.Relationships[0].TargetEntity as Entity).Telecoms[0].SetDelayLoad(false);
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