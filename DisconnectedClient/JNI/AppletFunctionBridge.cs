using OpenIZ.Core.Applets.Services;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Mobile.Core;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Xamarin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DisconnectedClient.JNI
{
    public class AppletFunctionBridge
    {
        // Context
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletFunctionBridge));

        // Cached menus
        private Dictionary<IPrincipal, String> m_cachedMenus = new Dictionary<IPrincipal, String>();

        private frmDisconnectedClient m_context;
        
        /// <summary>
        /// Gets the context of the function
        /// </summary>
        /// <param name="context">Context.</param>
        public AppletFunctionBridge(frmDisconnectedClient context)
        {
            this.m_context = context;
        }

        /// <summary>
        /// Get the specified reference set
        /// </summary>
        public String GetDataAsset(String dataId)
        {
            dataId = String.Format("data/{0}", dataId);
            return Convert.ToBase64String(ApplicationContext.Current.GetService<IAppletManagerService>().Applets.RenderAssetContent(XamarinApplicationContext.Current.GetService<IAppletManagerService>().Applets.SelectMany(o => o.Assets).FirstOrDefault(o => o.Name == dataId), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));
        }

        /// <summary>
        /// Gets the online status
        /// </summary>
        public bool GetOnlineState()
        {
            return ApplicationContext.Current?.GetService<INetworkInformationService>()?.IsNetworkAvailable == true;
        }

        /// <summary>
        /// Gets the online status
        /// </summary>
        public bool IsClinicalAvailable()
        {
            return ApplicationContext.Current.GetService<IClinicalIntegrationService>().IsAvailable();
        }

        /// <summary>
        /// Gets the online status
        /// </summary>
        public bool IsAdminAvailable()
        {
            return ApplicationContext.Current.GetService<IAdministrationIntegrationService>().IsAvailable();
        }

        /// <summary>
        /// Send log file
        /// </summary>
        public String GetStatus()
        {
            return "";
        }


        /// <summary>
        /// Gets the registered template form
        /// </summary>
        /// <param name="templateId"></param>
        public String GetTemplateForm(String templateId)
        {
            return XamarinApplicationContext.Current.GetService<IAppletManagerService>().Applets.GetTemplateDefinition(templateId)?.Form;
        }

        /// <summary>
        /// Get log files
        /// </summary>
        /// <returns></returns>
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
        public void ShowToast(String toastText)
        {
            try
            {
                // TODO:
            }
            catch { }
        }

        /// <summary>
        /// Gets the current asset title
        /// </summary>
        public string GetCurrentAssetTitle()
        {
            return null;
        }

        /// <summary>
        /// Get version name
        /// </summary>
        public String GetVersion()
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.DefinedTypes.Any(t => t.Name == "SplashActivity")) ?? typeof(OpenIZConfiguration).Assembly;

            return String.Format("{0} ({1})", asm.GetName().Version,
                asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
        }

        /// <summary>
        /// Create new UUID
        /// </summary>
        public String NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Get version name
        /// </summary>
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


        
        public string GetNetworkState()
        {
            var networkInformationService = ApplicationContext.Current.GetService<INetworkInformationService>();

            return networkInformationService.IsNetworkAvailable.ToString();
        }
      
        /// <summary>
        /// Get the specified string
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="stringId">String identifier.</param>
        public String GetString(String stringId)
        {
            try
            {
                return stringId;
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
        public void Back()
        {
            this.m_context.Back();
        }


        /// <summary>Close the applet</summary>
        public void Close()
        {
            this.m_context.Close();
        }

        /// <summary>
        /// Performs a barcode scan
        /// </summary>
        public String BarcodeScan()
        {
            // TODO:
            return null;
        }

        /// <summary>
        /// Get the locale of the user interface
        /// </summary>
        public String GetLocale()
        {
            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;//this.m_context.Resources.Configuration.Locale.Language;
        }

        /// <summary>
        /// Get the applet strings
        /// </summary>
        public String GetStrings(String locale)
        {
            var strings = XamarinApplicationContext.Current.GetService<IAppletManagerService>().Applets.GetStrings(locale);

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
        public String SetLocale(String locale)
        {
            try
            {
                this.m_tracer.TraceVerbose("Setting locale to {0}", locale);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(locale);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error setting locale to {0}: {1}", locale, e);
            }
            return CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName;
        }

        /// <summary>
        /// Get magic
        /// </summary>
        public String GetMagic()
        {
            return ApplicationContext.Current.ExecutionUuid.ToString();
        }
    }
}
