using System.Linq;
using System;
using Android.Webkit;
using Java.Interop;
using Android.Widget;
using Android.Content;
using Org.Json;
using Android.App;
using Android.Content.Res;
using OpenIZ.Mobile.Core.Diagnostics;
using System.IO;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	
	/// <summary>
	/// Applet functions which are related to javascript
	/// </summary>
	public class AppletFunctionBridge : Java.Lang.Object
	{

		// Context
		private Context m_context;
		private AppletWebView m_view;
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletFunctionBridge));

		/// <summary>
		/// Gets the context of the function
		/// </summary>
		/// <param name="context">Context.</param>
		public AppletFunctionBridge (Context context, AppletWebView view)
		{
			
			this.m_context = context;
			this.m_view = view;
		}

		/// <summary>
		/// Shows the specified message as a toast
		/// </summary>
		[Export]
		[JavascriptInterface]
		public void ShowToast(String toastText)
		{
			Toast.MakeText (this.m_context, toastText, ToastLength.Short).Show();
		}

		/// <summary>
		/// Navigate the specified appletId and context.
		/// </summary>
		/// <param name="appletId">Applet identifier.</param>
		/// <param name="context">Context.</param>
		[Export]
		[JavascriptInterface]
		public void Navigate(String appletId, String context)
		{
			// TODO: Parameters
			Application.SynchronizationContext.Post (_ => {
				this.m_view.LoadUrl(String.Format("app://openiz.org/applet/{0}/", appletId));
			}, null);
		}

		/// <summary>
		/// Get the specified string
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="stringId">String identifier.</param>
		[Export]
		[JavascriptInterface]
		public String GetString(String stringId)
		{
            try
            {
                var appletResource = AndroidApplicationContext.Current.LoadedApplets.GetStrings(this.GetLocale()).FirstOrDefault(o => o.Key == stringId).Value;
                return appletResource ?? this.m_context.Resources.GetString(this.m_context.Resources.GetIdentifier(stringId, "string", this.m_context.PackageName));
            }
            catch(Exception e)
            {
                this.m_tracer.TraceWarning("Error retreiving string {0}", stringId);
                return stringId;
            }
		}

		/// <summary>
		/// Go back home
		/// </summary>
		[Export]
		[JavascriptInterface]
		public void Back()
		{
			Application.SynchronizationContext.Post (_ => {
				if (this.m_view.CanGoBack ())
					this.m_view.GoBack ();
				else
					(this.m_context as Activity).Finish ();
			}, null);
		}


		/// <summary>Close the applet</summary>
		[Export]
		[JavascriptInterface]
		public void Close() {
			Application.SynchronizationContext.Post (_ => {
				(this.m_context as Activity).Finish ();
			}, null);
		}

		/// <summary>
		/// Performs a barcode scan
		/// </summary>
		[Export]
		[JavascriptInterface]
		public String BarcodeScan()
		{
			try
			{
				return "XXXXXXXXXXXX";
			}
			catch(Exception e) {
				this.ShowToast (e.Message);
				return null;
			}
		}

        /// <summary>
        /// Get the locale of the user interface
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String GetLocale()
        {
            return this.m_context.Resources.Configuration.Locale.Language;
        }

        /// <summary>
        /// Get the applet strings
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String GetStrings(String locale)
        {
            var strings = AndroidApplicationContext.Current.LoadedApplets.GetStrings(locale);

            using (StringWriter sw = new StringWriter())
            {
                sw.Write("{");
                foreach (var itm in strings)
                {
                    sw.Write("\"{0}\":\"{1}\",", itm.Key, itm.Value);
                }
                sw.Write("\"locale\":\"{0}\" }}", locale);
                return sw.ToString();
            }
            
        }

        /// <summary>
        /// Set the locale of the application
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String SetLocale(String locale)
        {
            try
            {
                this.m_tracer.TraceVerbose("Setting locale to {0}", locale);
                this.m_context.Resources.Configuration.SetLocale(new Java.Util.Locale(locale));
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error setting locale to {0}: {1}", locale, e);
            }
            return this.m_context.Resources.Configuration.Locale.Language;
        }
    }
}

