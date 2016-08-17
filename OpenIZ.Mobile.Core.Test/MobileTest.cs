/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-14
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
    /// <summary>
    /// Represents a simple mobile test which sets up context
    /// </summary>
    [DeploymentItem("sqlite3.dll")]
    [DeploymentItem("OpenIZ.sqlite")]
    public abstract class MobileTest
    {

        /// <summary>
        /// Mobile test
        /// </summary>
        public MobileTest()
        {
            ApplicationContext.Current = new TestApplicationContext();
        }
    }
}