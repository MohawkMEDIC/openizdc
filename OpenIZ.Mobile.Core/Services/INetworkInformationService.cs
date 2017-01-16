/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-13
 */
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
        public NetworkInterfaceInfo(String name, String macAddress, bool isActive, String manufacturer)
        {
            this.Name = name;
            this.MacAddress = macAddress;
            this.IsActive = isActive;
            this.Manufacturer = manufacturer;
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

        /// <summary>
        /// Manufacturer
        /// </summary>
        public String Manufacturer { get; private set; }
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

		/// <summary>
		/// Gets whether the network is available
		/// </summary>
		bool IsNetworkAvailable { get; }

		/// <summary>
		/// Gets whether the network is connected.
		/// </summary>
		bool IsNetworkConnected { get; }

        /// <summary>
        /// Fired when the network status changes
        /// </summary>
        event EventHandler NetworkStatusChanged;

		/// <summary>
		/// Perform a DNS lookup
		/// </summary>
		string Nslookup(string address);
	}
}
