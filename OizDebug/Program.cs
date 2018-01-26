using MohawkCollege.Util.Console.Parameters;
using OizDebug.Options;
using OizDebug.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OizDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("OpenIZ Debugger v{0} ({1})", Assembly.GetEntryAssembly().GetName().Version, Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            Console.WriteLine("Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology");

            ParameterParser<DebuggerParameters> parser = new ParameterParser<DebuggerParameters>();
            var parameters = parser.Parse(args);

            if (parameters.Help)
                parser.WriteHelp(Console.Out);
            else if (parameters.Protocol)
                new ProtoDebugger(parameters).Debug();
            else if (parameters.BusinessRule)
                new BreDebugger(parameters).Debug();
            else
                Console.WriteLine("Nothing to do!");
        }
    }
}
