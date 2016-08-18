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
 * Date: 2016-8-18
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
using OpenIZ.Mobile.Core.Android.Services.Attributes;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Mobile.Core.Android.Services.ServiceHandlers
{
	[RestService("/__user")]
	public class UserService
	{
		private Tracer m_tracer = Tracer.GetTracer(typeof(ApplicationService));

		[RestOperation(UriPath = "/profile", Method = "POST")]
		public UserEntity SaveUserProfile(UserEntity user)
		{
			return null;
		}

		[RestOperation(UriPath = "/profile", Method = "GET")]
		public UserEntity GetUserProfile()
		{
			return null;
		}

	}
}