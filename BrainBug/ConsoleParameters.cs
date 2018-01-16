/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBug
{
    /// <summary>
    /// Console parameters
    /// </summary>
    public class ConsoleParameters
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ConsoleParameters()
        {
            this.BackupFile = Path.GetTempFileName();
            this.TargetFile = Path.GetTempFileName();
            this.SdkPath = @"c:\Program Files (x86)\Android\android-sdk\";
        }

        /// <summary>
        /// Gets or sets the device id
        /// </summary>
        [Parameter("d")]
        [Parameter("device")]
        public String DeviceId { get; set; }

        /// <summary>
        /// Shows help
        /// </summary>
        [Parameter("help")]
        [Parameter("?")]
        [Description("Shows help and exits the programm")]
        public bool Help { get; set; }

        /// <summary>
        /// Package identifier
        /// </summary>
        [Parameter("pkgid")]
        [Description("The android package identifier to extract")]
        public String PackageId { get; set; }

        /// <summary>
        /// Gets or sets the path to the sdk
        /// </summary>
        [Parameter("sdk")]
        [Description("The path to the android SDK")]
        public String SdkPath { get; set; }

        /// <summary>
        /// Backup file
        /// </summary>
        [Parameter("bkfile")]
        [Description("The name of the AB (raw) backup file")]
        public String BackupFile { get; set; }

        /// <summary>
        /// Output directory
        /// </summary>
        [Parameter("tar")]
        [Description("The name of the TAR file to extract from the AB")]
        public String TargetFile { get; set; }

        /// <summary>
        /// Output directory
        /// </summary>
        [Parameter("extract")]
        [Description("The directory to extract the extracted data to")]
        public String ExtractDir { get; set; }


    }
}
