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
using OpenIZ.Mobile.Core.Xamarin.Net;
using Android.Net;
using OpenIZ.Mobile.Core.Services;
using System.Net.NetworkInformation;
using AndroidOS = Android.OS;

namespace OpenIZ.Mobile.Core.Android.Net
{
    /// <summary>
    /// Android specific network information service
    /// </summary>
    public class AndroidNetworkInformationService : NetworkInformationService
    {
        public override IEnumerable<NetworkInterfaceInfo> GetInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Select(o => new NetworkInterfaceInfo(
                o.Name, o.GetPhysicalAddress().ToString(), o.OperationalStatus == OperationalStatus.Up, AndroidOS.Build.Model
            ));
        }
        /// <summary>
        /// Is network available
        /// </summary>
        public override bool IsNetworkAvailable
        {
            get
            {
                ConnectivityManager connectivityManager = (ConnectivityManager)((Xamarin.XamarinApplicationContext.Current as AndroidApplicationContext).Context).GetSystemService(Context.ConnectivityService);
                return connectivityManager.ActiveNetworkInfo != null && connectivityManager.ActiveNetworkInfo.IsConnected;
            }
        }
    }
}