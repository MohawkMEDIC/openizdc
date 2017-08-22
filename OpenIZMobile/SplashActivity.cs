/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-10-11
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

namespace OpenIZMobile
{
    [Activity(Label = "@string/app_name", Theme = "@style/OpenIZ.Splash", MainLauncher = true, Icon = "@mipmap/icon", NoHistory = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SplashActivity : Activity
    {

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
        }

        protected override void OnResume()
        {
            base.OnResume();
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
            UserInterfaceUtils.ShowMessage(this,
                (s, a) =>
                {
                    XamarinApplicationContext.Current?.SetProgress("Backing up...", 0.1f);
                    var fils = Directory.GetFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "*.sqlite");
                    // Perform a backup if possible
                    for (int i = 0; i < fils.Length; i++)
                    {
                        try
                        {
                            XamarinApplicationContext.Current?.SetProgress(String.Format("Backing up {0}...", Path.GetFileNameWithoutExtension(fils[i])), (float)i / (float)fils.Length);
                            File.Copy(fils[i], Path.ChangeExtension(fils[i], ".bak"), true);
                            File.Delete(fils[i]);
                        }
                        catch
                        {
                            
                        }
                    }
                    File.Delete(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "OpenIZ.config"));
                    this.Finish();
                },
                Resources.GetString(Resource.String.err_startup), e is TargetInvocationException ? e.InnerException.Message : e.Message);
        }
    }
}

