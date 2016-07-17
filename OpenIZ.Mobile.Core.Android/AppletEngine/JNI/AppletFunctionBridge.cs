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

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	
	/// <summary>
	/// Applet functions which are related to javascript
	/// </summary>
	public class AppletFunctionBridge : Java.Lang.Object
	{

        /// <summary>
        /// Menu information
        /// </summary>
        [JsonObject]
        private class MenuInformation
        {
           

            /// <summary>
            /// Get or sets the menu
            /// </summary>
            [JsonProperty("menu")]
            public List<MenuInformation> Menu { get; set; }

            /// <summary>
            /// Icon text
            /// </summary>
            [JsonProperty("icon")]
            public String Icon { get; set; }

            /// <summary>
            /// Text for the menu item
            /// </summary>
            [JsonProperty("text")]
            public String Text { get; set; }

            /// <summary>
            /// Action
            /// </summary>
            [JsonProperty("action")]
            public String Action { get; set; }
        }

		// Context
		private Context m_context;
		private AppletWebView m_view;
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletFunctionBridge));

		/// <summary>
		/// Gets the context of the function
		/// </summary>
		/// <param name="context">Context.</param>
		public AppletFunctionBridge (Context context, AppletWebView view)
		{
			
			this.m_context = context;
			this.m_view = view;
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
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error sending log file: {0}", e);
            }
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
            catch(Exception ex)
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
			Toast.MakeText (this.m_context, toastText, ToastLength.Short).Show();
		}

        /// <summary>
        /// Gets the current asset title
        /// </summary>
        [Export]
        [JavascriptInterface]
        public string GetCurrentAssetTitle()
        {
            return this.m_view.Title ;
        }

        /// <summary>
        /// Get version name
        /// </summary>
        [JavascriptInterface]
        [Export]
        public String GetVersion()
        {
            return String.Format("{0} ({1})", typeof(OpenIZConfiguration).Assembly.GetName().Version,
                typeof(OpenIZConfiguration).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
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

                var rootMenus = AndroidApplicationContext.Current.LoadedApplets.SelectMany(o => o.Menus).ToArray();
                List<MenuInformation> retVal = new List<MenuInformation>();
               
                // Create menus
                foreach(var mnu in rootMenus)
                    this.ProcessMenuItem(mnu, retVal);
                retVal.RemoveAll(o => o.Action == null && o.Menu?.Count == 0);

                return JniUtil.ToJson(retVal);
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error retrieving menus: {0}", e);
                return "err_menu";
            }
        }

        /// <summary>
        /// Process menu item
        /// </summary>
        private void ProcessMenuItem(AppletMenu menu, List<MenuInformation> retVal)
        {
            // TODO: Demand permission
            if (menu.Launcher != null &&
                !AndroidApplicationContext.Current.LoadedApplets.ResolveAsset(menu.Launcher)?.Policies?.Any(p => ApplicationContext.Current.PolicyDecisionService.GetPolicyOutcome(ApplicationContext.Current.Principal, p) == OpenIZ.Core.Model.Security.PolicyGrantType.Deny) == false)
                return;

            string menuText = menu.GetText(this.GetLocale());
            var existing = retVal.Find(o => o.Text == menuText);
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
            if(menu.Menus.Count > 0)
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
			Application.SynchronizationContext.Post (_ => {
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
            catch(Exception e)
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
			Application.SynchronizationContext.Post (_ => {
				if (this.m_view.CanGoBack ())
					this.m_view.GoBack ();
				else
					(this.m_context as Activity).Finish ();
			}, null);
		}


		/// <summary>Close the applet</summary>
		[Export]
		[JavascriptInterface]
		public void Close() {
			Application.SynchronizationContext.Post (_ => {
				(this.m_context as Activity).Finish ();
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
				return "XXXXXXXXXXXX";
			}
			catch(Exception e) {
				this.ShowToast (e.Message);
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
            return this.m_context.Resources.Configuration.Locale.Language;
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
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error setting locale to {0}: {1}", locale, e);
            }
            return this.m_context.Resources.Configuration.Locale.Language;
        }
    }
}

