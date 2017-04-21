using OpenIZ.Core.Applets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Applets;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Diagnostics;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using System.Security;
using OpenIZ.Mobile.Core.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using OpenIZ.Mobile.Core.Services;

namespace OpenIZ.Mobile.Core.Xamarin.Services
{
    /// <summary>
    /// File based applet manager
    /// </summary>
    public class LocalAppletManagerService : IAppletManagerService
    {
        // Applet collection
        protected AppletCollection m_appletCollection = new AppletCollection();

        // RO applet collection
        private ReadonlyAppletCollection m_readonlyAppletCollection;

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalAppletManagerService));
        
        /// <summary>
        /// Local applet manager ctor
        /// </summary>
        public LocalAppletManagerService()
        {
            this.m_appletCollection = new AppletCollection();
            this.m_readonlyAppletCollection = this.m_appletCollection.AsReadonly();

        }

        /// <summary>
        /// Gets the loaded applets from the manager
        /// </summary>
        public ReadonlyAppletCollection Applets
        {
            get
            {
                return this.m_readonlyAppletCollection;
            }
        }

        /// <summary>
        /// Get the specified package data
        /// </summary>
        public byte[] GetPackage(String appletId)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Get applet by id
        /// </summary>
        /// <returns>The applet.</returns>
        /// <param name="id">Identifier.</param>
        public virtual AppletManifest GetApplet(String id)
        {
            return this.m_appletCollection.FirstOrDefault(o => o.Info.Id == id);
        }

        /// <summary>
		/// Register applet
		/// </summary>
		/// <param name="applet">Applet.</param>
		public virtual bool LoadApplet(AppletManifest applet)
        {
            if (applet.Info.Id == (ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().StartupAsset ?? "org.openiz.core"))
                this.m_appletCollection.DefaultApplet = applet;
            applet.Initialize();
            this.m_appletCollection.Add(applet);
            AppletCollection.ClearCaches();
            return true;
        }



        /// <summary>
        /// Verifies the manifest against it's recorded signature
        /// </summary>
        /// <returns><c>true</c>, if manifest was verifyed, <c>false</c> otherwise.</returns>
        /// <param name="manifest">Manifest.</param>
        protected bool VerifyPackage(AppletPackage package)
        {
            // First check: Hash - Make sure the HASH is ok
            if (Convert.ToBase64String(SHA256.Create().ComputeHash(package.Manifest)) != Convert.ToBase64String(package.Meta.Hash))
                throw new InvalidOperationException($"Package contents of {package.Meta.Id} appear to be corrupt!");

            if (package.Meta.Signature != null)
            {
                this.m_tracer.TraceInfo("Will verify package {0}", package.Meta.Id.ToString());

                // Get the public key - first, is the publisher in the trusted publishers store?
                var x509Store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
                try
                {
                    x509Store.Open(OpenFlags.ReadOnly);
                    var cert = x509Store.Certificates.Find(X509FindType.FindByThumbprint, package.Meta.PublicKeyToken, false);

                    // Not in the central store, perhaps the cert is embedded?
                    if (cert.Count == 0)
                    {
                        // Embedded CER
                        if (package.PublicKey != null)
                        {
                            // Attempt to load
                            cert = new X509Certificate2Collection(new X509Certificate2(package.PublicKey));

                            // The embedded certificate is not in trusted publisher store and/or not valid
                            if (!ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().Security.TrustedPublishers.Contains(cert[0].Thumbprint) && !cert[0].Verify())
                            {
                                if (!ApplicationContext.Current.Confirm(String.Format(Strings.locale_untrustedPublisherPrompt, package.Meta.Names.First().Value, this.ExtractDNPart(cert[0].Subject, "CN"))))
                                    return false;
                                else
                                {
                                    ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().Security.TrustedPublishers.Add(cert[0].Thumbprint);
                                    ApplicationContext.Current.SaveConfiguration();
                                }
                            }
                        }
                        else
                        {
                            this.m_tracer.TraceError($"Cannot find public key of publisher information for {package.Meta.PublicKeyToken} or the local certificate is invalid");
                            throw new SecurityException(Strings.locale_invalidSignature);
                        }
                    }

                    // Certificate is not yet valid or expired 
                    if ((cert[0].NotAfter < DateTime.Now || cert[0].NotBefore > DateTime.Now) &&
                        !ApplicationContext.Current.Confirm(String.Format(Strings.locale_certificateExpired, this.ExtractDNPart(cert[0].Subject, "CN"), cert[0].NotAfter)))
                    {
                        this.m_tracer.TraceError($"Cannot find public key of publisher information for {package.Meta.PublicKeyToken} or the local certificate is invalid");
                        throw new SecurityException(Strings.locale_invalidSignature);
                    }

                    RSACryptoServiceProvider rsa = cert[0].PublicKey.Key as RSACryptoServiceProvider;

                    var retVal = rsa.VerifyData(package.Manifest, CryptoConfig.MapNameToOID("SHA1"), package.Meta.Signature);
                    if (!retVal)
                        throw new SecurityException(Strings.locale_invalidSignature);
                    return retVal;
                }
                finally
                {
                    x509Store.Close();
                }
            }
            else if (ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>().Security.AllowUnsignedApplets)
            {
                return ApplicationContext.Current.Confirm(String.Format(Strings.locale_unsignedAppletPrompt, package.Meta.Names.First().Value));
            }
            else
            {
                this.m_tracer.TraceError("Package {0} v.{1} (publisher: {2}) is not signed and cannot be installed", package.Meta.Id, package.Meta.Version, package.Meta.Author);
                throw new SecurityException(String.Format(Strings.locale_unsignedAppletsNotAllowed, package.Meta.Id));
            }
        }

        /// <summary>
        /// Extract a common name
        /// </summary>
        protected String ExtractDNPart(string subject, string part)
        {
            Regex cnParse = new Regex(@"([A-Za-z]{1,2})=(.*?),\s?");
            var matches = cnParse.Matches(subject + ",");
            foreach (Match m in matches)
                if (m.Groups[1].Value == part)
                    return m.Groups[2].Value;
            return String.Empty;
        }


        /// <summary>
        /// Uninstall the applet package
        /// </summary>
        public virtual bool UnInstall(String packageId)
        {

            this.m_tracer.TraceWarning("Un-installing {0}", packageId);
            // Applet check
            var applet = this.m_appletCollection.FirstOrDefault(o => o.Info.Id == packageId);
            if (applet == null)
                throw new FileNotFoundException($"Applet {packageId} is not installed");

            // Dependency check
            var dependencies = this.m_appletCollection.Where(o => o.Info.Dependencies.Any(d => d.Id == packageId));
            if (dependencies.Any())
                throw new InvalidOperationException($"Uninstalling {packageId} would break : {String.Join(", ", dependencies.Select(o => o.Info))}");

            this.UnInstallInternal(applet);

            return true;
        }

        /// <summary>
        /// Uninstall
        /// </summary>
        private void UnInstallInternal(AppletManifest applet)
        {

            // We're good to go!
            this.m_appletCollection.Remove(applet);

            var appletConfig = ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>();

            // Delete the applet registration data
            appletConfig.Applets.RemoveAll(o => o.Id == applet.Info.Id);
            ApplicationContext.Current.SaveConfiguration();

            if (File.Exists(Path.Combine(appletConfig.AppletDirectory, applet.Info.Id)))
                File.Delete(Path.Combine(appletConfig.AppletDirectory, applet.Info.Id));
            if (Directory.Exists(Path.Combine(appletConfig.AppletDirectory, "assets", applet.Info.Id)))
                Directory.Delete(Path.Combine(appletConfig.AppletDirectory, "assets", applet.Info.Id), true);

            AppletCollection.ClearCaches();
        }

        /// <summary>
        /// Performs an installation 
        /// </summary>
        public virtual bool Install(AppletPackage package, bool isUpgrade = false)
        {
            this.m_tracer.TraceWarning("Installing {0}", package.Meta);

            // TODO: Verify package hash / signature
            if (!this.VerifyPackage(package))
                throw new SecurityException("Applet failed validation");
            else if (!this.m_appletCollection.VerifyDependencies(package.Meta))
                throw new InvalidOperationException($"Applet {package.Meta} depends on : [{String.Join(", ", package.Meta.Dependencies.Select(o => o.ToString()))}] which are missing or incompatible");

            var appletSection = ApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>();
            String appletPath = Path.Combine(appletSection.AppletDirectory, package.Meta.Id);

            try
            {
                // Desearialize an prep for install

                this.m_tracer.TraceInfo("Installing applet {0} (IsUpgrade={1})", package.Meta, isUpgrade);

                ApplicationContext.Current.SetProgress(package.Meta.GetName("en"), 0.0f);
                // TODO: Verify the package

                // Copy
                if (!Directory.Exists(appletSection.AppletDirectory))
                    Directory.CreateDirectory(appletSection.AppletDirectory);

                if (File.Exists(appletPath))
                {
                    if (!isUpgrade)
                        throw new InvalidOperationException(Strings.err_duplicate_package_name);

                    // Unload the loaded applet version
                    var existingApplet = this.m_appletCollection.FirstOrDefault(o => o.Info.Id == package.Meta.Id);
                    if (existingApplet != null)
                        this.UnInstallInternal(existingApplet);
                }

                var mfst = package.Unpack();
                // Migrate data.
                if (mfst.DataSetup != null)
                {
                    foreach (var itm in mfst.DataSetup.Action)
                    {
                        Type idpType = typeof(IDataPersistenceService<>);
                        idpType = idpType.MakeGenericType(new Type[] { itm.Element.GetType() });
                        var svc = ApplicationContext.Current.GetService(idpType);
                        idpType.GetMethod(itm.ActionName).Invoke(svc, new object[] { itm.Element });
                    }
                }

                // Now export all the binary files out
                var assetDirectory = Path.Combine(appletSection.AppletDirectory, "assets", mfst.Info.Id);
                if (!Directory.Exists(assetDirectory))
                {
                    Directory.CreateDirectory(assetDirectory);
                }

                for (int i = 0; i < mfst.Assets.Count; i++)
                {
                    var itm = mfst.Assets[i];
                    var itmPath = Path.Combine(assetDirectory, itm.Name);
                    ApplicationContext.Current.SetProgress($"Installing {package.Meta.GetName("en")}", 0.1f + (float)(0.8 * (float)i / mfst.Assets.Count));

                    // Get dir name and create
                    if (!Directory.Exists(Path.GetDirectoryName(itmPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(itmPath));

                    // Extract content
                    if (itm.Content is byte[])
                    {
                        File.WriteAllBytes(itmPath, itm.Content as byte[]);
                        itm.Content = null;
                    }
                    else if (itm.Content is String)
                    {
                        File.WriteAllText(itmPath, itm.Content as String);
                        itm.Content = null;
                    }
                }

                // Serialize the data to disk
                using (FileStream fs = File.Create(appletPath))
                    mfst.Save(fs);

                // For now sign with SHA256
                SHA256 sha = SHA256.Create();
                package.Meta.Hash = sha.ComputeHash(File.ReadAllBytes(appletPath));
                // HACK: Re-re-remove 
                appletSection.Applets.RemoveAll(o => o.Id == package.Meta.Id);
                appletSection.Applets.Add(package.Meta.AsReference());

                ApplicationContext.Current.SetProgress(package.Meta.GetName("en"), 0.98f);

                ApplicationContext.Current.SaveConfiguration();

                this.LoadApplet(mfst);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error installing applet {0} : {1}", package.Meta.ToString(), e);

                // Remove
                if (File.Exists(appletPath))
                {
                    File.Delete(appletPath);
                }

                throw;
            }

            return true;
        }

    }
}
