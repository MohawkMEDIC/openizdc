﻿/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using OpenIZ.Core.Applets.Model;
using OpenIZ.Mobile.Core;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Xamarin.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DisconnectedClient.Core
{
    /// <summary>
    /// Disconnected client applet manager service
    /// </summary>
    public class DcAppletManagerService : LocalAppletManagerService
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public DcAppletManagerService()
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

            if (navigateAsset.MimeType == "text/javascript" ||
                        navigateAsset.MimeType == "text/css" ||
                        navigateAsset.MimeType == "application/json" ||
                        navigateAsset.MimeType == "text/xml")
            {
                var script = File.ReadAllText(itmPath);
                if (itmPath.Contains("openiz.js") || itmPath.Contains("openiz.min.js"))
                    script += this.GetShimMethods();
                return script;
            }
            else
                using (MemoryStream response = new MemoryStream())
                using (var fs = File.OpenRead(itmPath))
                {
                    int br = 8096;
                    byte[] buffer = new byte[8096];
                    while (br == 8096)
                    {
                        br = fs.Read(buffer, 0, 8096);
                        response.Write(buffer, 0, br);
                    }

                    return response.ToArray();
                }
        }

        /// <summary>
        /// Get the SHIM methods
        /// </summary>
        /// <returns></returns>
        private String GetShimMethods()
        {

            // Load the default SHIM
            // Write the generated shims
            using (StringWriter tw = new StringWriter())
            {
                tw.WriteLine("/// START OPENIZ MINI IMS SHIM");
                // Version
                tw.WriteLine("OpenIZApplicationService.GetMagic = function() {{ return '{0}'; }}", ApplicationContext.Current.ExecutionUuid);
                tw.WriteLine("OpenIZApplicationService.GetVersion = function() {{ return '{0} ({1})'; }}", typeof(OpenIZConfiguration).Assembly.GetName().Version, typeof(OpenIZConfiguration).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
                tw.WriteLine("OpenIZApplicationService.GetString = function(key) {");
                tw.WriteLine("\tswitch(key) {");
                foreach (var itm in this.Applets.GetStrings(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName))
                {
                    tw.WriteLine("\t\tcase '{0}': return '{1}'; break;", itm.Key, itm.Value?.Replace("'", "\\'").Replace("\r", "").Replace("\n", ""));
                }
                tw.WriteLine("\t}");
                tw.WriteLine("}");

                tw.WriteLine("OpenIZApplicationService.GetTemplateForm = function(templateId) {");
                tw.WriteLine("\tswitch(templateId) {");
                foreach (var itm in this.Applets.SelectMany(o => o.Templates))
                {
                    tw.WriteLine("\t\tcase '{0}': return '{1}'; break;", itm.Mnemonic.ToLowerInvariant(), itm.Form);
                }
                tw.WriteLine("\t}");
                tw.WriteLine("}");

                tw.WriteLine("OpenIZApplicationService.GetTemplateView = function(templateId) {");
                tw.WriteLine("\tswitch(templateId) {");
                foreach (var itm in this.Applets.SelectMany(o => o.Templates))
                {
                    tw.WriteLine("\t\tcase '{0}': return '{1}'; break;", itm.Mnemonic.ToLowerInvariant(), itm.View);
                }
                tw.WriteLine("\t}");
                tw.WriteLine("}");

                tw.WriteLine("OpenIZApplicationService.GetTemplates = function() {");
                tw.WriteLine("return '[{0}]'", String.Join(",", this.Applets.SelectMany(o=>o.Templates).Where(o=>o.Public).Select(o=>$"\"{o.Mnemonic}\"")));
                tw.WriteLine("}");

                tw.WriteLine("OpenIZApplicationService.GetDataAsset = function(assetId) {");
                tw.WriteLine("\tswitch(assetId) {");
                foreach (var itm in this.Applets.SelectMany(o => o.Assets).Where(o => o.Name.StartsWith("data/")))
                    tw.WriteLine("\t\tcase '{0}': return '{1}'; break;", itm.Name.Replace("data/", ""), Convert.ToBase64String(this.Applets.RenderAssetContent(itm)).Replace("'", "\\'"));
                tw.WriteLine("\t}");
                tw.WriteLine("}");
                
                // Read the static shim
                String shimFile = "DisconnectedClient.Core.lib.shim.js";
                var manifestName = Assembly.GetEntryAssembly().GetManifestResourceNames().FirstOrDefault(o => o.EndsWith("shim.js"));
                using (StreamReader shim = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream(manifestName)))
                    tw.Write(shim.ReadToEnd());

                return tw.ToString();
            }
        }


    }
}
