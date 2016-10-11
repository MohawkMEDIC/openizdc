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

namespace OpenIZ.Mobile.Core.Android.Net
{
    /// <summary>
    /// Android specific network information service
    /// </summary>
    public class AndroidNetworkInformationService : NetworkInformationService
    {
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