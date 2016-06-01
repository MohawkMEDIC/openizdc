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
        /// Session information
        /// </summary>
        [JsonObject]
        private class SessionInformation
        {
            /// <summary>
            /// Default ctor
            /// </summary>
            public SessionInformation()
            {

            }

            /// <summary>
            /// Create the session object from the principal
            /// </summary>
            public SessionInformation(IPrincipal principal)
            {
                this.UserName = principal.Identity.Name;
                this.IsAuthenticated = principal.Identity.IsAuthenticated;
                this.AuthenticationType = principal.Identity.AuthenticationType;
                if (principal is TokenClaimsPrincipal)
                    this.Token = principal.ToString();

                // Expiry / etc
                if(principal is ClaimsPrincipal)
                {
                    var cp = principal as ClaimsPrincipal;
                    this.Issued = DateTime.Parse(cp.FindClaim(ClaimTypes.AuthenticationInstant).Value);
                    this.Expiry = DateTime.Parse(cp.FindClaim(ClaimTypes.Expiration).Value);
                    this.Roles = cp.Claims.Where(o => o.Type == ClaimsIdentity.DefaultRoleClaimType).Select(o => o.Value).ToArray();
                    this.AuthenticationType = cp.FindClaim(ClaimTypes.AuthenticationMethod).Value;
                }
                else
                {
                    IRoleProviderService rps = ApplicationContext.Current.GetService<IRoleProviderService>();
                    this.Roles = rps.GetAllRoles(this.UserName);
                    this.Issued = DateTime.Now;
                    this.Expiry = DateTime.MaxValue;
                }
            }

            /// <summary>
            /// Gets the user name
            /// </summary>
            [JsonProperty("username")]
            public string UserName { get; set; }

            /// <summary>
            /// Gets the roles to which the identity belongs
            /// </summary>
            [JsonProperty("roles")]
            public String[] Roles { get; set; }

            /// <summary>
            /// True if authenticated
            /// </summary>
            [JsonProperty("isAuthenticated")]
            public bool IsAuthenticated { get; set; }

            /// <summary>
            /// Gets or sets the mechanism
            /// </summary>
            [JsonProperty("method")]
            public String AuthenticationType { get; set; }

            /// <summary>
            /// Expiry time
            /// </summary>
            [JsonProperty("exp")]
            public DateTime Expiry { get; set; }

            /// <summary>
            /// Issued time
            /// </summary>
            [JsonProperty("nbf")]
            public DateTime Issued { get; set; }

            /// <summary>
            /// Gets or sets the JWT token
            /// </summary>
            [JsonProperty("jwt")]
            public String Token { get; set; }

            /// <summary>
            /// Gets or sets the refresh token
            /// </summary>
            [JsonProperty("refresh_token")]
            public String RefreshToken { get; set; }
        }

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
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error authenticating: {0}", e);
                return e.Message;
            }
        }

    }
}