using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
    /// Memory query persistence service
    /// </summary>
    public class MemoryQueryPersistenceService : IQueryPersistenceService
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(MemoryQueryPersistenceService));

        // Memory cache of queries
        private Dictionary<Guid, MemoryQueryInfo> m_queryCache = new Dictionary<Guid, MemoryQueryInfo>(10);

        // Sync object
        private Object m_syncObject = new object();

        /// <summary>
        /// Memory based query information
        /// </summary>
        public class MemoryQueryInfo
        {
            /// <summary>
            /// Total results
            /// </summary>
            public int TotalResults { get; set; }

            /// <summary>
            /// Results in the result set
            /// </summary>
            public IEnumerable<Guid> Results { get; set; }

            /// <summary>
            /// The query tag
            /// </summary>
            public object QueryTag { get; set; }

        }

        /// <summary>
        /// Gets the specified query results
        /// </summary>
        public IEnumerable<Guid> GetQueryResults(Guid queryId, int offset, int count)
        {
            MemoryQueryInfo retVal = null;
            if (this.m_queryCache.TryGetValue(queryId, out retVal))
                return retVal.Results.Skip(offset).Take(count);
            return null;
        }

        /// <summary>
        /// Get the query tag
        /// </summary>
        public object GetQueryTag(Guid queryId)
        {
            MemoryQueryInfo retVal = null;
            if (this.m_queryCache.TryGetValue(queryId, out retVal))
                return retVal.QueryTag;
            return null;
        }

        /// <summary>
        /// Return whether the query is registered
        /// </summary>
        public bool IsRegistered(Guid queryId)
        {
            return this.m_queryCache.ContainsKey(queryId);
        }

        /// <summary>
        /// Get the total results
        /// </summary>
        public long QueryResultTotalQuantity(Guid queryId)
        {
            MemoryQueryInfo retVal = null;
            if (this.m_queryCache.TryGetValue(queryId, out retVal))
                return retVal.TotalResults;
            return 0;
        }

        /// <summary>
        /// Register a query
        /// </summary>
        public bool RegisterQuerySet(Guid queryId, IEnumerable<Guid> results, object tag)
        {
            MemoryQueryInfo retVal = null;
            if (this.m_queryCache.TryGetValue(queryId, out retVal))
            {
                this.m_tracer.TraceVerbose("Updating query {0} ({1} results)", queryId, results.Count());
                retVal.Results = results;
                retVal.QueryTag = tag;
            }
            else
                lock (this.m_syncObject)
                {
                    this.m_tracer.TraceVerbose("Registering query {0} ({1} results)", queryId, results.Count());

                    this.m_queryCache.Add(queryId, new MemoryQueryInfo()
                    {
                        QueryTag = tag,
                        Results = results,
                        TotalResults = results.Count()
                    });
                }
            return true;
        }
    }
}
