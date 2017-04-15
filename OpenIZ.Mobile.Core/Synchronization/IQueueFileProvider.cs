using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Synchronization
{
    /// <summary>
    /// Queue file provider
    /// </summary>
    public interface IQueueFileProvider
    {

        /// <summary>
        /// Save queue data
        /// </summary>
        String SaveQueueData(IdentifiedData data);

        /// <summary>
        /// Remove queue data
        /// </summary>
        void RemoveQueueData(String pathSpec);

        /// <summary>
        /// Get queue data
        /// </summary>
        IdentifiedData GetQueueData(String pathSpec, Type typeSpec);

        /// <summary>
        /// Get the raw queue data
        /// </summary>
        byte[] GetQueueData(String pathSpec);

        /// <summary>
        /// Copy queue data
        /// </summary>
        string CopyQueueData(string data);
    }
}
