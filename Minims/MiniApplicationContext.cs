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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services.Impl;
using OpenIZ.Mobile.Core.Data;
using OpenIZ.Mobile.Core.Configuration.Data;
using OpenIZ.Core.Model.EntityLoader;
using System.Diagnostics;
using System.Security.Principal;
using OpenIZ.Core.Applets;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Diagnostics.Tracing;
using OpenIZ.Mobile.Core;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Xamarin;
using OpenIZ.Mobile.Core.Xamarin.Configuration;
using System.Xml.Linq;
using System.Reflection;
using System.Globalization;
using OpenIZ.Protocol.Xml;
using OpenIZ.Protocol.Xml.Model;

namespace Minims
{
    /// <summary>
    /// Test application context
    /// </summary>
    public class MiniApplicationContext : XamarinApplicationContext
    {
        // XSD OpenIZ
        private static readonly XNamespace xs_openiz = "http://openiz.org/applet";

        // The application
        private static readonly OpenIZ.Core.Model.Security.SecurityApplication c_application = new OpenIZ.Core.Model.Security.SecurityApplication()
        {
            ApplicationSecret = "A1CF054D04D04CD1897E114A904E328D",
            Key = Guid.Parse("4C5A581C-A6EE-4267-9231-B0D3D50CC08A"),
            Name = "org.openiz.minims"
        };

        // Applet bas directory
        private Dictionary<AppletManifest, String> m_appletBaseDir = new Dictionary<AppletManifest, string>();
        
        private MiniConfigurationManager m_configurationManager;

        /// <summary>
        /// Configuration manager
        /// </summary>
        public override IConfigurationManager ConfigurationManager
        {
            get
            {
                return this.m_configurationManager;
            }
        }

        /// <summary>
        /// Get the configuration
        /// </summary>
        public override OpenIZConfiguration Configuration
        {
            get
            {
                return this.ConfigurationManager.Configuration;
            }
        }

        /// <summary>
        /// Get the application
        /// </summary>
        public override SecurityApplication Application
        {
            get
            {
                return c_application;
            }
        }

        /// <summary>
        /// Static CTOR bind to global handlers to log errors
        /// </summary>
        /// <value>The current.</value>
        static MiniApplicationContext()
        {

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (XamarinApplicationContext.Current != null)
                {
                    Tracer tracer = Tracer.GetTracer(typeof(XamarinApplicationContext));
                    tracer.TraceEvent(EventLevel.Critical, "Uncaught exception: {0}", e.ExceptionObject.ToString());
                }
            };


        }


        /// <summary>
		/// Starts the application context using in-memory default configuration for the purposes of 
		/// configuring the software
		/// </summary>
		/// <returns><c>true</c>, if temporary was started, <c>false</c> otherwise.</returns>
		public static bool StartTemporary(ConsoleParameters consoleParms)
        {
            try
            {
                var retVal = new MiniApplicationContext();
                retVal.SetProgress("Run setup", 0);

                retVal.m_configurationManager = new MiniConfigurationManager(MiniConfigurationManager.GetDefaultConfiguration());
                retVal.Principal = new ClaimsPrincipal(new ClaimsIdentity("SYSTEM", true, new Claim[] {
                    new Claim(ClaimTypes.OpenIzGrantedPolicyClaim, PolicyIdentifiers.AccessClientAdministrativeFunction)
                }));
                ApplicationContext.Current = retVal;
                retVal.m_tracer = Tracer.GetTracer(typeof(MiniApplicationContext));

                retVal.SetProgress("Loading configuration", 0.2f);
                // Load all user-downloaded applets in the data directory
                foreach (var appletDir in consoleParms.AppletDirectories)// Directory.GetFiles(this.m_configuration.GetSection<AppletConfigurationSection>().AppletDirectory)) {
                    try
                    {
                        if (!Directory.Exists(appletDir) || !File.Exists(Path.Combine(appletDir, "Manifest.xml")))
                            continue;

                        retVal.m_tracer.TraceInfo("Loading applet {0}", appletDir);
                        String appletPath = Path.Combine(appletDir, "manifest.xml");
                        using (var fs = File.OpenRead(appletPath))
                        {
                            AppletManifest manifest = AppletManifest.Load(fs);
                            retVal.m_appletBaseDir.Add(manifest, appletDir);
                            // Is this applet in the allowed applets

                            // public key token match?
                            retVal.LoadApplet(manifest);
                        }
                    }
                    catch (Exception e)
                    {
                        retVal.m_tracer.TraceError("Loading applet {0} failed: {1}", appletDir, e.ToString());
                        throw;
                    }
                retVal.LoadedApplets.CachePages = false;
                retVal.LoadedApplets.Resolver = retVal.ResolveAppletAsset;

                retVal.Start();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("OpenIZ FATAL", e.ToString());
                return false;
            }
        }


        /// <summary>
        /// Start the application context
        /// </summary>
        public static bool Start(ConsoleParameters consoleParms)
        {

            var retVal = new MiniApplicationContext();
            retVal.m_configurationManager = new MiniConfigurationManager();
            // Not configured
            if (!retVal.ConfigurationManager.IsConfigured)
            {
                return false;
            }
            else
            { // load configuration
                try
                {
                    retVal.ConfigurationManager.Load();
                    // Set master application context
                    ApplicationContext.Current = retVal;
                    retVal.m_tracer = Tracer.GetTracer(typeof(MiniApplicationContext), retVal.ConfigurationManager.Configuration);

                    retVal.SetProgress("Loading configuration", 0.2f);
                    // Load all user-downloaded applets in the data directory
                    foreach (var appletDir in consoleParms.AppletDirectories)// Directory.GetFiles(this.m_configuration.GetSection<AppletConfigurationSection>().AppletDirectory)) {
                        try
                        {
                            if (!Directory.Exists(appletDir) || !File.Exists(Path.Combine(appletDir, "Manifest.xml")))
                                continue;

                            retVal.m_tracer.TraceInfo("Loading applet {0}", appletDir);
                            String appletPath = Path.Combine(appletDir, "manifest.xml");
                            using (var fs = File.OpenRead(appletPath))
                            {
                                AppletManifest manifest = AppletManifest.Load(fs);
                                retVal.m_appletBaseDir.Add(manifest, appletDir);
                                // Is this applet in the allowed applets

                                // public key token match?
                                retVal.LoadApplet(manifest);
                            }
                        }
                        catch (Exception e)
                        {
                            retVal.m_tracer.TraceError("Loading applet {0} failed: {1}", appletDir, e.ToString());
                            throw;
                        }

                    // Ensure data migration exists
                    try
                    {
                        // If the DB File doesn't exist we have to clear the migrations
                        if (!File.Exists(retVal.Configuration.GetConnectionString(retVal.Configuration.GetSection<DataConfigurationSection>().MainDataSourceConnectionStringName).Value))
                        {
                            retVal.m_tracer.TraceWarning("Can't find the OpenIZ database, will re-install all migrations");
                            retVal.Configuration.GetSection<DataConfigurationSection>().MigrationLog.Entry.Clear();
                        }
                        retVal.SetProgress("Migrating databases", 0.6f);

                        DataMigrator migrator = new DataMigrator();
                        migrator.Ensure();

                        // Set the entity source
                        EntitySource.Current = new EntitySource(retVal.GetService<IEntitySourceProvider>());

                        // Prepare clinical protocols
                        //retVal.GetService<ICarePlanService>().Repository = retVal.GetService<IClinicalProtocolRepositoryService>();
                        ApplicationServiceContext.Current = ApplicationContext.Current;
                    }
                    catch (Exception e)
                    {
                        retVal.m_tracer.TraceError(e.ToString());
                        throw;
                    }
                    finally
                    {
                        retVal.ConfigurationManager.Save();
                    }
                    retVal.LoadedApplets.CachePages = false;
                    retVal.Configuration.GetSection<DiagnosticsConfigurationSection>().TraceWriter.Add(new TraceWriterConfiguration()
                    {
                        TraceWriter = new ConsoleTraceWriter(EventLevel.LogAlways, "")
                    });

                    retVal.LoadedApplets.Resolver = retVal.ResolveAppletAsset;

                    // Load clinical protocols
                    retVal.m_tracer.TraceInfo("Loading clinical protocols..");
                    foreach (var dir in consoleParms.ProtocolDirectory)
                        foreach (var f in Directory.GetFiles(dir))
                        {
                            retVal.m_tracer.TraceVerbose("Installing {0}...", f);
                            using (var stream = File.OpenRead(f))
                            {
                                var pd = ProtocolDefinition.Load(stream);
                                retVal.InstallProtocol(new XmlClinicalProtocol(pd));
                            }
                        }
                    // Start daemons
                    retVal.Start();
                    
                }
                catch (Exception e)
                {
                    retVal.m_tracer?.TraceError(e.ToString());
                    ApplicationContext.Current = null;
                    throw;
                }
                return true;
            }
        }

        /// <summary>
        /// Load applet
        /// </summary>
        public override void LoadApplet(AppletManifest applet)
        {
            var baseDirectory = this.m_appletBaseDir[applet];
            if (!baseDirectory.EndsWith("\\"))
                baseDirectory += "\\";
            applet.Assets.AddRange(this.ProcessDirectory(baseDirectory, baseDirectory));
            applet.Initialize();
            base.LoadApplet(applet);
        }


        private static Dictionary<String, String> mime = new Dictionary<string, string>()
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
                            AppletAssetHtml htmlAsset = new AppletAssetHtml();
                            htmlAsset.Layout = ResolveName(xe.Attribute(xs_openiz + "layout")?.Value);
                            htmlAsset.Titles = new List<LocaleString>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "title").Select(o => new LocaleString() { Language = o.Attribute("lang")?.Value, Value = o.Value }));
                            htmlAsset.Bundle = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "bundle").Select(o => ResolveName(o.Value)));
                            htmlAsset.Script = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "script").Select(o => ResolveName(o.Value)));
                            htmlAsset.Style = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "style").Select(o => ResolveName(o.Value)));
                            var demand = new List<String>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "demand").Select(o => o.Value));

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
                                MimeType = mime.TryGetValue(Path.GetExtension(itm), out mt) ? mt : "application/octet-stream",
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
        /// Resolve the specified applet name
        /// </summary>
        private static String ResolveName(string value)
        {

            return value?.ToLower().Replace("\\", "/");
        }

        /// <summary>
        /// Get applet asset
        /// </summary>
        public override object ResolveAppletAsset(AppletAsset navigateAsset)
        {

            String itmPath = System.IO.Path.Combine(
                                        this.m_appletBaseDir[navigateAsset.Manifest],
                                        navigateAsset.Name);

            if (navigateAsset.MimeType == "text/html")
            {
                XElement xe = XElement.Load(itmPath);

                // Now we have to iterate throuh and add the asset\
                AppletAssetHtml htmlAsset = new AppletAssetHtml();
                htmlAsset.Layout = ResolveName(xe.Attribute(xs_openiz + "layout")?.Value);
                htmlAsset.Titles = new List<LocaleString>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "title").Select(o => new LocaleString() { Language = o.Attribute("lang")?.Value, Value = o.Value }));
                htmlAsset.Bundle = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "bundle").Select(o => ResolveName(o.Value)));
                htmlAsset.Script = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "script").Select(o => ResolveName(o.Value)));
                htmlAsset.Style = new List<string>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "style").Select(o => ResolveName(o.Value)));
                var demand = new List<String>(xe.Descendants().OfType<XElement>().Where(o => o.Name == xs_openiz + "demand").Select(o => o.Value));

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
            //else if(navigateAsset.MimeType == "text/javascript" ||
            //    navigateAsset.MimeType == "text/css" ||
            //    navigateAsset.MimeType == "application/json")
            else
            {
                using (MemoryStream response = new MemoryStream())
                using (var fs = File.OpenRead(itmPath))
                {
                    int br = 8092;
                    byte[] buffer = new byte[8092];
                    while (br == 8092)
                    {
                        br = fs.Read(buffer, 0, 8092);
                        response.Write(buffer, 0, br);
                    }

                    if (navigateAsset.Name.Contains("/openiz.js"))
                    {
                        byte[] shim = this.GetShimMethods();
                        response.Write(shim, 0, shim.Length);
                    }
                    return response.ToArray();
                }
            }

        }

        /// <summary>
        /// Get the SHIM methods
        /// </summary>
        /// <returns></returns>
        private byte[] GetShimMethods()
        {
            // Load the default SHIM
            using (MemoryStream response = new MemoryStream())
            {
                // Write the generated shims
                using (TextWriter tw = new StreamWriter(response, Encoding.ASCII, 1024, true))
                {
                    tw.WriteLine("/// START OPENIZ MINI IMS SHIM");
                    // Version
                    tw.WriteLine("OpenIZApplicationService.GetVersion = function() {{ return '{0} ({1})'; }}", typeof(OpenIZConfiguration).Assembly.GetName().Version, typeof(OpenIZConfiguration).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
                    tw.WriteLine("OpenIZApplicationService.GetString = function(key) {");
                    tw.WriteLine("\tswitch(key) {");
                    foreach(var itm in this.LoadedApplets.GetStrings(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName))
                    {
                        tw.WriteLine("\t\tcase '{0}': return '{1}'; break;", itm.Key, itm.Value.Replace("'","\\'"));
                    }
                    tw.WriteLine("\t}");
                    tw.WriteLine("}");

                    tw.WriteLine("OpenIZApplicationService._CUUID = 0;");
                    tw.WriteLine("OpenIZApplicationService._UUIDS = [");
                    for (int i = 0; i < 30; i++)
                        tw.WriteLine("\t'{0}',", Guid.NewGuid());
                    tw.WriteLine("\t''];");

                    tw.WriteLine("OpenIZApplicationService.NewGuid = function() { return OpenIZApplicationService._UUIDS[OpenIZApplicationService._CUUID++]; }");

                }
                // Read the static shim
                using (Stream shim = typeof(MiniApplicationContext).Assembly.GetManifestResourceStream("Minims.lib.shim.js"))
                {
                    int br = 8092;
                    byte[] buffer = new byte[8092];
                    // HACK: Skip BOM
                    shim.Seek(3, SeekOrigin.Begin);
                    while (br == 8092)
                    {
                        br = shim.Read(buffer, 0, 8092);
                        response.Write(buffer, 0, br);
                    }
                }

                return response.ToArray();
            }
        }

        /// <summary>
        /// Install an applet
        /// </summary>
        public override void InstallApplet(AppletPackage package, bool isUpgrade = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        public override void SaveConfiguration()
        {
            this.m_configurationManager.Save();
        }
    }
}
