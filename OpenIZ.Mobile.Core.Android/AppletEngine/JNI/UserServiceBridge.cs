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
 * User: khannan
 * Date: 2016-8-15
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
using OpenIZ.Core.Diagnostics;
using Java.Interop;
using Android.Webkit;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Android.Resources;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	public class UserServiceBridge : Java.Lang.Object
	{
		// Tracer
		private Tracer m_tracer = Tracer.GetTracer(typeof(UserServiceBridge));

		private IDataPersistenceService<UserEntity> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();

		/// <summary>
		/// Changes the password of a user.
		/// </summary>
		/// <param name="username">The username of the user.</param>
		/// <param name="existing">The existing password of the user.</param>
		/// <param name="password">The new password of the user.</param>
		/// <param name="confirmation">The new password confirmation of the user.</param>
		/// <returns>Returns the updated user.</returns>
		[Export]
		[JavascriptInterface]
		public string ChangePassword(string username, string existing, string password, string confirmation)
		{
			string retVal = null;

			var networkInformationService = ApplicationContext.Current.GetService<INetworkInformationService>();

			if (networkInformationService.IsNetworkAvailable)
			{
				ISecurityRepositoryService securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

				if (securityRepositoryService == null)
				{
					throw new InvalidOperationException(string.Format("{0} not found", nameof(ISecurityRepositoryService)));
				}

				var user = securityRepositoryService.FindUsers(u => u.UserName == username).FirstOrDefault();

				if (user == null)
				{
					throw new ArgumentException(Strings.err_invalid_argumentType, nameof(user));
				}

				if (password != confirmation)
				{
					throw new ArgumentException(Strings.err_invalid_argumentType, nameof(password));
				}

				securityRepositoryService.ChangePassword(user.Key.Value, password);

				retVal = JniUtil.ToJson<SecurityUser>(user);
			}

			return retVal;
		}

		[Export]
		[JavascriptInterface]
		public string Save(string imsiUserEntity)
		{
			string retVal = null;

			try
			{
				var user = JsonViewModelSerializer.DeSerialize<UserEntity>(imsiUserEntity);


				if (user == null)
				{
					throw new ArgumentException(Strings.err_invalid_argumentType, nameof(imsiUserEntity));
				}

				user = this.persistenceService.Update(user);

				retVal = JsonViewModelSerializer.Serialize(user);
			}
			catch (Exception e)
			{
				this.m_tracer.TraceError("Error updating user {0}: {1}", imsiUserEntity, e);
				retVal = "err_general";
			}

			return retVal;
		}
	}
}