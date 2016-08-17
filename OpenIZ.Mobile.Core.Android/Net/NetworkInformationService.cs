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
 * Date: 2016-7-18
 */
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
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Data;
using OpenIZ.Mobile.Core.Android.Security;
using OpenIZ.Mobile.Core.Security;
using Android.Net;

namespace OpenIZ.Mobile.Core.Android.Net
{
    /// <summary>
    /// Implementation of the network information service
    /// </summary>
    public class NetworkInformationService : INetworkInformationService
    {
        /// <summary>
        /// Network avaialbility changed
        /// </summary>
        public NetworkInformationService()
        {
            NetworkChange.NetworkAvailabilityChanged += (o, e) => this.NetworkStatusChanged?.Invoke(this, e);

            // TODO: Discuss the ramifications of this
			// this.NetworkStatusChanged += NetworkInformationService_NetworkStatusChanged;
        }

		/// <summary>
		/// Updates the registered services in the application context when the network status changes.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void NetworkInformationService_NetworkStatusChanged(object sender, EventArgs e)
		{
			INetworkInformationService networkInformationService = (INetworkInformationService)sender;

            // Because we may have network connectivity
			if (networkInformationService.IsNetworkAvailable)
			{
				ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(LocalPolicyInformationService).AssemblyQualifiedName);
				ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AmiPolicyInformationService).AssemblyQualifiedName);
				ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(OAuthIdentityProvider).AssemblyQualifiedName);
				ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(ImsiPersistenceService).AssemblyQualifiedName);
			}
			else
			{
				ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalPersistenceService).AssemblyQualifiedName);
				ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalIdentityService).AssemblyQualifiedName);
			}
		}

		/// <summary>
		/// Return whether the network is available
		/// </summary>
		public bool IsNetworkAvailable
        {
            get
            {
				ConnectivityManager connectivityManager = (ConnectivityManager)AndroidApplicationContext.Current.Context.GetSystemService(Context.ConnectivityService);

				return connectivityManager.ActiveNetworkInfo != null && connectivityManager.ActiveNetworkInfo.IsConnected;
			}
        }

        /// <summary>
        /// Network status has changed
        /// </summary>
        public event EventHandler NetworkStatusChanged;

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
        /// Perform a DNS lookup
        /// </summary>
        public string Nslookup(string address)
        {
            try
            {
				System.Uri uri = null;
                if (System.Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out uri))
                    address = uri.Host;
                var resolution = System.Net.Dns.GetHostEntry(address); 
                return resolution.AddressList.First().ToString();
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Retrieves the ping time to the specified host
        /// </summary>
        public long Ping(string hostName)
        {
            try
            {
				System.Uri uri = null;
                if (System.Uri.TryCreate(hostName, UriKind.RelativeOrAbsolute, out uri))
                    hostName = uri.Host;
                System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
                var reply = p.Send(hostName);
                return reply.Status == IPStatus.Success ? reply.RoundtripTime : -1;
            }
            catch
            {
                return -1;
            }
        }
    }
}