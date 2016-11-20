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
 * Date: 2016-7-30
 */
using OpenIZ.Core.Http;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Patch;
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace OpenIZ.Mobile.Core.Interop.IMSI
{
    /// <summary>
    /// Represents an IMSI integration service which sends and retrieves data from the IMS.
    /// </summary>
    public class ImsiIntegrationService : IIntegrationService
    {

        /// <summary>
        /// Gets current credentials
        /// </summary>
        private Credentials GetCredentials(IRestClient client)
        {
            var appConfig = ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>();

            // TODO: Clean this up - Login as device account
            if (!AuthenticationContext.Current.Principal.Identity.IsAuthenticated ||
                DateTime.Parse((AuthenticationContext.Current.Principal as ClaimsPrincipal)?.FindClaim(ClaimTypes.Expiration)?.Value ?? "0001-01-01") < DateTime.Now)
                AuthenticationContext.Current = new AuthenticationContext(ApplicationContext.Current.GetService<IIdentityProviderService>().Authenticate(appConfig.DeviceName, appConfig.DeviceSecret));
            return client.Description.Binding.Security.CredentialProvider.GetCredentials(AuthenticationContext.Current.Principal);
        }
        /// <summary>
        /// Gets the specified model object
        /// </summary>
        public Bundle Find(Type modelType, NameValueCollection filter, int offset, int? count, IntegrationQueryOptions options = null)
        {
            try
            {
                var method = this.GetType().GetRuntimeMethod("Find", new Type[] { typeof(NameValueCollection), typeof(int), typeof(int?), typeof(IntegrationQueryOptions) }).MakeGenericMethod(new Type[] { modelType });
                return method.Invoke(this, new object[] { filter, offset, count, options }) as Bundle;
            }
            catch (TargetInvocationException e)
            {
                throw Activator.CreateInstance(e.InnerException.GetType(), "Error performing action", e) as Exception;
            }

        }

        /// <summary>
        /// Finds the specified model
        /// </summary>
        public Bundle Find<TModel>(NameValueCollection filter, int offset, int? count, IntegrationQueryOptions options = null) where TModel : IdentifiedData
        {
            var predicate = QueryExpressionParser.BuildLinqExpression<TModel>(filter);
            return this.Find<TModel>(predicate, offset, count, options);
        }

        /// <summary>
        /// Finds the specified model
        /// </summary>
        public Bundle Find<TModel>(Expression<Func<TModel, bool>> predicate, int offset, int? count, IntegrationQueryOptions options = null) where TModel : IdentifiedData
        {
            try
            {
                ImsiServiceClient client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));
                client.Client.Requesting += (o, e) =>
                {
                    if (options == null) return;
                    else if (options.IfModifiedSince.HasValue)
                        e.AdditionalHeaders[HttpRequestHeader.IfModifiedSince] = options.IfModifiedSince.Value.ToString();
                    else if (!String.IsNullOrEmpty(options.IfNoneMatch))
                        e.AdditionalHeaders[HttpRequestHeader.IfNoneMatch] = options.IfNoneMatch;
                };
                client.Client.Credentials = this.GetCredentials(client.Client);
                if (options.Timeout.HasValue)
                    client.Client.Description.Endpoint[0].Timeout = options.Timeout.Value;

                var retVal = client.Query<TModel>(predicate, offset, count);
                //retVal?.Reconstitute();
                return retVal;
            }
            catch (TargetInvocationException e)
            {
                throw Activator.CreateInstance(e.InnerException.GetType(), "Error performing action", e) as Exception;
            }

        }

        /// <summary>
        /// Gets the specified model object
        /// </summary>
        public IdentifiedData Get(Type modelType, Guid key, Guid? version, IntegrationQueryOptions options = null)
        {
            try
            {
                var method = this.GetType().GetRuntimeMethod("Get", new Type[] { typeof(Guid), typeof(Guid?), typeof(IntegrationQueryOptions) }).MakeGenericMethod(new Type[] { modelType });
                return method.Invoke(this, new object[] { key, version, options }) as IdentifiedData;
            }
            catch (TargetInvocationException e)
            {
                throw Activator.CreateInstance(e.InnerException.GetType(), "Error performing action", e) as Exception;
            }

        }

        /// <summary>
        /// Gets a specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of model data to retrieve.</typeparam>
        /// <param name="key">The key of the model.</param>
        /// <param name="versionKey">The version key of the model.</param>
        /// <param name="options">The integrations query options.</param>
        /// <returns>Returns a model.</returns>
        public TModel Get<TModel>(Guid key, Guid? versionKey, IntegrationQueryOptions options = null) where TModel : IdentifiedData
        {
            try
            {
                ImsiServiceClient client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));
                client.Client.Requesting += (o, e) =>
                {
                    if (options == null) return;
                    else if (options.IfModifiedSince.HasValue)
                        e.AdditionalHeaders[HttpRequestHeader.IfModifiedSince] = options.IfModifiedSince.Value.ToString();
                    else if (!String.IsNullOrEmpty(options.IfNoneMatch))
                        e.AdditionalHeaders[HttpRequestHeader.IfNoneMatch] = options.IfNoneMatch;
                };
                client.Client.Credentials = this.GetCredentials(client.Client);

                var retVal = client.Get<TModel>(key, versionKey);

                if (retVal is Bundle)
                {
                    (retVal as Bundle)?.Reconstitute();
                    return (retVal as Bundle).Entry as TModel;
                }
                else
                    return retVal as TModel;
            }
            catch (TargetInvocationException e)
            {
                throw Activator.CreateInstance(e.InnerException.GetType(), "Error performing action", e) as Exception;
            }

        }

        /// <summary>
        /// Inserts specified data.
        /// </summary>
        /// <param name="data">The data to be inserted.</param>
        public void Insert(IdentifiedData data)
        {
            try
            {
                ImsiServiceClient client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));

                client.Client.Credentials = this.GetCredentials(client.Client);

                var method = typeof(ImsiServiceClient).GetRuntimeMethods().FirstOrDefault(o => o.Name == "Create" && o.GetParameters().Length == 1);
                method = method.MakeGenericMethod(new Type[] { data.GetType() });
                method.Invoke(client, new object[] { data });
            }
            catch (TargetInvocationException e)
            {
                throw Activator.CreateInstance(e.InnerException.GetType(), "Error performing action", e) as Exception;
            }
        }

        /// <summary>
        /// Determines whether the network is available.
        /// </summary>
        /// <returns>Returns true if the network is available.</returns>
        public bool IsAvailable()
        {
            try
            {
                var restClient = ApplicationContext.Current.GetRestClient("imsi");

                ImsiServiceClient client = new ImsiServiceClient(restClient);
                client.Client.Credentials = this.GetCredentials(client.Client);

                var networkInformationService = ApplicationContext.Current.GetService<INetworkInformationService>();

                return networkInformationService.IsNetworkAvailable &&
                    client.Client.Options<Object>("/Patient") == null;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Obsoletes specified data.
        /// </summary>
        /// <param name="data">The data to be obsoleted.</param>
        public void Obsolete(IdentifiedData data)
        {
            try
            {
                ImsiServiceClient client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));
                client.Client.Credentials = this.GetCredentials(client.Client);
                var method = typeof(ImsiServiceClient).GetRuntimeMethods().FirstOrDefault(o => o.Name == "Obsolete" && o.GetParameters().Length == 1);
                method.MakeGenericMethod(new Type[] { data.GetType() });
                method.Invoke(this, new object[] { data });
            }
            catch (TargetInvocationException e)
            {
                throw Activator.CreateInstance(e.InnerException.GetType(), "Error performing action", e) as Exception;
            }

        }

        /// <summary>
        /// Updates specified data.
        /// </summary>
        /// <param name="data">The data to be updated.</param>
        public void Update(IdentifiedData data)
        {
            try
            {
                ImsiServiceClient client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));
                client.Client.Credentials = this.GetCredentials(client.Client);

                if (data is Patch)
                {
                    var patch = data as Patch;
                    var newUuid = client.Patch(patch);

                    // Update the ETag of the current version
                    var idp = typeof(IDataPersistenceService<>).MakeGenericType(patch.AppliesTo.GetType());
                    var idpService = ApplicationContext.Current.GetService(idp);
                    if (idpService != null)
                    {
                        var updateTarget = idp.GetRuntimeMethod("Get", new Type[] { typeof(Guid) }).Invoke(idpService, new object[] { patch.AppliesTo.Key }) as IVersionedEntity;
                        updateTarget.VersionKey = newUuid;
                        idp.GetRuntimeMethod("Update", new Type[] { patch.AppliesTo.GetType() }).Invoke(idpService, new object[] { updateTarget });
                    }
                }
                else // regular update 
                {
                    var method = typeof(ImsiServiceClient).GetRuntimeMethods().FirstOrDefault(o => o.Name == "Update" && o.GetParameters().Length == 1);
                    method.MakeGenericMethod(new Type[] { data.GetType() });
                    method.Invoke(this, new object[] { data });
                }
            }
            catch (TargetInvocationException e)
            {
                throw Activator.CreateInstance(e.InnerException.GetType(), "Error performing action", e) as Exception;
            }

        }
    }
}