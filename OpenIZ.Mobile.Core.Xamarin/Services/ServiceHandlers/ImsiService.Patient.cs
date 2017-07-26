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
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Patch;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Core.Services.Impl;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Xamarin.Resources;
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
    /// IMSI Service
    /// </summary>
    public partial class ImsiService
    {

        /// <summary>
        /// Creates a patient.
        /// </summary>
        /// <param name="patientToInsert">The patient to be inserted.</param>
        /// <returns>Returns the inserted patient.</returns>
        [RestOperation(Method = "POST", UriPath = "/Patient", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Patient CreatePatient([RestMessage(RestMessageFormat.SimpleJson)]Patient patientToInsert)
        {
            IPatientRepositoryService repository = ApplicationContext.Current.GetService<IPatientRepositoryService>();
            return repository.Insert(patientToInsert).GetLocked() as Patient;
        }


        /// <summary>
        /// Updates a patient.
        /// </summary>
        /// <param name="patientToUpdate">The patient to be updated.</param>
        /// <returns>Returns the updated patient.</returns>
        [RestOperation(Method = "PUT", UriPath = "/Patient", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Patient UpdatePatient([RestMessage(RestMessageFormat.SimpleJson)]Patient patientToUpdate)
        {
            IPatientRepositoryService repository = ApplicationContext.Current.GetService<IPatientRepositoryService>();
            patientToUpdate.VersionKey = null;
            // Get all the acts if none were supplied, and all of the relationships if none were supplied
            return repository.Save(patientToUpdate).GetLocked() as Patient;
        }

        /// <summary>
		/// Gets a patient.
		/// </summary>
		/// <returns>Returns the patient.</returns>
        [RestOperation(Method = "GET", UriPath = "/Patient", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetPatient()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var patientService = ApplicationContext.Current.GetService<IPatientRepositoryService>();
            var integrationService = ApplicationContext.Current.GetService<IClinicalIntegrationService>();

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                //MemoryCache.Current.RemoveObject(typeof(Patient), Guid.Parse(search["_id"].FirstOrDefault()));
                // Filtes
                Patient patient = null;
                if (search.ContainsKey("_onlineOnly"))
                {
                    patient = integrationService.Get<Patient>(Guid.Parse(search["_id"].FirstOrDefault()), null, new IntegrationQueryOptions()
                    {
                        Expand = new String[] { "relationship.target" }
                    });
                    // Add this to the cache
                    //ApplicationContext.Current.GetService<IDataCachingService>().Add(patient);
                    patient.Tags.Add(new EntityTag("onlineResult", "true"));
                }
                else
                    patient = patientService.Get(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);

                if (patient == null)
                    throw new FileNotFoundException();

                return patient;
            }
            else
            {

                int totalResults = 0,
                    offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                    count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;
                Guid queryId = search.ContainsKey("_queryId") ? Guid.Parse(search["_queryId"][0]) : Guid.Empty;

                IEnumerable<Patient> retVal = null;

                // Any filter
                if (search.ContainsKey("any") || search.ContainsKey("any[]"))
                {

                    this.m_tracer.TraceVerbose("Freetext search: {0}", MiniImsServer.CurrentContext.Request.Url.Query);

                    var values = search.ContainsKey("any") ? search["any"] : search["any[]"];
                    
                    // Filtes
                    if (search.ContainsKey("_onlineOnly"))
                    {
                        String[] tryFields = { "identifier.value", "name.component.value", "relationship[Mother].target.name.component.value" };

                        Bundle bundle = new Bundle();
                        int tryField = 0;
                        // Created predicate for value
                        while (bundle.TotalResults == 0 && tryField < tryFields.Length)
                        {
                            var predicate = QueryExpressionParser.BuildLinqExpression<Patient>(
                                new NameValueCollection() {
                                    { tryFields[tryField] , values.Select(o=>tryFields[tryField].Contains("name.component.value") ? "~" + o : o).ToList() }
                                }
                            );

                            bundle = integrationService.Find(predicate, offset, count, new IntegrationQueryOptions() {
                                Expand = new String[] { "relationship.target" }
                            });
                            tryField++;
                        }

                        // Now compose bundle
                        if (bundle != null && bundle.TotalResults > 0)
                        {
                            totalResults = bundle.TotalResults;
                            bundle.Reconstitute();
                            retVal = bundle.Item.OfType<Patient>();
                        }
                    }
                    else
                    {
                        var fts = ApplicationContext.Current.GetService<IFreetextSearchService>();
                        retVal = fts.Search<Patient>(values.Select(o=>o.Replace("~","")).ToArray(), offset, count, out totalResults);
                    }

                    search.Remove("any");
                    search.Remove("any[]");
                }

                // There is additional filter parameters
                if (search.Keys.Count(o => !o.StartsWith("_")) > 0)
                {
                    var predicate = QueryExpressionParser.BuildLinqExpression<Patient>(search, null, false);
                    this.m_tracer.TraceVerbose("Searching Patients : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);

                    if (search.ContainsKey("_onlineOnly"))
                    {
                        var bundle = integrationService.Find(predicate, offset, count);
                        totalResults = bundle.TotalResults;
                        bundle.Reconstitute();
                        if (retVal == null)
                            retVal = bundle.Item.OfType<Patient>();
                        else
                            retVal = retVal.OfType<IIdentifiedEntity>().Intersect(bundle.Item.OfType<IIdentifiedEntity>(), new KeyComparer()).OfType<Patient>();
                    }
                    else
                    {
                        if(retVal != null)
                            retVal = retVal.Where(predicate.Compile());
                        else
                        {
                            if (search.ContainsKey("_viewModel") && search["_viewModel"][0] != "full")
                                retVal = (patientService as IFastQueryRepositoryService).FindFast(predicate, offset, count, out totalResults, queryId);
                            else
                                retVal = (patientService as IPersistableQueryRepositoryService).Find(predicate, offset, count, out totalResults, queryId);
                        }
                    }
                }

				if (retVal == null)
				{
					retVal = new List<Patient>();
				}

				retVal = retVal.Select(o => o.Clone()).OfType<Patient>();
                if (search.ContainsKey("_onlineOnly"))
                    retVal.ToList().ForEach((o) => {
                        ApplicationContext.Current.GetService<IDataCachingService>().Remove(o.Key.Value);
                        if (patientService.Get(o.Key.Value, Guid.Empty) == null)
                            o.Tags.Add(new EntityTag("onlineResult", "true"));
                    });

                // Serialize the response
                return new Bundle()
                {
                    Item = retVal.OfType<IdentifiedData>().ToList(),
                    Offset = offset,
                    Count = retVal.Count(),
                    TotalResults = totalResults
                };
            }
        }

        /// <summary>
		/// Gets a patient.
		/// </summary>
		/// <returns>Returns the patient.</returns>
        [RestOperation(Method = "GET", UriPath = "/Patient.Download", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData DownloadPatient()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            if (!search.ContainsKey("_id"))
                throw new ArgumentNullException("Missing _id parameter");

            Guid patientId = Guid.Parse(search["_id"][0]);

            // Get the patient
            var imsiIntegrationService = ApplicationContext.Current.GetService<IClinicalIntegrationService>();
            var pdp = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();

            // We shove the data onto the queue for import!!! :)
            ApplicationContext.Current.SetProgress(Strings.locale_downloadingExternalPatient, 0.1f);
            var dbundle = imsiIntegrationService.Find<Patient>(o=>o.Key == patientId, 0, 1);
            dbundle.Item.RemoveAll(o => !(o is Patient || o is Person));
            pdp.Insert(dbundle);
            var patient = dbundle.Item.OfType<Patient>().FirstOrDefault();

            // We now want to subscribe this patient our facility
            var facilityId = ApplicationContext.Current.Configuration.GetSection<SynchronizationConfigurationSection>().Facilities?.FirstOrDefault();
            if (facilityId != null)
            {
                patient.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.IncidentalServiceDeliveryLocation, Guid.Parse(facilityId)));
                imsiIntegrationService.Update(patient);
            }
            var personBundle = new Bundle();
            foreach( var rel in patient.Relationships)
            {
                var person = imsiIntegrationService.Get<Entity>(rel.TargetEntityKey.Value, null);
                if(person != null && person.Type == "Person")
                {
                    personBundle.Add(person as Person);
                }
            }
            if(personBundle.Item.Count > 0)
            {
                pdp.Insert(personBundle);
            }
            int tr = 1,
                ofs = 0;
            Guid qid = Guid.NewGuid();

            while(ofs < tr)
            {
                var bundle = imsiIntegrationService.Find<Act>(o => o.Participations.Where(p=>p.ParticipationRole.Mnemonic == "RecordTarget").Any(p=>p.PlayerEntityKey == patientId), ofs, 20);
                //bundle.Reconstitute();
                tr = bundle.TotalResults;
                ApplicationContext.Current.SetProgress(Strings.locale_downloadingExternalPatient, ((float)ofs / tr) * 0.9f + 0.1f);
                ofs += 20;
                pdp.Insert(bundle);

            }

            return patient;
        }


        /// <summary>
        /// Get a patient
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Empty/Patient", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.Login)]
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
                    new EntityIdentifier("NEW", "")
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
            // Serialize the response
            return retVal;
        }
    }
}
