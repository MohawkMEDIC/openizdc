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
 * Date: 2016-6-14
 */
using System.Linq;
using OpenIZ.Mobile.Core.Interop;

using System;
using Android.Webkit;
using Java.Interop;
using Android.Widget;
using Android.Content;
using Org.Json;
using Android.App;
using Android.Content.Res;
using OpenIZ.Mobile.Core.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using OpenIZ.Core.Applets.Model;
using A = Android;
using System.Reflection;
using OpenIZ.Mobile.Core.Configuration;
using System.Globalization;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Xamarin;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{

    /// <summary>
    /// Applet functions which are related to javascript
    /// </summary>
    public class AppletFunctionBridge : Java.Lang.Object
    {

        // Cached menus
        private Dictionary<IPrincipal, String> m_cachedMenus = new Dictionary<IPrincipal, String>();

        private KeyValuePair<String, float> m_applicationStatus;



        // Context
        private Context m_context;
        private AppletWebView m_view;
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletFunctionBridge));
        private Assembly m_appAssembly;

        /// <summary>
        /// Gets the context of the function
        /// </summary>
        /// <param name="context">Context.</param>
        public AppletFunctionBridge(Context context, AppletWebView view)
        {
            if((XamarinApplicationContext.Current as AndroidApplicationContext).AndroidApplication != null)
                ZXing.Mobile.MobileBarcodeScanner.Initialize((XamarinApplicationContext.Current as AndroidApplicationContext).AndroidApplication);
            ApplicationContext.ProgressChanged += (o, e) => this.m_applicationStatus = new KeyValuePair<string, float>(e.ProgressText, e.Progress);
            this.m_context = context;
            this.m_view = view;
        }

        /// <summary>
        /// Gets the online status
        /// </summary>
        [Export]
        [JavascriptInterface]
        public bool GetOnlineState()
        {
            return ApplicationContext.Current?.GetService<INetworkInformationService>()?.IsNetworkAvailable == true;
        }

        /// <summary>
        /// Send log file
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String GetStatus()
        {
            return String.Format("[\"{0}\",{1}]", this.m_applicationStatus.Key, this.m_applicationStatus.Value);
        }


        /// <summary>
        /// Send log file
        /// </summary>
        /// <returns></returns>
        [Export]
        [JavascriptInterface]
        public void SendLog(String logId)
        {
            try
            {
                String logFileBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log", logId);
                var email = new Intent(A.Content.Intent.ActionSend);
                email.PutExtra(A.Content.Intent.ExtraEmail, new string[] { "medic@mohawkcollege.ca" });
                email.PutExtra(A.Content.Intent.ExtraSubject, "Mobile Application Log File");
                email.SetType("message/rfc822");

                email.PutExtra(A.Content.Intent.ExtraText, File.ReadAllText(logFileBase));

                this.m_context.StartActivity(email);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error sending log file: {0}", e);
            }
        }

        /// <summary>
        /// Gets the registered template form
        /// </summary>
        /// <param name="templateId"></param>
        [JavascriptInterface]
        [Export]
        public String GetTemplateForm(String templateId)
        {
            return AndroidApplicationContext.Current.LoadedApplets.GetTemplateDefinition(templateId)?.Form;
        }

        /// <summary>
        /// Get log files
        /// </summary>
        /// <returns></returns>
        [Export]
        [JavascriptInterface]
        public String GetLogFiles()
        {
            try
            {
                String logFileBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log");
                List<String> files = new List<string>();
                foreach (var f in Directory.GetFiles(logFileBase))
                    files.Add(Path.GetFileName(f));
                return JniUtil.ToJson(files);
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error getting log files: {0}", ex);
                return null;
            }
        }

        /// <summary>
        /// Shows the specified message as a toast
        /// </summary>
        [Export]
        [JavascriptInterface]
        public void ShowToast(String toastText)
        {
            Toast.MakeText(this.m_context, toastText, ToastLength.Short).Show();
        }

        /// <summary>
        /// Gets the current asset title
        /// </summary>
        [Export]
        [JavascriptInterface]
        public string GetCurrentAssetTitle()
        {
            return (this.m_view as AppletWebView).ThreadSafeTitle;
        }

        /// <summary>
        /// Get version name
        /// </summary>
        [JavascriptInterface]
        [Export]
        public String GetVersion()
        {
            if (this.m_appAssembly == null)
                this.m_appAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.DefinedTypes.Any(t => t.Name == "SplashActivity")) ?? typeof(OpenIZConfiguration).Assembly;

            return String.Format("{0} ({1})", this.m_appAssembly.GetName().Version,
                this.m_appAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
        }

        /// <summary>
        /// Create new UUID
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Get version name
        /// </summary>
        [JavascriptInterface]
        [Export]
        public String GetService(String serviceName)
        {

            Type serviceType = Type.GetType(serviceName);
            if (serviceType == null)
                return ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Services.FirstOrDefault(
                    o => o.GetType().GetInterfaces().Any(i => i.Name == serviceName) ||
                    o.GetType().Name == serviceName || o.GetType().BaseType.Name == serviceName
                )?.GetType().Name;
            else
                return ApplicationContext.Current.GetService(serviceType)?.GetType().Name;
        }

        /// <summary>
        /// Get the menu items for the current user for specified language
        /// </summary>
        [JavascriptInterface]
        [Export]
        public String GetMenus()
        {
            try
            {

                // Cannot have menus if not logged in
                if (ApplicationContext.Current.Principal == null) return null;

                string cached = null;
                if (this.m_cachedMenus.TryGetValue(ApplicationContext.Current.Principal, out cached))
                    return cached;

                var rootMenus = AndroidApplicationContext.Current.LoadedApplets.SelectMany(o => o.Menus).OrderBy(o => o.Order).ToArray();
                List<MenuInformation> retVal = new List<MenuInformation>();

                // Create menus
                foreach (var mnu in rootMenus)
                    this.ProcessMenuItem(mnu, retVal);
                retVal.RemoveAll(o => o.Action == null && o.Menu?.Count == 0);


                cached = JniUtil.ToJson(retVal);
                lock (this.m_cachedMenus)
                    this.m_cachedMenus.Add(ApplicationContext.Current.Principal, cached);
                return cached;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error retrieving menus: {0}", e);
                return "err_menu";
            }
        }

		[Export]
		[JavascriptInterface]
		public string GetNetworkState()
		{
			var networkInformationService = ApplicationContext.Current.GetService<INetworkInformationService>();

			return networkInformationService.IsNetworkAvailable.ToString();
		}

        /// <summary>
        /// Process menu item
        /// </summary>
        private void ProcessMenuItem(AppletMenu menu, List<MenuInformation> retVal)
        {
            // TODO: Demand permission
            if (menu.Launcher != null &&
                !AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(menu.Launcher, menu.Manifest.Assets[0])?.Policies?.Any(p => ApplicationContext.Current.PolicyDecisionService.GetPolicyOutcome(ApplicationContext.Current.Principal, p) == OpenIZ.Core.Model.Security.PolicyGrantType.Deny) == false)
                return;

            // Get text for menu item
            string menuText = menu.GetText(this.GetLocale());
            var existing = retVal.Find(o => o.Text == menuText && o.Icon == menu.Icon);
            if (existing == null)
            {
                existing = new MenuInformation()
                {
                    Action = menu.Launcher,
                    Icon = menu.Icon,
                    Text = menuText
                };
                retVal.Add(existing);
            }
            if (menu.Menus.Count > 0)
            {
                existing.Menu = new List<MenuInformation>();
                foreach (var child in menu.Menus)
                    this.ProcessMenuItem(child, existing.Menu);
            }
        }

        /// <summary>
        /// Navigate the specified appletId and context.
        /// </summary>
        /// <param name="appletId">Applet identifier.</param>
        /// <param name="context">Context.</param>
        [Export]
        [JavascriptInterface]
        public void Navigate(String appletId, String context)
        {
            // TODO: Parameters
            Application.SynchronizationContext.Post(_ =>
            {
                this.m_view.LoadUrl(String.Format("app://openiz.org/applet/{0}/", appletId));
            }, null);
        }

        /// <summary>
        /// Get the specified string
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="stringId">String identifier.</param>
        [Export]
        [JavascriptInterface]
        public String GetString(String stringId)
        {
            try
            {
                var appletResource = AndroidApplicationContext.Current.LoadedApplets.GetStrings(this.GetLocale()).FirstOrDefault(o => o.Key == stringId).Value;
                if (appletResource != null)
                    return appletResource;
                else
                {
                    var androidStringId = this.m_context.Resources.GetIdentifier(stringId, "string", this.m_context.PackageName);
                    if (androidStringId > 0)
                        return this.m_context.Resources.GetString(androidStringId);
                    else
                        return stringId;
                }
            }
            catch (Exception e)
            {
                //this.m_tracer.TraceWarning("Error retreiving string {0}", stringId);
                return stringId;
            }
        }

        /// <summary>
        /// Go back home
        /// </summary>
        [Export]
        [JavascriptInterface]
        public void Back()
        {
            Application.SynchronizationContext.Post(_ =>
            {
                if (this.m_view.CanGoBack())
                    this.m_view.GoBack();
                else
                    (this.m_context as Activity).Finish();
            }, null);
        }


        /// <summary>Close the applet</summary>
        [Export]
        [JavascriptInterface]
        public void Close()
        {
            Application.SynchronizationContext.Post(_ =>
            {
                ApplicationContext.Current.Stop();
                (this.m_context as Activity).Finish();
            }, null);
        }

        /// <summary>
        /// Performs a barcode scan
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String BarcodeScan()
        {
            try
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                String retVal = String.Empty;
                var result = scanner.Scan().ContinueWith((o) => retVal = o.Result?.Text);
                result.Wait();
                return retVal;
            }
            catch (Exception e)
            {
                this.ShowToast(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get the locale of the user interface
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String GetLocale()
        {
            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;//this.m_context.Resources.Configuration.Locale.Language;
        }

        /// <summary>
        /// Get the applet strings
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String GetStrings(String locale)
        {
            var strings = AndroidApplicationContext.Current.LoadedApplets.GetStrings(locale);

            using (StringWriter sw = new StringWriter())
            {
                sw.Write("{");
                foreach (var itm in strings)
                {
                    sw.Write("\"{0}\":\"{1}\",", itm.Key, itm.Value);
                }
                sw.Write("\"locale\":\"{0}\" }}", locale);
                return sw.ToString();
            }

        }

        /// <summary>
        /// Set the locale of the application
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String SetLocale(String locale)
        {
            try
            {
                this.m_tracer.TraceVerbose("Setting locale to {0}", locale);
                this.m_context.Resources.Configuration.SetLocale(new Java.Util.Locale(locale));
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(locale);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error setting locale to {0}: {1}", locale, e);
            }
            return this.m_context.Resources.Configuration.Locale.Language;
        }
    }
}

