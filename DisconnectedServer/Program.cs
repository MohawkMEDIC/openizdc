using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Mobile.Core.Xamarin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DisconnectedClient.Core;
using System.Net;
using System.Net.Security;
using System.Reflection;
using OpenIZ.Mobile.Core.Xamarin;

namespace DisconnectedServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            var consoleArgs = new ParameterParser<ConsoleParameters>().Parse(args);

            if (consoleArgs.Help)
            {
                // Start up!!!
                Console.WriteLine("OpenIZ Disconnected Server");
                Console.WriteLine("Version {0}", Assembly.GetEntryAssembly().GetName().Version);
                new ParameterParser<ConsoleParameters>().WriteHelp(Console.Out);
            }
            else if (consoleArgs.Reset)
            {
                var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenIZDS");
                var cData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenIZDS");
                if (Directory.Exists(appData)) Directory.Delete(cData, true);
                if (Directory.Exists(appData)) Directory.Delete(appData, true);
                Console.WriteLine("Environment Reset Successful");
                return;
            }
            else if (!consoleArgs.ConsoleMode)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new DisconnectedClientService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                String[] directory = {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenIZDS"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenIZDS")
                };

                foreach (var dir in directory)
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                // Token validator
                TokenValidationManager.SymmetricKeyValidationCallback += (o, k, i) =>
                {
                    Console.WriteLine("Trust issuer {0} failed", i);
                    return false;
                };
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, error) =>
                {
                    if (certificate == null || chain == null)
                        return false;
                    else
                    {
                        if (chain.ChainStatus.Length > 0 || error != SslPolicyErrors.None)
                        {
                            Console.WriteLine("The remote certificate is not trusted. The error was {0}. The certificate is: \r\n{1}", error, certificate.Subject);
                            return false;
                        }
                        return true;
                    }
                };

                // Start up!!!
                Console.WriteLine("OpenIZ Disconnected Server");
                Console.WriteLine("Version {0}", Assembly.GetEntryAssembly().GetName().Version);


                XamarinApplicationContext.ProgressChanged += (o, e) =>
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(">>> PROGRESS >>> {0} : {1:#0%}", e.ProgressText, e.Progress);
                    Console.ResetColor();
                };

                if (!DcApplicationContext.StartContext(new ConsoleDialogProvider(), "OpenIZDS"))
                    DcApplicationContext.StartTemporary(new ConsoleDialogProvider(), "OpenIZDS");

                Console.WriteLine("Press [Enter] key to close...");
                Console.ReadLine();
            }
        }
    }
}
