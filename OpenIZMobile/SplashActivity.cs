﻿
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

namespace OpenIZMobile
{
    [Activity(Label = "@string/app_name", Theme = "@style/OpenIZ.Splash", MainLauncher = true, Icon = "@mipmap/icon", NoHistory = true)]
    public class SplashActivity : Activity
    {

        // Tracer
        private Tracer m_tracer;

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
                typeof(OpenIZConfiguration).Assembly.GetName().Version,
                typeof(OpenIZConfiguration).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
            );

            CancellationTokenSource ctSource = new CancellationTokenSource();
            CancellationToken ct = ctSource.Token;

            Task startupWork = new Task(() =>
            {
                Task.Delay(1000);
                if (!this.DoConfigure())
                    ctSource.Cancel();
            }, ct);

            startupWork.ContinueWith(t =>
            {
                if (!ct.IsCancellationRequested)
                {
                    Intent viewIntent = new Intent(this, typeof(AppletActivity));
                    var appletConfig = AndroidApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>();
                    viewIntent.PutExtra("assetLink", "http://127.0.0.1:9200/" + appletConfig.StartupAsset + "/index.html");
                    this.StartActivity(viewIntent);

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

                if (!AndroidApplicationContext.Start(this.ApplicationContext))
                {

                    CancellationTokenSource ctSource = new CancellationTokenSource();
                    CancellationToken ct = ctSource.Token;

                    Task notifyUserWork = new Task(() =>
                    {

                        try
                        {

                            if (AndroidApplicationContext.StartTemporary(this.ApplicationContext))
                            {
                                this.m_tracer = Tracer.GetTracer(typeof(SplashActivity));

                                try
                                {
                                    using (var gzs = new GZipStream(Assets.Open("Applets/openiz.core.applet.pak"), CompressionMode.Decompress))
                                    {
                                        // Write data to assets directory
                                        var package = AppletPackage.Load(gzs);
                                        AndroidApplicationContext.Current.InstallApplet(package, true);
                                    }
                                }
                                catch (Exception e)
                                {
                                    this.m_tracer.TraceError(e.ToString());
                                    throw;
                                }

                            }
                            else
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
                            Intent viewIntent = new Intent(this, typeof(AppletActivity));
                            viewIntent.PutExtra("assetLink", "http://127.0.0.1:9200/org.openiz.core/views/settings/index.html");
                            viewIntent.PutExtra("continueTo", typeof(SplashActivity).AssemblyQualifiedName);
                            this.StartActivity(viewIntent);

                        }
                    }, TaskScheduler.Current);

                    notifyUserWork.Start();
                    return false;
                }
                else
                {


                    this.m_tracer = Tracer.GetTracer(this.GetType());

                    // Upgrade applets from our app manifest
                    foreach (var itm in Assets.List("Applets"))
                    {
                        try
                        {
                            this.m_tracer.TraceVerbose("Loading {0}", itm);
                            AppletPackage pkg = null;
                            if (Path.GetExtension(itm) == ".pak")
                            {
                                using (var gzs = new GZipStream(Assets.Open(String.Format("Applets/{0}", itm)), CompressionMode.Decompress))
                                    pkg = AppletPackage.Load(gzs);

                            }
                            else
                            {
                                AppletManifest manifest = AppletManifest.Load(Assets.Open(String.Format("Applets/{0}", itm)));
                                pkg = manifest.CreatePackage();
                            }

                            // Write data to assets directory
#if !DEBUG
                            if(AndroidApplicationContext.Current.GetApplet(pkg.Meta.Id) == null ||
                                new Version(AndroidApplicationContext.Current.GetApplet(pkg.Meta.Id).Info.Version) < new Version(pkg.Meta.Version))
#endif       
                            AndroidApplicationContext.Current.InstallApplet(pkg, true);
                        }
                        catch (Exception e)
                        {
                            this.m_tracer?.TraceError(e.ToString());
                        }
                    }
                    AndroidApplicationContext.ProgressChanged -= this.OnProgressUpdated;
                }


                return true;
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
                    this.Finish();
                },
                "{0} : {1}", Resources.GetString(Resource.String.err_startup), e is TargetInvocationException ? e.InnerException.Message : e.Message);
        }
    }
}

