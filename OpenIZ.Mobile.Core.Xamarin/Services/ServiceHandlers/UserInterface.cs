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
 * Date: 2016-11-8
 */
using OpenIZ.Core.Applets.Model;
using OpenIZ.Mobile.Core.Services;
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
