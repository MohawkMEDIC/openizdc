using System;
using Android.Webkit;
using Java.Interop;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Android.Security;
using System.Security.Permissions;
using OpenIZ.Mobile.Core.Diagnostics;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	/// <summary>
	/// Represents an administrative functions bridge which controls the application itself
	/// </summary>
	public class ConfigurationServiceBridge : Java.Lang.Object
	{

		private Tracer m_tracer = Tracer.GetTracer (typeof(ConfigurationServiceBridge));
		/// <summary>
		/// Backs up the database to the user's SD card
		/// </summary>
		[Export]
		[JavascriptInterface]
		public bool Save(String options)
		{
			// Demand the appropriate policy
			new PolicyPermission(PermissionState.Unrestricted, PolicyIdentifiers.AccessClientAdministrativeFunction).Demand();

			this.m_tracer.TraceInfo ("Saving configuration options {0}", options);
			AndroidApplicationContext.Current.ConfigurationManager.Save ();
			return true;
		}

	}
}

