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
 * Date: 2017-1-16
 */
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Free text search service
    /// </summary>
    public interface IFreetextSearchService
    {

        /// <summary>
        /// Performs a full index scan
        /// </summary>
        bool Index();

        /// <summary>
        /// Performs a freetext search 
        /// </summary>
        IEnumerable<TEntity> Search<TEntity>(String term, int offset, int? count, out int totalResults) where TEntity : Entity;
        /// <summary>
        /// Search based on tokens
        /// </summary>
        IEnumerable<TEntity> Search<TEntity>(String[] term, int offset, int? count, out int totalResults) where TEntity : Entity;

    }
}
