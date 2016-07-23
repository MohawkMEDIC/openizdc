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
using OpenIZ.Mobile.Core.Android.Security;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Security;

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
            AndroidApplicationContext.Current.SetPrincipal(null);
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