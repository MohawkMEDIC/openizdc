using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Tickler
{
    /// <summary>
    /// Represents a tickle service that just uses memory
    /// </summary>
    public class MemoryTickleService : ITickleService
    {

        // Tickles
        private List<Tickle> m_tickles = new List<Tickle>();

        /// <summary>
        /// Dismiss a tickle
        /// </summary>
        public void DismissTickle(Guid tickleId)
        {
            lock (this.m_tickles)
                this.m_tickles.RemoveAll(o => o.Id == tickleId);
        }

        /// <summary>
        /// Get tickles
        /// </summary>
        public IEnumerable<Tickle> GetTickles(Expression<Func<Tickle, bool>> filter)
        {
            return this.m_tickles.Where(filter.Compile()).Where(o=>o.Expiry > DateTime.Now);
        }

        /// <summary>
        /// Send a tickle
        /// </summary>
        /// <param name="tickle"></param>
        public void SendTickle(Tickle tickle)
        {
            lock (this.m_tickles)
                this.m_tickles.Add(tickle);
        }
    }
}
