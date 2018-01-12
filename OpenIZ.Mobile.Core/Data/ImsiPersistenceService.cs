/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Interop;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using OpenIZ.Core.Http;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Security;
using System.Net;
using System.Xml.Serialization;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Mobile.Core.Data
{
    /// <summary>
    /// Represents a persistence service which uses the IMSI only in online mode
    /// </summary>
    public class ImsiPersistenceService
    {
       
        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(ImsiPersistenceService));

        // Constructor
        public ImsiPersistenceService()
        {
            var appSection = ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>();

            // Now iterate through the map file and ensure we have all the mappings, if a class does not exist create it
            try
            {

                foreach (var itm in typeof(IdentifiedData).GetTypeInfo().Assembly.ExportedTypes.Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && !o.GetTypeInfo().IsAbstract))
                {

                    var rootElement = itm.GetTypeInfo().GetCustomAttribute<XmlRootAttribute>();
                    if (rootElement == null) continue;
                    // Is there a persistence service?
                    var idpType = typeof(IDataPersistenceService<>);
                    idpType = idpType.MakeGenericType(itm);

                    
                    this.m_tracer.TraceVerbose("Creating persister {0}", itm);

                    // Is the model class a Versioned entity?
                    var pclass = typeof(GenericImsiPersister<>);
                    pclass = pclass.MakeGenericType(itm);
                    appSection.Services.Add(Activator.CreateInstance(pclass));
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error initializing local persistence: {0}", e);
                throw e;
            }
        }


        /// <summary>
        /// Generic versioned persister service for any non-customized persister
        /// </summary>
        internal class GenericImsiPersister<TModel> : IDataPersistenceService<TModel>
            where TModel : IdentifiedData, new()
        {
            // Service client
            private ImsiServiceClient m_client = null;

            public GenericImsiPersister()
            {
                this.m_client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));
                this.m_client.Client.Requesting += (o, e) =>
                {
                    e.Query.Add("_expand", new List<String>() {
                        "typeConcept",
                        "address.use",
                        "name.use"
                    });
                };

            }

            private IPrincipal m_cachedCredential = null;

            /// <summary>
            /// Gets current credentials
            /// </summary>
            private Credentials GetCredentials()
            {
                var appConfig = ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>();

                AuthenticationContext.Current = new AuthenticationContext(this.m_cachedCredential ?? AuthenticationContext.Current.Principal);

                // TODO: Clean this up - Login as device account
                if (!AuthenticationContext.Current.Principal.Identity.IsAuthenticated ||
                    ((AuthenticationContext.Current.Principal as ClaimsPrincipal)?.FindClaim(ClaimTypes.Expiration)?.AsDateTime().ToLocalTime() ?? DateTime.MinValue) < DateTime.Now)
                    AuthenticationContext.Current = new AuthenticationContext(ApplicationContext.Current.GetService<IIdentityProviderService>().Authenticate(appConfig.DeviceName, appConfig.DeviceSecret));
                this.m_cachedCredential = AuthenticationContext.Current.Principal;
                return this.m_client.Client.Description.Binding.Security.CredentialProvider.GetCredentials(AuthenticationContext.Current.Principal);
            }

            /// <summary>
            /// Inserted data
            /// </summary>
            public event EventHandler<DataPersistenceEventArgs<TModel>> Inserted;

            /// <summary>
            /// Inserting data
            /// </summary>
            public event EventHandler<DataPersistencePreEventArgs<TModel>> Inserting;

            /// <summary>
            /// Obsoleted data
            /// </summary>
            public event EventHandler<DataPersistenceEventArgs<TModel>> Obsoleted;

            /// <summary>
            /// Obsoleting data
            /// </summary>
            public event EventHandler<DataPersistencePreEventArgs<TModel>> Obsoleting;

            /// <summary>
            /// Queried data
            /// </summary>
            public event EventHandler<DataQueryEventArgsBase<TModel>> Queried;

            /// <summary>
            /// Querying data
            /// </summary>
            public event EventHandler<DataQueryEventArgsBase<TModel>> Querying;

            /// <summary>
            /// Updated data
            /// </summary>
            public event EventHandler<DataPersistenceEventArgs<TModel>> Updated;

            /// <summary>
            /// Updating data
            /// </summary>
            public event EventHandler<DataPersistencePreEventArgs<TModel>> Updating;

            /// <summary>
            /// Count the number of items
            /// </summary>
            public int Count(Expression<Func<TModel, bool>> predicate)
            {
                this.GetCredentials();
                var retVal = this.m_client.Query(predicate);
                return (retVal as Bundle)?.TotalResults ?? ((retVal as IdentifiedData != null) ? 1 : 0);
            }

            /// <summary>
            /// Gets the specified item
            /// </summary>
            public TModel Get(Guid key)
            {
                this.GetCredentials();

                try
                {
                    var existing = ApplicationContext.Current.GetService<IDataCachingService>()?.GetCacheItem(key);
                    if (existing == null)
                    {
                        existing = this.m_client.Get<TModel>(key, null) as TModel;
                        ApplicationContext.Current.GetService<IDataCachingService>()?.Add(existing as IdentifiedData);
                    }
                    return (TModel)existing;
                }
                catch (WebException)
                {
                    return default(TModel);
                }
            }

            /// <summary>
            /// Gets the specified object
            /// </summary>
            object IDataPersistenceService.Get(Guid id)
            {
                return this.Get(id) as TModel;
            }

            /// <summary>
            /// Inserts the specified data
            /// </summary>
            public object Insert(object data)
            {
                return this.Insert(data as TModel);
            }

            /// <summary>
            /// Inserts the specified typed data
            /// </summary>
            public TModel Insert(TModel data)
            {
                this.GetCredentials();

                var retVal = this.m_client.Create(data);
                return retVal;
            }

            /// <summary>
            /// Obsoletes the specified data
            /// </summary>
            public object Obsolete(object data)
            {
                return this.Obsolete(data as TModel);
            }

            /// <summary>
            /// Obsoletes the specified data
            /// </summary>
            public TModel Obsolete(TModel data)
            {
                this.GetCredentials();

                var retVal = this.m_client.Obsolete(data);
                return retVal;
            }

            /// <summary>
            /// Query the specified data
            /// </summary>
            public IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query)
            {
                int t;
                return this.Query(query, 0, null, out t, Guid.Empty);
            }

            /// <summary>
            /// Executes the specified named query
            /// </summary>
            public IEnumerable<TModel> Query(string queryName, IDictionary<string, object> parameters)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Query the specifie data
            /// </summary>
            public IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query, int offset, int? count, out int totalResults, Guid queryId)
            {
                this.GetCredentials();

                try
                {
                    var data = this.m_client.Query(query, offset, count, false, queryId);
                    (data as Bundle)?.Reconstitute();
                    offset = (data as Bundle)?.Offset ?? offset;
                    count = (data as Bundle)?.Count ?? count;
                    totalResults = (data as Bundle)?.TotalResults ?? 1;


                    data.Item.AsParallel().ForAll(o =>
                    {
                        ApplicationContext.Current.GetService<IDataCachingService>()?.Add(o as IdentifiedData);
                    });

                    return (data as Bundle)?.Item.OfType<TModel>() ?? new List<TModel>() { data as TModel };
                }
                catch (WebException)
                {
                    totalResults = 0;
                    return new List<TModel>();
                }

            }

            /// <summary>
            /// Executes the specified named query
            /// </summary>
            public IEnumerable<TModel> Query(string queryName, IDictionary<string, object> parameters, int offset, int? count, out int totalResults, Guid queryId)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Updates the specified data
            /// </summary>
            public object Update(object data)
            {
                return this.Update(data as TModel);
            }

            /// <summary>
            /// Update the specified data
            /// </summary>
            public TModel Update(TModel data)
            {
                this.GetCredentials();

                var retVal = this.m_client.Update(data);
                return retVal;
            }

            /// <summary>
            /// Query the specified object
            /// </summary>
            public IEnumerable Query(Expression query, int offset, int? count, out int totalResults)
            {
                return this.Query((Expression<Func<TModel, bool>>)query, offset, count, out totalResults, Guid.Empty);
            }

            /// <summary>
            /// Query fast (not implemented)
            /// </summary>
            public IEnumerable<TModel> QueryFast(Expression<Func<TModel, bool>> query, int offset, int? count, out int totalResults, Guid queryId)
            {
                return this.Query((Expression<Func<TModel, bool>>)query, offset, count, out totalResults, Guid.Empty);
            }

            /// <summary>
            /// Expliciry load
            /// </summary>
            public IEnumerable<TModel> QueryExplicitLoad(Expression<Func<TModel, bool>> query, int offset, int? count, out int totalResults, Guid queryId, IEnumerable<string> expandProperties)
            {
                throw new NotImplementedException();
            }
        }
    }
}