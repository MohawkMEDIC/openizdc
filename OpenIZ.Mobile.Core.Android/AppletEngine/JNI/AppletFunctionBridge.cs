using System;
using Android.Webkit;
using Java.Interop;
using Android.Widget;
using Android.Content;
using Org.Json;
using Android.App;

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
		/// Go back home
		/// </summary>
		[Export]
		[JavascriptInterface]
		public void Back()
		{
			if (this.m_view.CanGoBack ())
				this.m_view.GoBack ();
			else
				(this.m_context as Activity).Finish ();
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
	}
}

