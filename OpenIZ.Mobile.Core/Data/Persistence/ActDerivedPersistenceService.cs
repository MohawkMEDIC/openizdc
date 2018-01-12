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
using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persistence service which is derived from an act
    /// </summary>
    public class ActDerivedPersistenceService<TModel, TData, TQueryResult> : IdentifiedPersistenceService<TModel, TData, TQueryResult>
        where TModel : Act, new()
        where TData : DbIdentified, new()
        where TQueryResult : DbIdentified
    {
        // act persister
        protected ActPersistenceService m_actPersister = new ActPersistenceService();

        /// <summary>
        /// Insert the specified TModel into the database
        /// </summary>
        protected override TModel InsertInternal(LocalDataContext context, TModel data)
        {
            var inserted = this.m_actPersister.InsertCoreProperties(context, data);
            data.Key = inserted.Key;
            return base.InsertInternal(context, data);
        }

        /// <summary>
        /// Update the specified TModel
        /// </summary>
        protected override TModel UpdateInternal(LocalDataContext context, TModel data)
        {
            this.m_actPersister.UpdateCoreProperties(context, data);
            return base.UpdateInternal(context, data);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        protected override TModel ObsoleteInternal(LocalDataContext context, TModel data)
        {
            var retVal = this.m_actPersister.ObsoleteCoreProperties(context, data);
            return data;
        }
    }
}
