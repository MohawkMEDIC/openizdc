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
using Android.App;
using Android.Runtime;
using Android.Util;
using OpenIZ.Core;
using OpenIZ.Core.Applets;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Protocol;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Android.Configuration;
using OpenIZ.Mobile.Core.Android.Resources;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Configuration.Data;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Xamarin;
using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Xml.Serialization;
using A = Android;
using OpenIZ.Mobile.Core.Xamarin.Configuration;
using System.IO.Compression;
using System.Threading;
using Android.OS;
using OpenIZ.Core.Applets.Services;
using OpenIZ.Mobile.Core.Android.Services;

namespace OpenIZ.Mobile.Core.Android
{
    /// <summary>
    /// Represents an application context for Xamarin Android
    /// </summary>
    public class AndroidApplicationContext : XamarinApplicationContext
    {
        // The application
        private static readonly OpenIZ.Core.Model.Security.SecurityApplication c_application = new OpenIZ.Core.Model.Security.SecurityApplication()
        {
            ApplicationSecret = "C5B645B7D30A4E7E81A1C3D8B0E28F4C",
            Key = Guid.Parse("5248ea19-369d-4071-8947-413310872b7e"),
            Name = "org.openiz.openiz_mobile"
        };

        // Configuration manager
        private ConfigurationManager m_configurationManager;

        /// <summary>
        /// Static CTOR bind to global handlers to log errors
        /// </summary>
        /// <value>The current.</value>
        static AndroidApplicationContext()
        {
            Console.WriteLine("Binding global exception handlers");
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (AndroidApplicationContext.Current != null)
                {
                    Tracer tracer = Tracer.GetTracer(typeof(AndroidApplicationContext));
                    tracer.TraceEvent(EventLevel.Critical, "Uncaught exception: {0}", e.ExceptionObject.ToString());
                }
                Console.WriteLine("AndroindApplicationContext::UncaughtException", e.ExceptionObject.ToString());
            };
            AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
            {
                if (AndroidApplicationContext.Current != null)
                {
                    Tracer tracer = Tracer.GetTracer(typeof(AndroidApplicationContext));
                    tracer.TraceEvent(EventLevel.Critical, "Uncaught exception: {0}", e.Exception.ToString());
                }
                Console.WriteLine("AndroindApplicationContext::UncaughtException", e.Exception.ToString());
                e.Handled = true;
            };
        }

        /// <summary>
        /// Fired when no configuration is found
        /// </summary>
        public static event EventHandler NoConfiguration;

        /// <summary>
        /// Gets or sets the current activity
        /// </summary>
        public A.Content.Context CurrentActivity { get; set; }

        /// <summary>
        /// Start the application context
        /// </summary>
        public static bool Start(A.Content.Context launcherActivity, A.Content.Context context, A.App.Application application)
        {
            var retVal = new AndroidApplicationContext();
            retVal.Context = context;
            retVal.m_configurationManager = new ConfigurationManager();
            retVal.AndroidApplication = application;

            // Not configured
            if (!retVal.ConfigurationManager.IsConfigured)
            {
                NoConfiguration?.Invoke(null, EventArgs.Empty);
                return false;
            }
            else
            { // load configuration
                try
                {
                   
                    // Set master application context
                    ApplicationContext.Current = retVal;
                    retVal.CurrentActivity = launcherActivity;
                    try
                    {
                        retVal.ConfigurationManager.Load();
                        retVal.ConfigurationManager.Backup();
                    }
                    catch
                    {
                        if (retVal.ConfigurationManager.HasBackup() && 
                            retVal.Confirm(Strings.err_configuration_invalid_restore_prompt))
                        {
                            retVal.ConfigurationManager.Restore();
                            retVal.ConfigurationManager.Load();
                        }
                        else
                            throw;
                    }

                    retVal.AddServiceProvider(typeof(AndroidBackupService));

                    retVal.m_tracer = Tracer.GetTracer(typeof(AndroidApplicationContext), retVal.ConfigurationManager.Configuration);

                    // Is there a backup, and if so, does the user want to restore from that backup?
                    var backupSvc = retVal.GetService<IBackupService>();
                    if (backupSvc.HasBackup(BackupMedia.Public) &&
                        retVal.Configuration.GetAppSetting("ignore.restore") == null &&
                        retVal.Confirm(Strings.locale_confirm_restore))
                    {
                        backupSvc.Restore(BackupMedia.Public);
                    }

                    // Ignore restoration
                    if(!retVal.Configuration.GetSection<ApplicationConfigurationSection>().AppSettings.Any(o=>o.Key == "ignore.restore"))
                        retVal.Configuration.GetSection<ApplicationConfigurationSection>().AppSettings.Add(new AppSettingKeyValuePair()
                        {
                            Key = "ignore.restore",
                            Value = "true"
                        });

                    // HACK: For some reason the PCL doesn't do this automagically
                    //var connectionString = retVal.Configuration.GetConnectionString("openIzWarehouse");
                    //if (!File.Exists(connectionString.Value))
                    //{
                    //    retVal.m_tracer.TraceInfo("HAX: Creating warehouse file since PCL can't... {0}", connectionString.Value);
                    //    SqliteConnection.CreateFile(connectionString.Value);
                    //}
                    // Load configured applets
                    var configuredApplets = retVal.Configuration.GetSection<AppletConfigurationSection>().Applets;

                    retVal.SetProgress(context.GetString(Resource.String.startup_configuration), 0.2f);
                    var appletManager = retVal.GetService<IAppletManagerService>();

                    // Load all user-downloaded applets in the data directory
                    foreach (var appletInfo in configuredApplets)// Directory.GetFiles(this.m_configuration.GetSection<AppletConfigurationSection>().AppletDirectory)) {
                        try
                        {
                            retVal.m_tracer.TraceInfo("Loading applet {0}", appletInfo);
                            String appletPath = Path.Combine(retVal.Configuration.GetSection<AppletConfigurationSection>().AppletDirectory, appletInfo.Id);

                            if (!File.Exists(appletPath)) // reinstall
                            {
                                retVal.Configuration.GetSection<AppletConfigurationSection>().Applets.Clear();
                                retVal.SaveConfiguration();
                                retVal.Alert(Strings.locale_restartRequired);
                                throw new AppDomainUnloadedException();
                            }

                            // Load
                            using (var fs = File.OpenRead(appletPath))
                            {
                                AppletManifest manifest = AppletManifest.Load(fs);
                                // Is this applet in the allowed applets

                                // public key token match?
                                if (appletInfo.PublicKeyToken != manifest.Info.PublicKeyToken)
                                {
                                    retVal.m_tracer.TraceWarning("Applet {0} failed validation", appletInfo);
                                    ; // TODO: Raise an error
                                }

                                appletManager.LoadApplet(manifest);
                            }
                        }
                        catch(AppDomainUnloadedException) { throw; }
                        catch (Exception e)
                        {
                            retVal.m_tracer.TraceError("Applet Load Error: {0}", e);
                            if (retVal.Confirm(String.Format(Strings.err_applet_corrupt_reinstall, appletInfo.Id)))
                            {
                                String appletPath = Path.Combine(retVal.Configuration.GetSection<AppletConfigurationSection>().AppletDirectory, appletInfo.Id);
                                if (File.Exists(appletPath)) 
                                    File.Delete(appletPath);
                            }
                            else
                            {
                                retVal.m_tracer.TraceError("Loading applet {0} failed: {1}", appletInfo, e.ToString());
                                throw;
                            }
                        }

                    // Are we going to deploy applets
                    // Upgrade applets from our app manifest
                    foreach (var itm in context.Assets.List("Applets"))
                    {
                        try
                        {
                            retVal.m_tracer.TraceVerbose("Loading {0}", itm);
                            AppletPackage pkg = AppletPackage.Load(context.Assets.Open(String.Format("Applets/{0}", itm)));

                            // Write data to assets directory
#if !DEBUG
                            if (appletManager.GetApplet(pkg.Meta.Id) == null || new Version(appletManager.GetApplet(pkg.Meta.Id).Info.Version) < new Version(pkg.Meta.Version))
#endif
                                appletManager.Install(pkg, true);
                        }
                        catch (Exception e)
                        {
                            retVal.m_tracer?.TraceError(e.ToString());
                        }
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
                        retVal.SetProgress(context.GetString(Resource.String.startup_data), 0.6f);

                        DataMigrator migrator = new DataMigrator();
                        migrator.Ensure();

                        // Set the entity source
                        EntitySource.Current = new EntitySource(retVal.GetService<IEntitySourceProvider>());

                        ApplicationServiceContext.Current = ApplicationContext.Current;
                        ApplicationServiceContext.HostType = OpenIZHostType.MobileClient;

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

                    // Is there a backup manager? If no then we will use the default backup manager
                    

                    // Start daemons
                    ApplicationContext.Current.GetService<IUpdateManager>().AutoUpdate();
                    retVal.GetService<IThreadPoolService>().QueueNonPooledWorkItem(o => { retVal.Start(); }, null);
                    
                    // Set the tracer writers for the PCL goodness!
                    foreach (var itm in retVal.Configuration.GetSection<DiagnosticsConfigurationSection>().TraceWriter)
                        OpenIZ.Core.Diagnostics.Tracer.AddWriter(itm.TraceWriter, itm.Filter);
                }
                catch (Exception e)
                {
                    retVal.m_tracer?.TraceError(e.ToString());
                    //ApplicationContext.Current = null;
                    retVal.m_configurationManager = new Android.Configuration.ConfigurationManager(Android.Configuration.ConfigurationManager.GetDefaultConfiguration());
                    AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

                    throw;
                }

                return true;
            }
        }

        /// <summary>
        /// Starts the application context using in-memory default configuration for the purposes of
        /// configuring the software
        /// </summary>
        /// <returns><c>true</c>, if temporary was started, <c>false</c> otherwise.</returns>
        public static bool StartTemporary(A.Content.Context launcherActivity, A.Content.Context context)
        {
            try
            {
                var retVal = new AndroidApplicationContext();

                retVal.Context = context;
                retVal.CurrentActivity = launcherActivity;
                retVal.SetProgress(context.GetString(Resource.String.startup_setup), 0);
                retVal.ThreadDefaultPrincipal = AuthenticationContext.SystemPrincipal;

                retVal.m_configurationManager = new ConfigurationManager(OpenIZ.Mobile.Core.Android.Configuration.ConfigurationManager.GetDefaultConfiguration());
                ApplicationContext.Current = retVal;
                ApplicationServiceContext.Current = ApplicationContext.Current;
                ApplicationServiceContext.HostType = OpenIZHostType.MobileClient;

                retVal.m_tracer = Tracer.GetTracer(typeof(AndroidApplicationContext));

                try
                {
                    using (var fd = context.Assets.OpenFd("Applets/org.openiz.core.pak"))
                        retVal.m_tracer.TraceInfo("Installing org.openiz.core : {0} bytes", fd?.Length);
                    var package = AppletPackage.Load(context.Assets.Open("Applets/org.openiz.core.pak"));
                    retVal.GetService<IAppletManagerService>().Install(package, true);
                }
                catch (Exception e)
                {
                    retVal.m_tracer.TraceError(e.ToString());
                    throw;
                }


                retVal.Start();
                return true;
            }
            catch (Exception e)
            {
                Log.Error("OpenIZ 0118 999 881 999 119 7253", e.ToString());
                return false;
            }
        }
        
        /// <summary>
        /// Save the configuration
        /// </summary>
        public override void SaveConfiguration()
        {
            if(this.m_configurationManager.IsConfigured)
                this.m_configurationManager.Save();
        }


        #region implemented abstract members of ApplicationContext

        /// <summary>
        /// Gets or sets the android application
        /// </summary>
        public Application AndroidApplication { get; private set; }

        /// <summary>
        /// Gets the application information for the currently running application.
        /// </summary>
        /// <value>The application.</value>
        public override OpenIZ.Core.Model.Security.SecurityApplication Application
        {
            get
            {
                return c_application;
            }
        }

        /// <summary>
        /// Get the configuration
        /// </summary>
        /// <value>The configuration.</value>
        public override OpenIZ.Mobile.Core.Configuration.OpenIZConfiguration Configuration
        {
            get
            {
                return this.m_configurationManager.Configuration;
            }
        }

        /// <summary>
        /// Gets the current context
        /// </summary>
        public A.Content.Context Context { get; set; }

        /// <summary>
        /// Gets the device information for the currently running device
        /// </summary>
        /// <value>The device.</value>
        public override OpenIZ.Core.Model.Security.SecurityDevice Device
        {
            get
            {
                // TODO: Load this from configuration
                return new OpenIZ.Core.Model.Security.SecurityDevice()
                {
                    Name = this.Configuration.GetSection<SecurityConfigurationSection>().DeviceName,
                    DeviceSecret = this.Configuration.GetSection<SecurityConfigurationSection>().DeviceSecret
                };
            }
        }

        /// <summary>
        /// Get the configuration manager
        /// </summary>
        public override IConfigurationManager ConfigurationManager
        {
            get
            {
                return this.m_configurationManager;
            }
        }

        /// <summary>
        /// Close 
        /// </summary>
        public override void Exit()
        {
            A.App.Application.SynchronizationContext.Post(_ =>
            {
                this.m_tracer.TraceWarning("Restarting application context");
                ApplicationContext.Current.Stop();
                (this.CurrentActivity as Activity).Finish();
            }, null);
        }

        /// <summary>
        /// Confirm the alert
        /// </summary>
        public override bool Confirm(string confirmText)
        {
            AutoResetEvent evt = new AutoResetEvent(false);
            bool result = false;

            A.App.Application.SynchronizationContext.Post(_ =>
            {
                var alertDialogBuilder = new AlertDialog.Builder(this.CurrentActivity)
                        .SetMessage(confirmText)
                        .SetCancelable(false)
                        .SetPositiveButton(Strings.locale_confirm, (sender, args) =>
                        {
                            result = true;
                            evt.Set();
                        })
                        .SetNegativeButton(Strings.locale_cancel, (sender, args) =>
                        {
                            result = false;
                            evt.Set();
                        });

                alertDialogBuilder.Create().Show();
            }, null);

            evt.WaitOne();
            return result;
        }


        /// <summary>
        /// show toast
        /// </summary>
        public override void ShowToast(String message)
        {
	        if (Looper.MyLooper() == null)
	        {
		        Looper.Prepare();
	        }

            A.Widget.Toast.MakeText(this.CurrentActivity, message, A.Widget.ToastLength.Long);
        }

        /// <summary>
        /// Show an alert
        /// </summary>
        public override void Alert(string alertText)
        {
            AutoResetEvent evt = new AutoResetEvent(false);

            A.App.Application.SynchronizationContext.Post(_ =>
            {

                var alertDialogBuilder = new AlertDialog.Builder(this.CurrentActivity)
                         .SetMessage(alertText)
                        .SetCancelable(false)
                        .SetPositiveButton(Strings.locale_confirm, (sender, args) =>
                        {
                            evt.Set();
                        });

                alertDialogBuilder.Create().Show();
            }, null);

            evt.WaitOne();
        }

        /// <summary>
        /// Output performanc log info
        /// </summary>
        public override void PerformanceLog(string className, string methodName, string tagName, TimeSpan counter)
        {
            Log.Info("OpenIZ_PERF", $"{className}.{methodName}@{tagName} - {counter}");
        }

        #endregion implemented abstract members of ApplicationContext

        /// <summary>
        /// Provides a security key which is unique to the device
        /// </summary>
        public override byte[] GetCurrentContextSecurityKey()
        {
#if NOCRYPT
            return null;
#else
            var androidId = A.Provider.Settings.Secure.GetString(this.Context.ContentResolver, A.Provider.Settings.Secure.AndroidId);
            if (String.IsNullOrEmpty(androidId))
            {
                this.m_tracer.TraceWarning("Android ID cannot be found, databases will not be encrypted");
                return null; // can't encrypt
            }
            else
                return System.Text.Encoding.UTF8.GetBytes(androidId);
#endif
        }
    }
}