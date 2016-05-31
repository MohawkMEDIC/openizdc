using System;
using Java.Interop;
using Android.Webkit;
using OpenIZ.Mobile.Core.Services;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	/// <summary>
	/// Represents the concept service bridge
	/// </summary>
	public class ConceptServiceBridge : Java.Lang.Object
	{


		/// <summary>
		/// Gets the concept set.
		/// </summary>
		/// <returns>The concept set.</returns>
		/// <param name="setMnemonic">Set mnemonic.</param>
		[Export]
		[JavascriptInterface]
		public String GetConceptSet(String setMnemonic)
		{
			var conceptService = ApplicationContext.Current.GetService<IConceptService> ();
			var retVal = conceptService.GetConceptSet (setMnemonic);
			return JniUtil.ToJson (retVal);
		}
	}
}

