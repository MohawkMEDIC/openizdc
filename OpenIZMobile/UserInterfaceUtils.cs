using System;
using Android.App;
using Android.Content;
using Android.Content.Res;

namespace OpenIZMobile
{
	/// <summary>
	/// User interface utilitiy functions
	/// </summary>
	public static class UserInterfaceUtils
	{

		/// <summary>
		/// Shows an exception as a dialog box
		/// </summary>
		public static void ShowMessage(Context context, EventHandler<DialogClickEventArgs> confirmAction, String message, params String[] args)
		{
			Application.SynchronizationContext.Post (_ => {
				var alertDialogBuilder = new AlertDialog.Builder (context) 
					.SetMessage (String.Format (message, args))
					.SetCancelable (false) 
					.SetPositiveButton (Resource.String.confirm, confirmAction); 

				alertDialogBuilder.Show ();
			}, null);
		}


	}
}

