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
 * Date: 2017-4-3
 */
#if !IE
using CefSharp;
#endif
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Xamarin;
using OpenIZ.Mobile.Core.Xamarin.Security;
using OpenIZ.Mobile.Core.Xamarin.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DisconnectedClient.Core;
using OpenIZ.Mobile.Core.Xamarin.Data;
using OpenIZ.Mobile.Core.Services;

namespace DisconnectedClient
{
    static class Program
    {
        // Trusted certificates
        private static List<String> s_trustedCerts = new List<string>();

        /// <summary>
        /// Console parameters
        /// </summary>
        public static ConsoleParameters Parameters { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            Program.Parameters = new ParameterParser<ConsoleParameters>().Parse(args);
            if (Program.Parameters.Debug)
                Console.WriteLine("Will start in debug mode...");
            if (Program.Parameters.Reset)
            {
                if (MessageBox.Show("Are you sure you want to wipe all your data and configuration for the Disconnected Client?", "Confirm Reset", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenIZDC");
                    var cData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenIZDC");
                    if (Directory.Exists(appData)) Directory.Delete(cData, true);
                    if (Directory.Exists(appData)) Directory.Delete(appData, true);
                }
                return;
            }
            String[] directory = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenIZDC"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenIZDC")
            };

            foreach (var dir in directory)
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

            // Token validator
            TokenValidationManager.SymmetricKeyValidationCallback += (o, k, i) =>
            {
                return MessageBox.Show(String.Format("Trust issuer {0} with symmetric key?", i), "Token Validation Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;
            };
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, error) =>
            {
                if (certificate == null || chain == null)
                    return false;
                else
                {
                    var valid = s_trustedCerts.Contains(certificate.Subject);
                    if (!valid && (chain.ChainStatus.Length > 0 || error != SslPolicyErrors.None))
                        if (MessageBox.Show(String.Format("The remote certificate is not trusted. The error was {0}. The certificate is: \r\n{1}\r\nWould you like to temporarily trust this certificate?", error, certificate.Subject), "Certificate Error", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                            return false;
                        else
                            s_trustedCerts.Add(certificate.Subject);

                    return true;
                    //isValid &= chain.ChainStatus.Length == 0;
                }
            };

            // Start up!!!
            try
            {

#if IE
#else
                var settings = new CefSettings() { UserAgent = "OpenIZEmbedded" };
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
                Cef.EnableHighDPISupport();
#endif

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                frmSplash splash = new frmSplash();
                splash.Show();

                bool started = false;
                EventHandler startHandler = (o, e) =>
                {
                    started = true;
                };

                frmDisconnectedClient main = null;
                DateTime start = new DateTime();

                if (!DcApplicationContext.StartContext(new WinFormsDialogProvider()))
                {
					DcApplicationContext.StartTemporary(new WinFormsDialogProvider());
                    var minims = XamarinApplicationContext.Current.GetService<MiniImsServer>();


                    if (!minims.IsRunning)
                    {
                        minims.Started += startHandler;
                        while (!started && DateTime.Now.Subtract(start).TotalSeconds < 20 && splash.Visible)
                            Application.DoEvents();
                    }

                    if (minims.IsRunning)
                        main = new frmDisconnectedClient("http://127.0.0.1:9200/org.openiz.core/views/settings/splash.html");
                    else return;
                }
                else 
                {


                    DcApplicationContext.Current.Started += startHandler;
                    while (!started && splash.Visible)
                        Application.DoEvents();

                    main = new frmDisconnectedClient("http://127.0.0.1:9200/org.openiz.core/splash.html");
                }

                splash.Close();

                if(XamarinApplicationContext.Current.GetService<MiniImsServer>().IsRunning)
                    Application.Run(main);

            }
            catch (Exception e)
            {

                if (MessageBox.Show(String.Format(DisconnectedClient.Core.Resources.Strings.err_startup, e.Message), "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var bksvc = XamarinApplicationContext.Current.GetService<IBackupService>();
                    bksvc.Backup(OpenIZ.Mobile.Core.Services.BackupMedia.Public);
                    File.Delete(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "OpenIZDC", "OpenIZ.config"));
                    Directory.Delete(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "OpenIZDC"), true);
                }
                else
                {
                    var bksvc = XamarinApplicationContext.Current.GetService<IBackupService>();
                    bksvc.Backup(OpenIZ.Mobile.Core.Services.BackupMedia.Private);
                    File.Delete(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "OpenIZ.config"));
                    Directory.Delete(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "OpenIZDC"), true);
                }
                Application.Exit();
                Environment.Exit(996);
            }
        }
    }
}
