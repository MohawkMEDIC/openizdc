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
 * User: justi
 * Date: 2017-2-4
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
using OpenIZ.Mobile.Core.Resources;
using System.Diagnostics;
using SQLite.Net.Interop;
using OpenIZ.Mobile.Core.Exceptions;

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
        protected override IEnumerable<Bundle> QueryInternal(LocalDataContext context, Expression<Func<Bundle, bool>> query, int offset, int count, out int totalResults, Guid queryId, bool countResults)
        {
            totalResults = 0;
            return new List<Bundle>();
        }

        /// <summary>
        /// Connot query bundles
        /// </summary>
        protected override IEnumerable<Bundle> QueryInternal(LocalDataContext context, string storedQueryName, IDictionary<string, object> parms, int offset, int count, out int totalResults, Guid queryId, bool countResults)
        {
            totalResults = 0;
            return new List<Bundle>();

        }

        /// <summary>
        /// Bundles are special, they may be written on the current connection
        /// or in memory
        /// </summary>
        public override Bundle Insert(Bundle data)
        {
            // first, are we just doing a normal insert?
            if (data.Item.Count <= 100)
                return base.Insert(data);
            else
            { // It is cheaper to open a mem-db and let other threads access the main db for the time being

                base.FireInserting(new DataPersistencePreEventArgs<Bundle>(data));

                // Memory connection
                using (var memConnection = new SQLiteConnectionWithLock(ApplicationContext.Current.GetService<ISQLitePlatform>(), new SQLiteConnectionString(":memory:", true)))
                {
                    try
                    {
                        // We want to apply the initial schema
                        new OpenIZ.Mobile.Core.Configuration.Data.Migrations.InitialCatalog().Install(memConnection, true);

                        // We insert in the memcontext now
                        using (var memContext = new LocalDataContext(memConnection))
                            this.InsertInternal(memContext, data);

                        var columnMapping = memConnection.TableMappings.Where(o => o.MappedType.Namespace.StartsWith("OpenIZ")).ToList();

                        // Now we attach our local file based DB by requesting a lock so nobody else touches it!
                        using (var fileContext = this.CreateConnection())
                        using (fileContext.Connection.Lock())
                        {
                            memConnection.Execute($"ATTACH DATABASE '{ApplicationContext.Current.Configuration.GetConnectionString("openIzData").Value}' AS file_db");

                            try
                            {
                                memConnection.BeginTransaction();

                                // Copy copy!!!
                                foreach (var tbl in columnMapping)
                                {
                                    // insert new first
                                    memConnection.Execute($"INSERT OR REPLACE INTO file_db.{tbl.TableName} SELECT * FROM {tbl.TableName}");
                                }

                                memConnection.Commit();
                            }
                            catch
                            {
                                memConnection.Rollback();
                                throw;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceError("Error inserting bundle: {0}", e);
                        throw new LocalPersistenceException(Synchronization.Model.DataOperationType.Insert, data, e);
                    }
                }

                base.FireInserted(new DataPersistenceEventArgs<Bundle>(data));
                return data;
            }
        }

        /// <summary>
        /// Insert the bundle
        /// </summary>
        protected override Bundle InsertInternal(LocalDataContext context, Bundle data)
        {
            // Prepare to insert a bundle
            for (int i = 0; i < data.Item.Count; i++)
            {
                var itm = data.Item[i];
#if SHOW_STATUS || PERFMON
                Stopwatch itmSw = new Stopwatch();
                itmSw.Start();
#endif
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
                var svc = ApplicationContext.Current.GetService(idp);
                if (svc == null) continue; // can't insert
                String method = "Insert";
                if (itm.TryGetExisting(context) != null)
                    method = "Update";
                var mi = svc.GetType().GetRuntimeMethod(method, new Type[] { typeof(LocalDataContext), itm.GetType() });
                data.Item[i] = mi.Invoke(svc, new object[] { context, itm }) as IdentifiedData;
#if SHOW_STATUS || PERFMON
                itmSw.Stop();
#endif
#if SHOW_STATUS
                ApplicationContext.Current.SetProgress(String.Format(Strings.locale_processBundle, itm.GetType().Name, i, data.Item.Count), i / (float)data.Item.Count);
#endif
#if PERFMON
                ApplicationContext.Current.PerformanceLog(nameof(BundlePersistenceService), nameof(InsertInternal), $"Insert{itm.GetType().Name}", itmSw.Elapsed);
#endif
            }


            return data;
        }

        /// <summary>
        /// Update everything in the bundle
        /// </summary>
        protected override Bundle UpdateInternal(LocalDataContext context, Bundle data)
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
        protected override Bundle ObsoleteInternal(LocalDataContext context, Bundle data)
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
