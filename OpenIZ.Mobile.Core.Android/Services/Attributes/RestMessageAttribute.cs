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
    /// REST Message format
    /// </summary>
    public enum RestMessageFormat
    {
        Json,
        Xml,
        FormData,
        Raw,
        SimpleJson
    }

    /// <summary>
    /// REST message format attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    public class RestMessageAttribute : Attribute
    {

        /// <summary>
        /// REST message format ctor
        /// </summary>
        /// <param name="format"></param>
        public RestMessageAttribute(RestMessageFormat format)
        {
            this.MessageFormat = format;
        }

        /// <summary>
        /// Message format
        /// </summary>
        public RestMessageFormat MessageFormat { get; set; }

    }
}