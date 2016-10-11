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
        /// Get session information
        /// </summary>
        [JavascriptInterface]
        [Export]
        public String GetSession()
        {
            if (ApplicationContext.Current.Principal == null)
                return null;
            else
                return JniUtil.ToJson(new SessionInformation(ApplicationContext.Current.Principal));
        }

        /// <summary>
        /// Abandons the session
        /// </summary>
        [Export]
        [JavascriptInterface]
        public void Abandon()
        {
            XamarinApplicationContext.Current.SetPrincipal(null);
        }

        /// <summary>
        /// Refresh the current session
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String Refresh()
        {
            try
            {
                var idp = ApplicationContext.Current.GetService<IIdentityProviderService>();
                var principal = idp.Authenticate(ApplicationContext.Current.Principal, null); // Force a re-issue
                AndroidApplicationContext.Current.SetPrincipal(principal);
                return JniUtil.ToJson(new SessionInformation(ApplicationContext.Current.Principal));
            }
            catch (SecurityException e)
            {
                this.m_tracer.TraceError("Security exception refreshing session: {0}", e);
                if (e.Data.Contains("detail"))
                    return JniUtil.ToJson(e.Data["detail"]);
                return e.Message;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error refreshing session: {0}", e);
                return e.Message;
            }
        }

        /// <summary>
        /// Authenticates username / password
        /// </summary>
        [JavascriptInterface]
        [Export]
        public String Login(String userName, String password)
        {
            try
            {
                AndroidApplicationContext.Current.Authenticate(userName, password);
                
                if (ApplicationContext.Current.Principal == null)
                    return null;
                else
                {
                    return JniUtil.ToJson(new SessionInformation(ApplicationContext.Current.Principal));
                }
            }
            catch(SecurityException e)
            {
                this.m_tracer.TraceError("Security Exception: {0}", e);
                if (e.Data.Contains("detail"))
                    return JniUtil.ToJson(e.Data["detail"]);
                return "err_invalid_" + e.Message;
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error authenticating: {0}", e);
                return e.Message;
            }
        }

    }
}