using OpenIZ.Core.Applets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Update manager service is responsible for checking for updates and downloading / applying them
    /// </summary>
    public interface IUpdateManager 
    {

        /// <summary>
        /// Get server version of a package
        /// </summary>
        AppletInfo GetServerVersion(String packageId);

        /// <summary>
        /// Install the specified package from the server version
        /// </summary>
        void Install(String packageId);

    }
}
