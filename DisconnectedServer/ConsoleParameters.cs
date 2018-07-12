using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisconnectedServer
{
    /// <summary>
    /// Console parameters
    /// </summary>
    public class ConsoleParameters : DisconnectedClient.Core.ConsoleParameters
    {

        /// <summary>
        /// Whether to run on console
        /// </summary>
        [Parameter("console")]
        [Description("Run the server in console (opposed to service) mode")]
        public bool ConsoleMode { get; set; }

        /// <summary>
        /// Gets the help
        /// </summary>
        [Parameter("help")]
        [Description("Show this help")]
        public bool Help { get; set; }
    }
}
