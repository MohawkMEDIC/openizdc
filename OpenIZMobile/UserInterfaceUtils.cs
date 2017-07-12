/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-10-11
 */
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

