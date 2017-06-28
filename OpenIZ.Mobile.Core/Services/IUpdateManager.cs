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
using OpenIZ.Core.Applets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Update manager service is responsible for checking for updates and downloading / applying them
    /// </summary>
    public interface IUpdateManager 
    {

        /// <summary>
        /// Get server version of a package
        /// </summary>
        AppletInfo GetServerVersion(String packageId);

        /// <summary>
        /// Install the specified package from the server version
        /// </summary>
        void Install(String packageId);

        /// <summary>
        /// Check for updates
        /// </summary>
        void AutoUpdate();
    }
}
