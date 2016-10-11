using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minims
{
    /// <summary>
    /// Console parameters to the mini ims
    /// </summary>
    public class ConsoleParameters
    {

        /// <summary>
        /// Applet directory 
        /// </summary>
        [Parameter("applet")]
        public StringCollection AppletDirectories { get; set; }

        /// <summary>
        /// Protocol directory
        /// </summary>
        [Parameter("protocols")]
        public StringCollection ProtocolDirectory { get; set; }
    }
}
