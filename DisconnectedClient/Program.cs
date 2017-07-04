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
                var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenIZDC");
                var cData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenIZDC");
                if (Directory.Exists(appData)) Directory.Delete(cData, true);
                if (Directory.Exists(appData)) Directory.Delete(appData, true);
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
                uint x, y;
                Screen.PrimaryScreen.GetDpi(DpiType.Angular, out x, out y);
                var settings = new CefSettings();
                if (x > 120 || y > 120 || Program.Parameters.HdpiFix)
                    Cef.EnableHighDPISupport();
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
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
                if (!DcApplicationContext.StartContext())
                {
                    DcApplicationContext.StartTemporary();
                    var minims = XamarinApplicationContext.Current.GetService<MiniImsServer>();

                    DateTime start = new DateTime();

                    if (!minims.IsRunning)
                    {
                        minims.Started += startHandler;
                        while (!started && DateTime.Now.Subtract(start).TotalSeconds < 20)
                            Application.DoEvents();
                    }

                    main = new frmDisconnectedClient("http://127.0.0.1:9200/org.openiz.core/views/settings/splash.html");
                }
                else
                {

                    DcApplicationContext.Current.Started += startHandler;
                    while (!started)
                        Application.DoEvents();
                    main = new frmDisconnectedClient("http://127.0.0.1:9200/org.openiz.core/splash.html");
                }

                splash.Close();
                Application.Run(main);



            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
