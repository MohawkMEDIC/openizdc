using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Represents network interface information
    /// </summary>
    public struct NetworkInterfaceInfo
    {

        /// <summary>
        /// Network interface ctor
        /// </summary>
        public NetworkInterfaceInfo(String name, String macAddress, bool isActive)
        {
            this.Name = name;
            this.MacAddress = macAddress;
            this.IsActive = isActive;
        }

        /// <summary>
        /// Mac address
        /// </summary>
        public String MacAddress { get; private set; }

        /// <summary>
        /// Gets or sets the name of the interface
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Indicates whether the interface is connected
        /// </summary>
        public bool IsActive { get; private set; }
    }

    /// <summary>
    /// Represents network information service 
    /// </summary>
    public interface INetworkInformationService
    {

        /// <summary>
        /// Get interface information 
        /// </summary>
        IEnumerable<NetworkInterfaceInfo> GetInterfaces();

        /// <summary>
        /// Pings the specified host
        /// </summary>
        long Ping(String hostName);
    }
}
