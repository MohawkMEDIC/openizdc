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
using OpenIZ.Mobile.Core.Services;
using Android.Webkit;
using Java.Interop;
using Newtonsoft.Json;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Xamarin.Security;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Security;
using OpenIZ.Mobile.Core.Xamarin;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
    /// <summary>
    /// Class represents service bridge to the session functions of OpenIZ
    /// </summary>
    public class SessionServiceBridge : Java.Lang.Object
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SessionServiceBridge));

      
        /// <summary>
        /// Get magic
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String GetMagic()
        {
            return ApplicationContext.Current.ExecutionUuid.ToString();
        }

		[Export]
		[JavascriptInterface]
		public string GetFormattedDate(string date, string format)
		{
			DateTime parsedDate;

			if (!DateTime.TryParse(date, out parsedDate))
			{
				return null;
			}

			return parsedDate.ToString(format);
		}
    }
}