using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Represents a thread pooling service
    /// </summary>
    public interface IThreadPoolService
    {
        /// <summary>
        /// Queues the specified action into the worker pool
        /// </summary>
        void QueueUserWorkItem(Action<Object> action);

        /// <summary>
        /// Queues the specified action into the worker pool
        /// </summary>
        void QueueUserWorkItem(Action<Object> action, Object parm);
    }
}
