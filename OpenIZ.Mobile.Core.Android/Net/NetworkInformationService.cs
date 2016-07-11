using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Services;
using System.Net.NetworkInformation;

namespace OpenIZ.Mobile.Core.Android.Net
{
    /// <summary>
    /// Implementation of the network information service
    /// </summary>
    public class NetworkInformationService : INetworkInformationService
    {
        /// <summary>
        /// Gets all available interfaces
        /// </summary>
        public IEnumerable<NetworkInterfaceInfo> GetInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Select(o => new NetworkInterfaceInfo(
                o.Name, o.GetPhysicalAddress().ToString(), o.OperationalStatus == OperationalStatus.Up
            ));
        }

        /// <summary>
        /// Retrieves the ping time to the specified host
        /// </summary>
        public long Ping(string hostName)
        {
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            var reply = p.Send(hostName);
            return reply.Status == IPStatus.Success ? reply.RoundtripTime : -1 ;
        }
    }
}