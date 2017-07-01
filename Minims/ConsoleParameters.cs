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
 * Date: 2017-3-31
 */
using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        [Description("Identifies the source code directories to debug")]
        public StringCollection AppletDirectories { get; set; }

        /// <summary>
        /// Applet directory 
        /// </summary>
        [Parameter("ref")]
        [Description("Adds a reference to the current IMS session")]
        public StringCollection References { get; set; }


        /// <summary>
        /// Show help and exit
        /// </summary>
        [Parameter("help")]
        [Description("Shows help and exits")]
        public bool Help { get; set; }

        /// <summary>
        /// Instructs the minims to remove itself
        /// </summary>
        [Parameter("reset")]
        [Description("Deletes all configuration data restoring the MiniIMS to its default state")]
        public bool Reset { get; set; }
    }
}
