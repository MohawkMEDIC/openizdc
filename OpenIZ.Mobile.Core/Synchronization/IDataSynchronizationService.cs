using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Synchronization
{
    /// <summary>
    /// Represents a data synchronization management service
    /// </summary>
    public interface IDataSynchronizationService
    {

        /// <summary>
        /// Fetches changes from the remote server for the specified 
        /// </summary>
        void Fetch();


    }
}
