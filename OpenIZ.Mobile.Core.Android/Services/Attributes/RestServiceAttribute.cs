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
 * Date: 2016-7-23
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OpenIZ.Mobile.Core.Android.Services.Attributes
{
    /// <summary>
    /// REST service attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RestServiceAttribute : Attribute
    {
        /// <summary>
        /// REST Service attribute for base address
        /// </summary>
        public RestServiceAttribute(string baseAddress)
        {
            this.BaseAddress = baseAddress;
        }

        /// <summary>
        /// Gets or sets the base address of the service
        /// </summary>
        public String BaseAddress { get; set; }

    }
}