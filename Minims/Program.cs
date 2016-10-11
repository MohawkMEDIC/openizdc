using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Xamarin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minims
{
    class Program
    {
        
        static void Main(string[] args)
        {

            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MINIMS");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            // Start up!!!
            var consoleArgs = new ParameterParser<ConsoleParameters>().Parse(args);

            if (!MiniApplicationContext.Start(consoleArgs))
            {
                MiniApplicationContext.StartTemporary(consoleArgs);
                // Forward
                Process pi = Process.Start("http://127.0.0.1:9200/org.openiz.core/views/settings/index.html");
            }
            else
            {
                var appletConfig = XamarinApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>();
                Process pi = Process.Start("http://127.0.0.1:9200/" + appletConfig.StartupAsset + "/index.html");

            }
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}
