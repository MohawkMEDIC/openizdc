/*
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
using OpenIZ.Mobile.Core.Xamarin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Applets.Model;
using System.IO;
using System.Xml.Linq;
using OpenIZ.Mobile.Core;
using System.Globalization;
using OpenIZ.Mobile.Core.Configuration;
using System.Reflection;

namespace Minims
{
    /// <summary>
    /// Applet manager service which overrides the local applet manager service
    /// </summary>
    public class MiniAppletManagerService : LocalAppletManagerService
    {

        // XSD OpenIZ
        private static readonly XNamespace xs_openiz = "http://openiz.org/applet";

        // Applet bas directory
        internal Dictionary<AppletManifest, String> m_appletBaseDir = new Dictionary<AppletManifest, string>();

        /// <summary>
        /// Install applet
        /// </summary>
        /// <param name="package"></param>
        /// <param name="isUpgrade"></param>
        /// <returns></returns>
        public override bool Install(AppletPackage package, bool isUpgrade = false)
        {
            return false;
        }

        /// <summary>
        /// Mime types
        /// </summary>
        private readonly Dictionary<String, String> s_mime = new Dictionary<string, string>()
        {
            { ".eot", "application/vnd.ms-fontobject" },
            { ".woff", "application/font-woff" },
            { ".woff2", "application/font-woff2" },
            { ".ttf", "application/octet-stream" },
            { ".svg", "image/svg+xml" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".png", "image/png" },
            { ".bmp", "image/bmp" },
            { ".json", "application/json" }

        };

        /// <summary>
        /// Resolving of assets
        /// </summary>
        public MiniAppletManagerService()
        {
            this.m_appletCollection.Resolver = this.ResolveAppletAsset;
            this.m_appletCollection.CachePages = false;
        }

        /// <summary>
        /// Resolve the specified applet name
        /// </summary>
        private String ResolveName(string value)
        {
            return value?.ToLower().Replace("\\", "/");
        }

        /// <summary>
        /// Process the specified directory
        /// </summary>
        private IEnumerable<AppletAsset> ProcessDirectory(string source, String path)
        {
            List<AppletAsset> retVal = new List<AppletAsset>();
            foreach (var itm in Directory.GetFiles(source))
            {
                Console.WriteLine("\t Processing {0}...", itm);

                if (Path.GetFileName(itm).ToLower() == "manifest.xml")
                    continue;
                else
                    switch (Path.GetExtension(itm))
                    {
                        case ".html":
                        case ".htm":
                        case ".xhtml":
                            XElement xe = XElement.Load(itm);
                            // Now we have to iterate throuh and add the asset\

                            var demand = xe.DescendantNodes().OfType<XElement>().Where(o => o.Name == xs_openiz + "demand").Select(o => o.Value).ToList();

                            var includes = xe.DescendantNodes().OfType<XComment>().Where(o => o?.Value?.Trim().StartsWith("#include virtual=\"") == true).ToList();
                            foreach (var inc in includes)
                            {
                                String assetName = inc.Value.Trim().Substring(18); // HACK: Should be a REGEX
                                if (assetName.EndsWith("\""))
                                    assetName = assetName.Substring(0, assetName.Length - 1);
                                if (assetName == "content")
                                    continue;
                                var includeAsset = ResolveName(assetName);
                                inc.AddAfterSelf(new XComment(String.Format("#include virtual=\"{0}\"", includeAsset)));
                                inc.Remove();

                            }


                            var xel = xe.Descendants().OfType<XElement>().Where(o => o.Name.Namespace == xs_openiz).ToList();
                            if (xel != null)
                                foreach (var x in xel)
                                    x.Remove();
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "text/html",
                                Content = null,
                                Policies = demand

                            });
                            break;
                        case ".css":
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "text/css",
                                Content = null
                            });
                            break;
                        case ".js":
                        case ".json":
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = "text/javascript",
                                Content = null
                            });
                            break;
                        default:
                            string mt = null;
                            retVal.Add(new AppletAsset()
                            {
                                Name = ResolveName(itm.Replace(path, "")),
                                MimeType = s_mime.TryGetValue(Path.GetExtension(itm), out mt) ? mt : "application/octet-stream",
                                Content = null
                            });
                            break;

                    }
            }

            // Process sub directories
            foreach (var dir in Directory.GetDirectories(source))
                if (!Path.GetFileName(dir).StartsWith("."))
                    retVal.AddRange(ProcessDirectory(dir, path));
                else
                    Console.WriteLine("Skipping directory {0}", dir);

            return retVal;
        }
        
        /// <summary>
        /// Load applet
        /// </summary>
        public override bool LoadApplet(AppletManifest applet)
        {
            if (applet.Assets.Count == 0)
            {
                var baseDirectory = this.m_appletBaseDir[applet];
                if (!baseDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    baseDirectory += Path.DirectorySeparatorChar.ToString();
                applet.Assets.AddRange(this.ProcessDirectory(baseDirectory, baseDirectory));
                applet.Initialize();
                if (applet.Info.Version.Contains("*"))
                    applet.Info.Version = applet.Info.Version.Replace("*", "0000");
            }
            return base.LoadApplet(applet);
        }

        /// <summary>
        /// Get applet asset
        /// </summary>
        public object ResolveAppletAsset(AppletAsset navigateAsset)
        {

            String itmPath = System.IO.Path.Combine(
                                        this.m_appletBaseDir[navigateAsset.Manifest],
                                        navigateAsset.Name);

            if (navigateAsset.MimeType == "text/html")
            {
                XElement xe = XElement.Load(itmPath);

                // Now we have to iterate throuh and add the asset\
                AppletAssetHtml htmlAsset = null;

                if (xe.Elements().OfType<XElement>().Any(o => o.Name == xs_openiz + "widget"))
                {
                    var widgetEle = xe.Elements().OfType<XElement>().FirstOrDefault(o => o.Name == xs_openiz + "widget");
                    htmlAsset = new AppletWidget()
                    {
                        Icon = widgetEle.Element(xs_openiz + "icon")?.Value,
                        Type = (AppletWidgetType)Enum.Parse(typeof(AppletWidgetType), widgetEle.Attribute("type")?.Value),
                        Scope = (AppletWidgetScope)Enum.Parse(typeof(AppletWidgetScope), widgetEle.Attribute("scope")?.Value),
                        Description = widgetEle.Elements().Where(o => o.Name == xs_openiz + "description").Select(o => new LocaleString() { Value = o.Value, Language = o.Attribute("lang")?.Value }).ToList(),
                        Name = widgetEle.Attribute("name")?.Value,
                        Controller = widgetEle.Element(xs_openiz + "controller")?.Value,
                    };
                }
                else
                {
                    htmlAsset = new AppletAssetHtml();
                    // View state data
                    htmlAsset.ViewState = xe.Elements().OfType<XElement>().Where(o => o.Name == xs_openiz + "state").Select(o => new AppletViewState()
                    {
                        Name = o.Attribute("name")?.Value,
                        Route = o.Elements().OfType<XElement>().FirstOrDefault(r => r.Name == xs_openiz + "url" || r.Name == xs_openiz + "route")?.Value,
                        IsAbstract = Boolean.Parse(o.Attribute("abstract")?.Value ?? "False"),
                        View = o.Elements().OfType<XElement>().Where(v => v.Name == xs_openiz + "view")?.Select(v => new AppletView()
                        {
                            Name = v.Attribute("name")?.Value,
                            Title = v.Elements().OfType<XElement>().Where(t => t.Name == xs_openiz + "title")?.Select(t => new LocaleString()
                            {
                                Language = t.Attribute("lang")?.Value,
                                Value = t?.Value
                            }).ToList(),
                            Controller = v.Element(xs_openiz + "controller")?.Value
                        }).ToList()
                    }).FirstOrDefault();
                    htmlAsset.Layout = ResolveName(xe.Attribute(xs_openiz + "layout")?.Value);
                    htmlAsset.Static = xe.Attribute(xs_openiz + "static")?.Value == "true";
                }

                htmlAsset.Titles = new List<LocaleString>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "title").Select(o => new LocaleString() { Language = o.Attribute("lang")?.Value, Value = o.Value }));
                htmlAsset.Bundle = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "bundle").Select(o => ResolveName(o.Value)));
                htmlAsset.Script = new List<AssetScriptReference>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "script").Select(o => new AssetScriptReference()
                {
                    Reference = ResolveName(o.Value),
                    IsStatic = Boolean.Parse(o.Attribute("static")?.Value ?? "true")
                }));
                htmlAsset.Style = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "style").Select(o => ResolveName(o.Value)));

                var demand = xe.DescendantNodes().OfType<XElement>().Where(o => o.Name == xs_openiz + "demand").Select(o => o.Value).ToList();

                var includes = xe.DescendantNodes().OfType<XComment>().Where(o => o?.Value?.Trim().StartsWith("#include virtual=\"") == true).ToList();
                foreach (var inc in includes)
                {
                    String assetName = inc.Value.Trim().Substring(18); // HACK: Should be a REGEX
                    if (assetName.EndsWith("\""))
                        assetName = assetName.Substring(0, assetName.Length - 1);
                    if (assetName == "content")
                        continue;
                    var includeAsset = ResolveName(assetName);
                    inc.AddAfterSelf(new XComment(String.Format("#include virtual=\"{0}\"", includeAsset)));
                    inc.Remove();
                }

                var xel = xe.Descendants().OfType<XElement>().Where(o => o.Name.Namespace == xs_openiz).ToList();
                if (xel != null)
                    foreach (var x in xel)
                        x.Remove();
                htmlAsset.Html = xe;
                return htmlAsset;
            }
            else if (navigateAsset.MimeType == "text/javascript" ||
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
                return File.ReadAllBytes(itmPath);
        }

        /// <summary>
        /// Get the SHIM methods
        /// </summary>
        /// <returns></returns>
        public String GetShimMethods()
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
                tw.WriteLine("return '[{0}]'", String.Join(",", this.Applets.SelectMany(o => o.Templates).Where(o => o.Public).Select(o => $"\"{o.Mnemonic}\"")));
                tw.WriteLine("}");
                tw.WriteLine("OpenIZApplicationService.GetDataAsset = function(assetId) {");
                tw.WriteLine("\tswitch(assetId) {");
                foreach (var itm in this.Applets.SelectMany(o => o.Assets).Where(o => o.Name.StartsWith("data/")))
                    tw.WriteLine("\t\tcase '{0}': return '{1}'; break;", itm.Name.Replace("data/", ""), Convert.ToBase64String(this.Applets.RenderAssetContent(itm)).Replace("'", "\\'"));
                tw.WriteLine("\t}");
                tw.WriteLine("}");
                // Read the static shim
                using (StreamReader shim = new StreamReader(typeof(MiniApplicationContext).Assembly.GetManifestResourceStream("Minims.lib.shim.js")))
                    tw.Write(shim.ReadToEnd());

                return tw.ToString();
            }
        }

    }
}
