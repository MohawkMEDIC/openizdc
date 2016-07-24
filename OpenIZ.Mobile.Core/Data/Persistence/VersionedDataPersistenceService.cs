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
 * Date: 2016-6-14
 */
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Versioned domain data
    /// </summary>
    public abstract class VersionedDataPersistenceService<TModel, TDomain> : BaseDataPersistenceService<TModel, TDomain> 
        where TDomain : DbVersionedData, new() 
        where TModel : VersionedEntityData<TModel>, new()
    {

        /// <summary>
        /// Insert the data
        /// </summary>
        public override TModel Insert(SQLiteConnection context, TModel data)
        {
            data.VersionKey = data.VersionKey == Guid.Empty || !data.VersionKey.HasValue ? Guid.NewGuid() : data.VersionKey ;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the data with new version information
        /// </summary>
        public override TModel Update(SQLiteConnection context, TModel data)
        {
            data.PreviousVersionKey = data.VersionKey;
            data.VersionKey = Guid.NewGuid();
            return base.Update(context, data);
        }

        /// <summary>
        /// Obsolete the specified data
        /// </summary>
        public override TModel Obsolete(SQLiteConnection context, TModel data)
        {
            data.PreviousVersionKey = data.VersionKey;
            data.VersionKey = Guid.NewGuid();
            return base.Obsolete(context, data);
        }
    }
}
