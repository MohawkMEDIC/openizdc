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
using OpenIZ.Mobile.Core.Xamarin.Services;
using OpenIZ.Core.Applets.Model;
using System.IO;
using OpenIZ.Mobile.Core.Configuration;

namespace OpenIZ.Mobile.Core.Android.Services
{
    /// <summary>
    /// Represents an android asset manager
    /// </summary>
    public class AndroidAppletManagerService : LocalAppletManagerService
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public AndroidAppletManagerService()
        {
            this.m_appletCollection.Resolver = this.ResolveAppletAsset;
            this.m_appletCollection.CachePages = true;
        }

        /// <summary>
        /// Resolve asset
        /// </summary>
        public object ResolveAppletAsset(AppletAsset navigateAsset)
        {
            String itmPath = System.IO.Path.Combine(
                                        ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().AppletDirectory,
                                        "assets",
                                        navigateAsset.Manifest.Info.Id,
                                        navigateAsset.Name);

            return File.ReadAllBytes(itmPath);
        }

    }
}