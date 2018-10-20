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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Android.Configuration;
using System.IO;
using System.Threading.Tasks;
using Android.Util;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Exceptions;
using System.Threading;
using OpenIZ.Mobile.Core.Android;
using OpenIZ.Core.Applets.Model;
using System.Xml.Serialization;
using OpenIZ.Mobile.Core.Android.Services;
using OpenIZ.Mobile.Core;
using System.IO.Compression;
using OpenIZ.Protocol.Xml.Model;
using OpenIZ.Protocol.Xml;
using OpenIZ.Mobile.Core.Xamarin;
using OpenIZ.Mobile.Core.Xamarin.Services;
using OpenIZ.Mobile.Core.Xamarin.Data;
using OpenIZ.Mobile.Core.Services;
using Android.Content.PM;

namespace OpenIZMobile
{
    [Activity(Label = "@string/app_name", Theme = "@style/OpenIZ.Splash", MainLauncher = true, Icon = "@mipmap/icon", NoHistory = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SplashActivity : Activity, IOperatingSystemSecurityService
    {

        private ManualResetEvent m_permissionEvent = new ManualResetEvent(false);

        /// <summary>
        /// Return true if this object has permission
        /// </summary>
        public bool HasPermission(PermissionType permission)
        {
            if ((int)Build.VERSION.SdkInt < 23)
                return true;
            else
                switch (permission)
                {
                    case PermissionType.FileSystem:
                        return this.CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted;
                    case PermissionType.GeoLocation:
                        return this.CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted;
                    default:
                        return false;
                }
        }

        /// <summary>
        /// Requests permission
        /// </summary>
        public bool RequestPermission(PermissionType permission)
        {
            if ((int)Build.VERSION.SdkInt < 23)
                return true;
            else
            {
                this.m_permissionEvent.Reset();
                String permissionString = String.Empty;
                switch (permission)
                {
                    case PermissionType.FileSystem:
                        permissionString = Android.Manifest.Permission.WriteExternalStorage;
                        break;
                    case PermissionType.GeoLocation:
                        permissionString = Android.Manifest.Permission.AccessCoarseLocation;
                        break;
                    default:
                        return false;
                }
                this.RequestPermissions(new string[] { permissionString }, 0);
                this.m_permissionEvent.WaitOne();
                return this.CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted;
            }
        }

        /// <summary>
        /// Request permission result
        /// </summary>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            this.m_permissionEvent.Set();
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        // Tracer
        private Tracer m_tracer;

        /// <param name="newConfig">The new device configuration.</param>
        /// <summary>
        /// Configuration changed
        /// </summary>
        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
        }

        /// <summary>
        /// Progress has changed
        /// </summary>
        private void OnProgressUpdated(Object sender, ApplicationProgressEventArgs e)
        {
            this.RunOnUiThread(() => this.FindViewById<TextView>(Resource.Id.txt_splash_info).Text = String.Format("{0} {1}", e.ProgressText, e.Progress > 0 ? String.Format("({0:0%})", e.Progress) : null));
        }

        /// <summary>
        /// Create the activity
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.Splash);

            OpenIZ.Mobile.Core.ApplicationContext.Current = null;

            this.FindViewById<TextView>(Resource.Id.txt_splash_version).Text = String.Format("V {0} ({1})",
                typeof(SplashActivity).Assembly.GetName().Version,
                typeof(SplashActivity).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
            );

            CancellationTokenSource ctSource = new CancellationTokenSource();
            CancellationToken ct = ctSource.Token;

            Task startupWork = new Task(() =>
            {
                if (XamarinApplicationContext.Current == null)
                    if (!this.DoConfigure())
                        ctSource.Cancel();
            }, ct);


            startupWork.ContinueWith(t =>
            {
                if (!ct.IsCancellationRequested)
                {
                    Action doStart = () =>
                    {
                        AndroidApplicationContext.ProgressChanged -= this.OnProgressUpdated;
                        Intent viewIntent = new Intent(this, typeof(AppletActivity));
                        var appletConfig = AndroidApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>();
                        viewIntent.PutExtra("assetLink", "http://127.0.0.1:9200/" + appletConfig.StartupAsset + "/splash.html#/");
                        this.StartActivity(viewIntent);
                    };
                    if (AndroidApplicationContext.Current.GetService<MiniImsServer>().IsRunning)
                        doStart();
                    else
                        AndroidApplicationContext.Current.GetService<MiniImsServer>().Started += (oo, oe) =>
                        {
                            doStart();
                        };
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());


            startupWork.Start();

        }

        /// <summary>
        /// Startup is complete
        /// </summary>
        private bool DoConfigure()
        {

            this.RunOnUiThread(() => this.FindViewById<TextView>(Resource.Id.txt_splash_info).Text = Resources.GetString(Resource.String.startup));
            AndroidApplicationContext.ProgressChanged += this.OnProgressUpdated;

            try
            {

                if (AndroidApplicationContext.Current != null)
                    return true;

                if (!AndroidApplicationContext.Start(this, this.ApplicationContext, this.Application))
                {

                    CancellationTokenSource ctSource = new CancellationTokenSource();
                    CancellationToken ct = ctSource.Token;

                    Task notifyUserWork = new Task(() =>
                    {

                        try
                        {

                            if (!AndroidApplicationContext.StartTemporary(this, this.ApplicationContext))
                                throw new InvalidOperationException("Cannot start temporary authentication pricipal");

                        }
                        catch (Exception e)
                        {
                            this.m_tracer.TraceError(e.ToString());
                            ctSource.Cancel();
                            this.ShowException(e);
                        }
                        finally
                        {
                            AndroidApplicationContext.ProgressChanged -= this.OnProgressUpdated;
                        }
                    }, ct);

                    // Now show the configuration screen.
                    notifyUserWork.ContinueWith(t =>
                    {
                        if (!ct.IsCancellationRequested)
                        {
                           
                            Action doStart = () =>
                            {
                                Intent viewIntent = new Intent(this, typeof(AppletActivity));
                                viewIntent.PutExtra("assetLink", "http://127.0.0.1:9200/org.openiz.core/views/settings/splash.html");
                                viewIntent.PutExtra("continueTo", typeof(SplashActivity).AssemblyQualifiedName);
                                this.StartActivity(viewIntent);
                            };
                            if (AndroidApplicationContext.Current.GetService<MiniImsServer>().IsRunning)
                                doStart();
                            else
                                AndroidApplicationContext.Current.GetService<MiniImsServer>().Started += (oo, oe) =>
                                {
                                    doStart();
                                };
                        }
                    }, TaskScheduler.Current);

                    notifyUserWork.Start();
                    return false;
                }
                else
                {

                    this.m_tracer = Tracer.GetTracer(this.GetType());
                }


                return true;
            }
            catch (AppDomainUnloadedException)
            {
                this.Finish();
                return false;
            }
            catch (Exception e)
            {

                this.ShowException(e);
                return false;
            }

        }

        /// <summary>
        /// Shows an exception message box
        /// </summary>
        /// <param name="e">E.</param>
        private void ShowException(Exception e)
        {
            this.m_tracer?.TraceError("Error during startup: {0}", e);
            Log.Error("FATAL", e.ToString());
            while (e is TargetInvocationException)
                e = e.InnerException;

            var result = false;
            AutoResetEvent reset = new AutoResetEvent(false);
            UserInterfaceUtils.ShowConfirm(this,
                (s, a) =>
                {
                    result = true;
                    reset.Set();
                },
                (s,a) =>
                {
                    result = false;
                    reset.Set();
                },
                Resources.GetString(Resource.String.err_startup), e is TargetInvocationException ? e.InnerException.Message : e.Message);

            reset.WaitOne();

            var bksvc = XamarinApplicationContext.Current.GetService<IBackupService>();
            bksvc.Backup(result ? OpenIZ.Mobile.Core.Services.BackupMedia.Public : BackupMedia.Private);
            File.Delete(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "OpenIZ.config"));
            Directory.Delete(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)), true);
            this.Finish();

        }
    }
}

