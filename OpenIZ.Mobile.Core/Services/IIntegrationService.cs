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
using OpenIZ.Core.Model;
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
        /// Instructs the integration service to retrieve the specified object
        /// </summary>
        IdentifiedData Get(Type modelType, Guid key, Guid? versionKey);

        /// <summary>
        /// Gets the specified object
        /// </summary>
        TModel Get<TModel>(Guid key, Guid? versionKey) where TModel : IdentifiedData;

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
        IdentifiedData Find<TModel>(Expression<Func<TModel, bool>> predicate, int offset, int? count) where TModel : IdentifiedData;

        /// <summary>
        /// Determines if the integration target is available
        /// </summary>
        /// <returns></returns>
        bool IsAvailable();
    }
}
