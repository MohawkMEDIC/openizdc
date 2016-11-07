using OpenIZ.Core.Applets.Model;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{

    /// <summary>
    /// Represents user interface handlers
    /// </summary>
    [RestService("/ui")]
    [Anonymous]
    public class UserInterface
    {

        // Cached routes file
        private byte[] m_routes = null;

        /// <summary>
        /// Calculates an Angular Routes file and returns it
        /// </summary>
        /// <returns></returns>
        [RestOperation(Method = "GET", UriPath = "/routes.js")]
        [return: RestMessage(RestMessageFormat.Raw)]
        public byte[] GetRoutes()
        {

            // Ensure response makes sense
            MiniImsServer.CurrentContext.Response.ContentType = "text/javascript";

            // Calculate routes
#if !DEBUG
            if (this.m_routes == null)
#endif
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms))
                    {
                        sw.WriteLine("OpenIZ = OpenIZ || {}");
                        sw.WriteLine("OpenIZ.UserInterface = OpenIZ.UserInterface || {}");
                        sw.WriteLine("OpenIZ.UserInterface.states = [");
                        // Collect routes
                        foreach (var itm in XamarinApplicationContext.Current.LoadedApplets.ViewStateAssets)
                        {
                            var htmlContent = (itm.Content ?? XamarinApplicationContext.Current.ResolveAppletAsset(itm)) as AppletAssetHtml;
                            var viewState = htmlContent.ViewState;
                            sw.WriteLine($"{{ name: '{viewState.Name}', url: '{viewState.Route}', abstract: {viewState.IsAbstract.ToString().ToLower()}, views: {{");
                            foreach(var view in viewState.View)
                            {
                                sw.WriteLine($"'{view.Name}' : {{ controller: '{view.Controller}', templateUrl: '{view.Route ?? itm.ToString() }' }}, ");
                            }
                            sw.WriteLine("} },");
                        }
                        sw.Write("];");
                    }
                    this.m_routes = ms.ToArray();
                }
            return this.m_routes;
        }

    }
}
