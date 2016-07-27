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
 * Date: 2016-7-13
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
    /// Query options to control data coming back from the server
    /// </summary>
    public class IntegrationQueryOptions
    {
        /// <summary>
        /// Gets or sets the credentials
        /// </summary>
        public Credentials Credentials { get; set; }

        /// <summary>
        /// Gets or sets the If-Modified-Since header
        /// </summary>
        public DateTime? IfModifiedSince { get; set; }

        /// <summary>
        /// Gets or sets the If-None-Match
        /// </summary>
        public String IfNoneMatch { get; set; }
    }

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
        /// Instructs the integration service to retrieve the specified object
        /// </summary>
        IdentifiedData Get(Type modelType, Guid key, Guid? versionKey, IntegrationQueryOptions options = null);

        /// <summary>
        /// Gets the specified object
        /// </summary>
        TModel Get<TModel>(Guid key, Guid? versionKey, IntegrationQueryOptions options = null) where TModel : IdentifiedData;

        /// <summary>
        /// Instructs the integration service to save the specified object
        /// </summary>
        void Insert(IdentifiedData data);

        /// <summary>
        /// Instructs the integration service to save the specified object
        /// </summary>
        void Update(IdentifiedData data);

        /// <summary>
        /// Instructs the integration service to save the specified object
        /// </summary>
        void Obsolete(IdentifiedData data);

        /// <summary>
        /// Instructs the integration service to locate a specified object(s)
        /// </summary>
        Bundle Find<TModel>(Expression<Func<TModel, bool>> predicate, int offset, int? count, IntegrationQueryOptions options = null) where TModel : IdentifiedData;

        /// <summary>
        /// Determines if the integration target is available
        /// </summary>
        /// <returns></returns>
        bool IsAvailable();
    }
}
