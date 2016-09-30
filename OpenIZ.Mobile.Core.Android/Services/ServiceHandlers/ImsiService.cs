using System;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Extensions;
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
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Android.AppletEngine.JNI;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Mobile.Core.Android.Services.Model;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Android.Services.ServiceHandlers
{
    /// <summary>
    /// Represents an IMS service handler
    /// </summary>
    [RestService("/__ims")]
    public class ImsiService
    {
        // Tracer 
        private Tracer m_tracer = Tracer.GetTracer(typeof(ImsiService));

		/// <summary>
		/// Creates an act.
		/// </summary>
		/// <param name="actToInsert">The act to be inserted.</param>
		/// <returns>Returns the inserted act.</returns>
		[RestOperation(Method = "POST", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
		[return: RestMessage(RestMessageFormat.SimpleJson)]
		public Act CreateAct([RestMessage(RestMessageFormat.SimpleJson)]Act actToInsert)
		{
			IActRepositoryService actService = ApplicationContext.Current.GetService<IActRepositoryService>();

			return actService.Insert(actToInsert);
		}

		/// <summary>
		/// Creates a patient.
		/// </summary>
		/// <param name="patientToInsert">The patient to be inserted.</param>
		/// <returns>Returns the inserted patient.</returns>
		[RestOperation(Method = "POST", UriPath = "/Patient", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Patient CreatePatient([RestMessage(RestMessageFormat.SimpleJson)]Patient patientToInsert)
        {
            IPatientRepositoryService repository = ApplicationContext.Current.GetService<IPatientRepositoryService>();

            return repository.Insert(patientToInsert);
        }
        
        /// <summary>
		/// Creates a bundle.
		/// </summary>
		/// <param name="bundleToInsert">The bundle to be inserted.</param>
		/// <returns>Returns the inserted bundle.</returns>
        [RestOperation(Method = "POST", UriPath = "/Bundle", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Bundle CreateBundle([RestMessage(RestMessageFormat.SimpleJson)]Bundle bundleToInsert)
        {
            IBatchRepositoryService bundleService = ApplicationContext.Current.GetService<IBatchRepositoryService>();

            return bundleService.Insert(bundleToInsert);
        }

		/// <summary>
		/// Gets a list of patient encounters.
		/// </summary>
		/// <returns>Returns a list of patient encounters.</returns>
		[RestOperation(Method = "GET", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
		[return: RestMessage(RestMessageFormat.SimpleJson)]
		public Bundle GetAct()
		{
			var patientEncounterService = ApplicationContext.Current.GetService<IActRepositoryService>();

			var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

			int totalResults = 0;

			var results = patientEncounterService.Find(QueryExpressionParser.BuildLinqExpression<Act>(search), 0, null, out totalResults);

			return new Bundle
			{
				Count = results.Count(x => x.GetType() == typeof(IdentifiedData)),
				Item = results.Select(o => o.LoadDisplayProperties().LoadImmediateRelations()).OfType<IdentifiedData>().ToList(),
				Offset = 0,
				TotalResults = totalResults
			};
		}

        /// <summary>
        /// Gets an entity
        /// </summary>
        /// <returns>Returns an entity.</returns>
        [RestOperation(Method = "GET", UriPath = "/Entity", FaultProvider = nameof(ImsiFault))]
        //[return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetEntity()
        {
            var entityService = ApplicationContext.Current.GetService<IEntityRepositoryService>();
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            
            if (search.ContainsKey("id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Entity), Guid.Parse(search["id"].FirstOrDefault()));
                var entityId = Guid.Parse(search["id"].FirstOrDefault());
                var entity = entityService.Get(entityId, Guid.Empty);
                entity = entity.LoadDisplayProperties().LoadImmediateRelations();
                return entity;
            }
            else
            {
                {
                    int totalResults = 0,
                        offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                        count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                    IEnumerable<Entity> retVal = null;

                    // Any filter
                    if (search.ContainsKey("any") || search.ContainsKey("any[]"))
                    {

                        this.m_tracer.TraceVerbose("Freetext search: {0}", MiniImsServer.CurrentContext.Request.Url.Query);

                        var values = search.ContainsKey("any") ? search["any"] : search["any[]"];
                        // Filtes
                        var fts = ApplicationContext.Current.GetService<IFreetextSearchService>();
                        retVal = fts.Search<Entity>(values.ToArray(), offset, count, out totalResults);
                        search.Remove("any");
                        search.Remove("any[]");
                    }

                    if (search.Keys.Count(o => !o.StartsWith("_")) > 0)
                    {
                        var predicate = QueryExpressionParser.BuildLinqExpression<Entity>(search);
                        this.m_tracer.TraceVerbose("Searching Entities : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);

                        var tret = entityService.Find(predicate, offset, count, out totalResults);
                        if (retVal == null)
                            retVal = tret;
                        else
                            retVal = retVal.OfType<IIdentifiedEntity>().Intersect(tret.OfType<IIdentifiedEntity>(), new KeyComparer()).OfType<Entity>();
                    }

                    // Serialize the response
                    var itms = retVal.OfType<Entity>().Select(o => o.LoadDisplayProperties());
                    return new Bundle()
                    {
                        Item = itms.OfType<IdentifiedData>().ToList(),
                        Offset = offset,
                        Count = itms.Count(),
                        TotalResults = totalResults
                    };
                }
            }
        }

        /// <summary>
		/// Gets a patient.
		/// </summary>
		/// <returns>Returns the patient.</returns>
        [RestOperation(Method = "GET", UriPath = "/Patient", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetPatient()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var patientService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Patient), Guid.Parse(search["_id"].FirstOrDefault()));
                var patient = patientService.Get(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                patient = patient.LoadDisplayProperties().LoadImmediateRelations();
				// Ensure expanded
				//JniUtil.ExpandProperties(patient, search);
                return patient;
            }
            else
            {


                int totalResults = 0,
                    offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                    count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                IEnumerable<Patient> retVal = null;

                // Any filter
                if (search.ContainsKey("any") || search.ContainsKey("any[]"))
                {

                    this.m_tracer.TraceVerbose("Freetext search: {0}", MiniImsServer.CurrentContext.Request.Url.Query);


                    var values = search.ContainsKey("any") ? search["any"] : search["any[]"];
                    // Filtes
                    var fts = ApplicationContext.Current.GetService<IFreetextSearchService>();
                    retVal = fts.Search<Patient>(values.ToArray(), offset, count, out totalResults);
                    search.Remove("any");
                    search.Remove("any[]");
                }
                
                if(search.Keys.Count(o=>!o.StartsWith("_")) > 0)
                {
                    var predicate = QueryExpressionParser.BuildLinqExpression<Patient>(search);
                    this.m_tracer.TraceVerbose("Searching Patients : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);

                    var tret = patientService.Find(predicate, offset, count, out totalResults);
                    if (retVal == null)
                        retVal = tret;
                    else
                        retVal = retVal.OfType<IIdentifiedEntity>().Intersect(tret.OfType<IIdentifiedEntity>(), new KeyComparer()).OfType<Patient>();
                }

                // Serialize the response
                var itms = retVal.OfType<Patient>().Select(o=>o.LoadDisplayProperties());
                return new Bundle()
                    {
                        Item = itms.OfType<IdentifiedData>().ToList(),
                        Offset = offset,
                        Count = itms.Count(),
                        TotalResults = totalResults
                    };
            }
        }

        /// <summary>
		/// Gets providers.
		/// </summary>
		/// <returns>Returns a list of providers.</returns>
        [RestOperation(Method = "GET", UriPath = "/Provider" ,FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Bundle GetProvider()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var predicate = QueryExpressionParser.BuildLinqExpression<Provider>(search);

            this.m_tracer.TraceVerbose("Searching Providers : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);

            var patientService = ApplicationContext.Current.GetService<IProviderRepositoryService>();
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
		/// Get manufactured materials.
		/// </summary>
		/// <returns>Returns a list of manufactured materials.</returns>
        [RestOperation(Method = "GET", UriPath = "/ManufacturedMaterial", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Bundle GetManufacturedMaterial()
        {
			Bundle bundle = new Bundle();

			try
			{
				var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
				var predicate = QueryExpressionParser.BuildLinqExpression<ManufacturedMaterial>(search);

				this.m_tracer.TraceVerbose("Searching MMAT : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);

				var patientService = ApplicationContext.Current.GetService<IMaterialRepositoryService>();
				int totalResults = 0,
					offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
					count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;
				var manufacturedMaterials = patientService.FindManufacturedMaterial(predicate, offset, count, out totalResults);

				// Serialize the response
				bundle.Count = manufacturedMaterials.Count(x => x.GetType() == typeof(IdentifiedData));
				bundle.Item = manufacturedMaterials.Select(o => o.LoadDisplayProperties().LoadImmediateRelations()).OfType<IdentifiedData>().ToList();
				bundle.Offset = offset;
				bundle.TotalResults = totalResults;
			}
			catch (Exception e)
			{
#if DEBUG
				this.m_tracer.TraceError("{0}", e.StackTrace);
#endif
				this.m_tracer.TraceError("{0}", e.Message);
			}

			return bundle;
        }

        /// <summary>
        /// Get a template
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Act/Template", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Act GetActTemplate()
        {
            var templateString = this.GetTemplateString();
            // Load the data from the template string
            var retVal = JsonViewModelSerializer.DeSerialize<Act>(templateString);
            retVal.Key = Guid.NewGuid();
            retVal.LoadImmediateRelations();

            // Delayload
            return retVal;
        }

        /// <summary>
        /// Get a template
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Entity/Template", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Entity GetEntityTemplate()
        {
            var templateString = this.GetTemplateString();
            // Load the data from the template string
            var retVal = JsonViewModelSerializer.DeSerialize<Entity>(templateString);
            retVal.Key = Guid.NewGuid();
            retVal.LoadImmediateRelations();
            //retVal.SetDelayLoad(true);
            return retVal;
        }

        /// <summary>
        /// Get the template string
        /// </summary>
        private String GetTemplateString()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            // The template to construct
            List<String> templateId = search["templateId"];

            // Attempt to get the template definition
            var template = AndroidApplicationContext.Current.LoadedApplets.GetTemplateDefinition(templateId.First());

            // Load and replace constants
            var templateBytes = template.DefinitionContent;
            if (templateBytes == null)
                templateBytes = AndroidApplicationContext.Current.GetAppletAssetFile(AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(template.Definition));

            var templateString = Encoding.UTF8.GetString(templateBytes);
            this.m_tracer.TraceVerbose("Template {0} (Pre-Populated): {1}", templateId, templateString);
            var securityRepo = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
            var securityUser = securityRepo?.GetUser(ApplicationContext.Current.Principal.Identity);
            var userEntity = securityRepo?.FindUserEntity(o => o.SecurityUserKey == securityUser.Key).FirstOrDefault();
            templateString = templateString.Replace("{{today}}", DateTime.Today.ToString("o"))
                .Replace("{{uuid}}", Guid.NewGuid().ToString())
                .Replace("{{now}}", DateTime.Now.ToString("o"))
                .Replace("{{userId}}", securityUser.Key.ToString())
                .Replace("{{userEntityId}}", userEntity?.Key.ToString())
                .Replace("{{facilityId}}", userEntity?.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation)?.TargetEntityKey.ToString());
            this.m_tracer.TraceVerbose("Template {0} (Post-Populated): {1}", templateId, templateString);
            return templateString;
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
		/// Gets the user profile of the current user.
		/// </summary>
		/// <returns>Returns the user profile of the current user.</returns>
		[return: RestMessage(RestMessageFormat.SimpleJson)]
		[RestOperation(UriPath = "/UserEntity", Method = "GET")]
		public UserEntity GetUserProfile()
		{
			ISecurityRepositoryService securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return securityRepositoryService.GetUserEntity(ApplicationContext.Current.Principal.Identity).LoadDisplayProperties();
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

		/// <summary>
		/// Saves the user profile.
		/// </summary>
		/// <param name="user">The users modified profile information.</param>
		/// <returns>Returns the users updated profile.</returns>
		[return: RestMessage(RestMessageFormat.SimpleJson)]
		[RestOperation(UriPath = "/UserEntity", Method = "POST")]
		public UserEntity SaveUserProfile([RestMessage(RestMessageFormat.SimpleJson)] UserEntity user)
		{
			//IDataPersistenceService<UserEntity> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
			ISecurityRepositoryService securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			var userEntity = securityRepositoryService.GetUser(ApplicationContext.Current.Principal.Identity);
            if (userEntity != null && !user.SecurityUserKey.HasValue)
                user.SecurityUserKey = userEntity.Key;  
			return securityRepositoryService.SaveUserEntity(user);
		}

		/// <summary>
		/// Search places
		/// </summary>
		[RestOperation(Method = "GET", UriPath = "/Place", FaultProvider = nameof(ImsiFault))]
		[return: RestMessage(RestMessageFormat.SimpleJson)]
		public IdentifiedData SearchPlace()
		{
			var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
			var placeService = ApplicationContext.Current.GetService<IPlaceRepositoryService>();

			if (search.ContainsKey("_id"))
				return placeService.Get(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
			else
			{
				var predicate = QueryExpressionParser.BuildLinqExpression<Place>(search);
				this.m_tracer.TraceVerbose("Searching Places : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);

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
		}

		/// <summary>
		/// Updates an act.
		/// </summary>
		/// <param name="act">The act to update.</param>
		/// <returns>Returns the updated act.</returns>
		[RestOperation(Method = "PUT", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
		[return: RestMessage(RestMessageFormat.SimpleJson)]
		public Act UpdateAct([RestMessage(RestMessageFormat.SimpleJson)] Act act)
		{
			var query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

			Guid actKey = Guid.Empty;
			Guid actVersionKey = Guid.Empty;

			if (query.ContainsKey("_id") && Guid.TryParse(query["_id"][0], out actKey) && query.ContainsKey("_versionId") && Guid.TryParse(query["_versionId"][0], out actVersionKey))
			{
				if (act.Key == actKey && act.VersionKey == actVersionKey)
				{
					var actRepositoryService = ApplicationContext.Current.GetService<IActRepositoryService>();

					return actRepositoryService.Save(act);
				}
				else
				{
					throw new ArgumentException("Act not found");
				}
			}
			else
			{
				throw new ArgumentException("Act not found");
			}
		}

		/// <summary>
		/// Updates a manufactured material.
		/// </summary>
		/// <param name="manufacturedMaterial">The manufactured material to be updated.</param>
		/// <returns>Returns the updated manufactured material.</returns>
		[RestOperation(Method = "PUT", UriPath = "/ManufacturedMaterial", FaultProvider = nameof(ImsiFault))]
		[return: RestMessage(RestMessageFormat.SimpleJson)]
		public ManufacturedMaterial UpdateManufacturedMaterial([RestMessage(RestMessageFormat.SimpleJson)] ManufacturedMaterial manufacturedMaterial)
		{
			var query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

			Guid manufacturedMaterialKey = Guid.Empty;
			Guid manufacturedMaterialVersionKey = Guid.Empty;

			if (query.ContainsKey("_id") && Guid.TryParse(query["_id"][0], out manufacturedMaterialKey) && query.ContainsKey("_versionId") && Guid.TryParse(query["_versionId"][0], out manufacturedMaterialVersionKey))
			{
				if (manufacturedMaterial.Key == manufacturedMaterialKey && manufacturedMaterial.VersionKey == manufacturedMaterialVersionKey)
				{
					var manufacturedMaterialRepositoryService = ApplicationContext.Current.GetService<IMaterialRepositoryService>();

					return manufacturedMaterialRepositoryService.SaveManufacturedMaterial(manufacturedMaterial);
				}
				else
				{
					throw new ArgumentException("Manufactured Material not found");
				}
			}
			else
			{
				throw new ArgumentException("Manufactured Material not found");
			}
		}
	}

    /// <summary>
    /// Key comparion
    /// </summary>
    internal class KeyComparer : IEqualityComparer<IIdentifiedEntity>
    {
        public bool Equals(IIdentifiedEntity x, IIdentifiedEntity y)
        {
            return x.Key == y.Key;
        }

        public int GetHashCode(IIdentifiedEntity obj)
        {
            return obj.GetHashCode();
        }
    }
}