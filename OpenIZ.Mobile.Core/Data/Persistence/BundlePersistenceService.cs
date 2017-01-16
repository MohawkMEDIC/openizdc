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
using OpenIZ.Core.Model.Collection;
using OpenIZ.Mobile.Core.Data.Model.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using OpenIZ.Mobile.Core.Services;
using System.Reflection;
using System.Linq.Expressions;
using OpenIZ.Core.Model;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a bundle persistence service
    /// </summary>
    public class BundlePersistenceService : IdentifiedPersistenceService<Bundle, DbBundle>
    {
        /// <summary>
        /// Cannot query for bundles
        /// </summary>
        public override IEnumerable<Bundle> Query(SQLiteConnectionWithLock context, Expression<Func<Bundle, bool>> query, int offset, int count, out int totalResults, Guid queryId)
        {
            totalResults = 0;
            return new List<Bundle>();
        }

        /// <summary>
        /// Connot query bundles
        /// </summary>
        public override IEnumerable<Bundle> Query(SQLiteConnectionWithLock context, string storedQueryName, IDictionary<string, object> parms, int offset, int count, out int totalResults, Guid queryId)
        {
            totalResults = 0;
            return new List<Bundle>();

        }

        /// <summary>
        /// Insert the bundle
        /// </summary>
        public override Bundle Insert(SQLiteConnectionWithLock context, Bundle data)
        {
            foreach (var itm in data.Item)
            {
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
                var svc = ApplicationContext.Current.GetService(idp);
                String method = "Insert";
                if (itm.TryGetExisting(context) != null)
                    method = "Update";
                var mi = svc.GetType().GetRuntimeMethod(method, new Type[] { typeof(SQLiteConnectionWithLock), itm.GetType() });
                itm.CopyObjectData(mi.Invoke(svc, new object[] { context, itm }));
            }
            return data;
        }

        /// <summary>
        /// Update everything in the bundle
        /// </summary>
        public override Bundle Update(SQLiteConnectionWithLock context, Bundle data)
        {
            foreach (var itm in data.Item)
            {
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
                var mi = idp.GetRuntimeMethod("Update", new Type[] { typeof(SQLiteConnectionWithLock), itm.GetType() });
                itm.CopyObjectData(mi.Invoke(ApplicationContext.Current.GetService(idp), new object[] { context, itm }));
            }
            return data;
        }

        /// <summary>
        /// Obsolete everything in the bundle
        /// </summary>
        public override Bundle Obsolete(SQLiteConnectionWithLock context, Bundle data)
        {
            foreach (var itm in data.Item)
            {
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
                var mi = idp.GetRuntimeMethod("Obsolete", new Type[] { typeof(SQLiteConnectionWithLock), itm.GetType() });
                itm.CopyObjectData(mi.Invoke(ApplicationContext.Current.GetService(idp), new object[] { context, itm }));
            }
            return data;
        }

    }
}
