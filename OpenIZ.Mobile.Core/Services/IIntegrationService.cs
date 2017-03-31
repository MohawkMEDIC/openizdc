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
 * User: fyfej
 * Date: 2016-11-14
 */
using OpenIZ.Core.Http;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Represents an integration service which is responsible for sending and
    /// pulling data to/from remote sources
    /// </summary>
    public interface IIntegrationService
    {

        /// <summary>
        /// Find the specified filtered object
        /// </summary>
        Bundle Find(Type modelType, NameValueCollection filter, int offset, int? count, IntegrationQueryOptions options = null);

        /// <summary>
        /// Find the specified filtered object
        /// </summary>
        Bundle Find<TModel>(NameValueCollection filter, int offset, int? count, IntegrationQueryOptions options = null) where TModel : IdentifiedData;

		/// <summary>
		/// Instructs the integration service to locate a specified object(s)
		/// </summary>
		Bundle Find<TModel>(Expression<Func<TModel, bool>> predicate, int offset, int? count, IntegrationQueryOptions options = null) where TModel : IdentifiedData;

		/// <summary>
		/// Instructs the integration service to retrieve the specified object
		/// </summary>
		IdentifiedData Get(Type modelType, Guid key, Guid? versionKey, IntegrationQueryOptions options = null);

		/// <summary>
		/// Gets a specified model.
		/// </summary>
		/// <typeparam name="TModel">The type of model data to retrieve.</typeparam>
		/// <param name="key">The key of the model.</param>
		/// <param name="versionKey">The version key of the model.</param>
		/// <param name="options">The integrations query options.</param>
		/// <returns>Returns a model.</returns>
		TModel Get<TModel>(Guid key, Guid? versionKey, IntegrationQueryOptions options = null) where TModel : IdentifiedData;

		/// <summary>
		/// Inserts specified data.
		/// </summary>
		/// <param name="data">The data to be inserted.</param>
		void Insert(IdentifiedData data);

		/// <summary>
		/// Determines whether the network is available.
		/// </summary>
		/// <returns>Returns true if the network is available.</returns>
		bool IsAvailable();

		/// <summary>
		/// Obsoletes specified data.
		/// </summary>
		/// <param name="data">The data to be obsoleted.</param>
		void Obsolete(IdentifiedData data, bool forceObsolete = false);

		/// <summary>
		/// Updates specified data.
		/// </summary>
		/// <param name="data">The data to be updated.</param>
        /// <param name="forceUpdate">When true, indicates that update should not do a safety check</param>
		void Update(IdentifiedData data, bool forceUpdate = false);
    }

    /// <summary>
    /// Represents the clinical integration service
    /// </summary>
    public interface IClinicalIntegrationService : IIntegrationService
    {

    }

    /// <summary>
    /// Admin integration service
    /// </summary>
    public interface IAdministrationIntegrationService : IIntegrationService
    {

    }
}
