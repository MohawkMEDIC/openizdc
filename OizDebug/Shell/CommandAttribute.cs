﻿/*
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OizDebug.Shell
{
    /// <summary>
    /// Represents a debugger command 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {

        /// <summary>
        /// Ctore for debugger command
        /// </summary>
        public CommandAttribute(String cmd, String desc)
        {
            this.Command = cmd;
            this.Description = desc;
        }

        /// <summary>
        /// The command to be run
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The command to be run
        /// </summary>
        public string Description { get; set; }

    }
}
