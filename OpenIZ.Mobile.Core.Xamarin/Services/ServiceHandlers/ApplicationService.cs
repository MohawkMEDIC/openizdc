using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Mobile.Core.Alerting;
using OpenIZ.Core.Model.Query;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Services;
using Newtonsoft.Json;
using OpenIZ.Core.Applets.Model;
using System.Reflection;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using System.Globalization;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents a service for interacting with the application asynchronously
    /// </summary>
    [RestService("/__app")]
    public class ApplicationService
    {

        // App information
        private ApplicationInfo m_appInfo;

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(ApplicationService));

        /// <summary>
        /// Get menu information
        /// </summary>
        [RestOperation(UriPath = "/menu", Method = "GET")]
        [return: RestMessage(RestMessageFormat.Json)]
        public List<MenuInformation> GetMenus()
        {
            try
            {

                // Cannot have menus if not logged in
                if (!AuthenticationContext.Current.Principal.Identity.IsAuthenticated) return null;

                var rootMenus = XamarinApplicationContext.Current.LoadedApplets.SelectMany(o => o.Menus).OrderBy(o => o.Order).ToArray();
                List<MenuInformation> retVal = new List<MenuInformation>();

                // Create menus
                foreach (var mnu in rootMenus)
                    this.ProcessMenuItem(mnu, retVal);
                retVal.RemoveAll(o => o.Action == null && o.Menu?.Count == 0);


                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error retrieving menus: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Process menu item
        /// </summary>
        private void ProcessMenuItem(AppletMenu menu, List<MenuInformation> retVal)
        {
            // TODO: Demand permission
            if (menu.Launcher != null &&
                !XamarinApplicationContext.Current.LoadedApplets.ResolveAsset(menu.Launcher, menu.Manifest.Assets[0])?.Policies?.Any(p => ApplicationContext.Current.PolicyDecisionService.GetPolicyOutcome(AuthenticationContext.Current.Principal, p) == OpenIZ.Core.Model.Security.PolicyGrantType.Deny) == false)
                return;

            // Get text for menu item
            string menuText = menu.GetText(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
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
        /// Get the alerts from the service
        /// </summary>
        [RestOperation(UriPath = "/info", Method = "GET")]
        public ApplicationInfo GetInfo()
        {
            try
            {
                return new ApplicationInfo();
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Could not retrieve app info {0}...", e);
                throw;
            }
        }

        /// <summary>
        /// Get the alerts from the service
        /// </summary>
        [RestOperation(UriPath = "/alerts", Method = "GET")]
        public List<AlertMessage> GetAlerts()
        {
            try
            {
                // Gets the specified alert messages
                NameValueCollection query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

				var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();

				List<string> key = null;

				if (query.ContainsKey("id") && query.TryGetValue("id", out key))
				{
					var id = key?.FirstOrDefault();

					return new List<AlertMessage> { alertService.Get(Guid.Parse(id)) };
				}

                var predicate = QueryExpressionParser.BuildLinqExpression<AlertMessage>(query);
                int offset = query.ContainsKey("_offset") ? Int32.Parse(query["_offset"][0]) : 0,
                    count = query.ContainsKey("_count") ? Int32.Parse(query["_count"][0]) : 100;

                

				int totalCount = 0;

                return alertService.Find(predicate, offset, count, out totalCount).ToList();
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Could not retrieve alerts {0}...", e);
                throw;
            }
        }

        /// <summary>
        /// Get the alerts from the service
        /// </summary>
        [RestOperation(UriPath = "/alerts", Method = "POST")]
        public AlertMessage SaveAlert(AlertMessage alert)
        {
            try
            {
                // Gets the specified alert messages
                var alertService = ApplicationContext.Current.GetService<IAlertRepositoryService>();
                alertService.Save(alert);
                return alert;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Could not retrieve alerts {0}...", e);
                return null;
            }
        }

    }

}