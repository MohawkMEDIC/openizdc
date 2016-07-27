using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Mobile.Core.Synchronization.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Represents a synchronization service 
    /// </summary>
    public interface ISynchronizationService
    {

        /// <summary>
        /// Fetch to see if there are any particular changes on the specified model type
        /// </summary>
        bool Fetch(Type modelType);

        /// <summary>
        /// Pull data from the remove server and place it on the inbound queue
        /// </summary>
        void Pull(Type modelType);

        /// <summary>
        /// Pull data from the remove server and place it on the inbound queue
        /// </summary>
        void Pull(Type modelType, NameValueCollection filter);


    }
}
