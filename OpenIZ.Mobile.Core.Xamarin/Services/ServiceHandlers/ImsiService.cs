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
using System.Collections.Generic;

using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
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
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Core.Model.Interfaces;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using OpenIZ.Mobile.Core.Security;
using System.Reflection;
using OpenIZ.Core.Applets.ViewModel.Json;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents an IMS service handler
    /// </summary>
    [RestService("/__ims")]
    public partial class ImsiService
    {

        // UTF8 BOM
        private readonly String c_utf8bom = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        // Tracer 
        private Tracer m_tracer = Tracer.GetTracer(typeof(ImsiService));

        // View model serliazer
        private JsonViewModelSerializer m_serializer = new JsonViewModelSerializer();
        
        /// <summary>
        /// Creates a bundle.
        /// </summary>
        /// <param name="bundleToInsert">The bundle to be inserted.</param>
        /// <returns>Returns the inserted bundle.</returns>
        [RestOperation(Method = "POST", UriPath = "/Bundle", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Bundle CreateBundle([RestMessage(RestMessageFormat.SimpleJson)]Bundle bundleToInsert)
        {
            IBatchRepositoryService bundleService = ApplicationContext.Current.GetService<IBatchRepositoryService>();
            return bundleService.Insert(bundleToInsert);
        }

        /// <summary>
        /// Gets an entity
        /// </summary>
        /// <returns>Returns an entity.</returns>
        [RestOperation(Method = "GET", UriPath = "/Entity", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetEntity()
        {
            var entityService = ApplicationContext.Current.GetService<IEntityRepositoryService>();
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Entity), Guid.Parse(search["_id"].FirstOrDefault()));
                var entityId = Guid.Parse(search["_id"].FirstOrDefault());
                var entity = entityService.Get(entityId, Guid.Empty);
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
                    return new Bundle()
                    {
                        Item = retVal.OfType<IdentifiedData>().ToList(),
                        Offset = offset,
                        Count = retVal.Count(),
                        TotalResults = totalResults
                    };
                }
            }
        }

        
        /// <summary>
		/// Gets providers.
		/// </summary>
		/// <returns>Returns a list of providers.</returns>
        [RestOperation(Method = "GET", UriPath = "/Provider", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetProvider()
        {

            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var providerService = ApplicationContext.Current.GetService<IProviderRepositoryService>();

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Provider), Guid.Parse(search["_id"].FirstOrDefault()));
                var provider = providerService.Get(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                // Ensure expanded
                //JniUtil.ExpandProperties(patient, search);
                return provider;
            }
            else
            {
                var predicate = QueryExpressionParser.BuildLinqExpression<Provider>(search);

                int totalResults = 0,
                  offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                  count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;


                this.m_tracer.TraceVerbose("Searching Providers : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);

                var retVal = providerService.Find(predicate, offset, count, out totalResults);

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
		/// Get manufactured materials.
		/// </summary>
		/// <returns>Returns a list of manufactured materials.</returns>
        [RestOperation(Method = "GET", UriPath = "/ManufacturedMaterial", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.Login)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Bundle GetManufacturedMaterial()
        {
            var bundle = new Bundle();

            try
            {
                var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
                var predicate = QueryExpressionParser.BuildLinqExpression<ManufacturedMaterial>(search);
                var manufacturedMaterialService = ApplicationContext.Current.GetService<IMaterialRepositoryService>();

                if (search.ContainsKey("id"))
                {
                    // Force load from DB
                    MemoryCache.Current.RemoveObject(typeof(ManufacturedMaterial), Guid.Parse(search["id"].FirstOrDefault()));

                    var manufacturedMaterialId = Guid.Parse(search["id"].FirstOrDefault());
                    var manufacturedMaterial = manufacturedMaterialService.GetManufacturedMaterial(manufacturedMaterialId, Guid.Empty);

                    manufacturedMaterial = manufacturedMaterial;

                    if (manufacturedMaterial != null)
                    {
                        bundle.Count = 1;
                        bundle.Item = new List<IdentifiedData> { manufacturedMaterial };
                        bundle.Reconstitute();
                        bundle.TotalResults = 1;
                    }

                    return bundle;
                }

                this.m_tracer.TraceVerbose("Searching MMAT : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);


                int totalResults = 0,
                    offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                    count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                var manufacturedMaterials = manufacturedMaterialService.FindManufacturedMaterial(predicate, offset, count, out totalResults);

                // Serialize the response
                bundle.Count = manufacturedMaterials.Count(x => x.GetType() == typeof(IdentifiedData));
                bundle.Item = manufacturedMaterials.OfType<IdentifiedData>().ToList();
                bundle.Offset = offset;
                bundle.TotalResults = totalResults;
            }
            catch (Exception e)
            {
#if DEBUG
                this.m_tracer.TraceError("{0}", e);
#endif
                this.m_tracer.TraceError("{0}", e.Message);
            }

            return bundle;
        }

        /// <summary>
        /// Get a template
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Act/Template", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.Login)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Act GetActTemplate()
        {
            var templateString = this.GetTemplateString();
            // Load the data from the template string
            var retVal = this.m_serializer.DeSerialize<Act>(templateString);
            retVal.Key = Guid.NewGuid();
            // Delayload
            return retVal;
        }

        /// <summary>
        /// Get a template
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Entity/Template", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.Login)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Entity GetEntityTemplate()
        {
            var templateString = this.GetTemplateString();
            // Load the data from the template string
            var retVal = this.m_serializer.DeSerialize<Entity>(templateString);
            retVal.Key = Guid.NewGuid();
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
            var template = XamarinApplicationContext.Current.LoadedApplets.GetTemplateDefinition(templateId.First());

            // Load and replace constants
            var templateBytes = template.DefinitionContent;
            if (templateBytes == null)
                templateBytes = XamarinApplicationContext.Current.ResolveAppletAsset(XamarinApplicationContext.Current.LoadedApplets.ResolveAsset(template.Definition)) as byte[];

            var templateString = Encoding.UTF8.GetString(templateBytes);

            //if (templateString.StartsWith(c_utf8bom))
            //    templateString = templateString.Remove(0, c_utf8bom.Length);

            this.m_tracer.TraceVerbose("Template {0} (Pre-Populated): {1}", templateId, templateString);
            var securityRepo = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
            var securityUser = securityRepo?.GetUser(AuthenticationContext.Current.Principal.Identity);
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
        /// Gets the user profile of the current user.
        /// </summary>
        /// <returns>Returns the user profile of the current user.</returns>
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        [RestOperation(UriPath = "/UserEntity", Method = "GET")]
        [Demand(PolicyIdentifiers.Login)]
        public IdentifiedData GetUserEntity()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var securityService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Provider), Guid.Parse(search["_id"].FirstOrDefault()));
                var provider = securityService.GetUserEntity(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                // Ensure expanded
                //JniUtil.ExpandProperties(patient, search);
                return provider;
            }
            else
            {
                var predicate = QueryExpressionParser.BuildLinqExpression<UserEntity>(search);

                int totalResults = 0,
                  offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                  count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                this.m_tracer.TraceVerbose("Searching User Entity : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);

                var retVal = securityService.FindUserEntity(predicate, offset, count, out totalResults);

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
        /// Handle a fault
        /// </summary>
        public ErrorResult ImsiFault(Exception e)
        {
            return new ErrorResult()
            {
                Error = e is TargetInvocationException ? e.InnerException.Message : e.Message,
                ErrorDescription = e.InnerException?.ToString(),
                ErrorType = e.GetType().Name
            };
        }

        /// <summary>
        /// Saves the user profile.
        /// </summary>
        /// <param name="user">The users modified profile information.</param>
        /// <returns>Returns the users updated profile.</returns>
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        [RestOperation(UriPath = "/UserEntity", Method = "PUT")]
        [Demand(PolicyIdentifiers.Login)]
        public UserEntity UpdateUserEntity([RestMessage(RestMessageFormat.SimpleJson)] UserEntity user)
        {
            var query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            ISecurityRepositoryService securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
            AuthenticationContext.Current?.Session?.ClearCached();
            //IDataPersistenceService<UserEntity> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            return securityRepositoryService.SaveUserEntity(user);
        }

        /// <summary>
        /// Search places
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Place", FaultProvider = nameof(ImsiFault))]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetPlace()
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
        /// Updates a manufactured material.
        /// </summary>
        /// <param name="manufacturedMaterial">The manufactured material to be updated.</param>
        /// <returns>Returns the updated manufactured material.</returns>
        [RestOperation(Method = "PUT", UriPath = "/ManufacturedMaterial", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.Login)]
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