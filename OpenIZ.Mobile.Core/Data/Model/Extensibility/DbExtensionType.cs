﻿/*
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
 * User: fyfej
 * Date: 2016-10-25
 */
using System;
using SQLite.Net;
using SQLite.Net.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Extensibility
{
	/// <summary>
	/// Extension types
	/// </summary>
	[Table("extension_type")]
	public class DbExtensionType: DbIdentified
	{

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Column("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the extension handler.
		/// </summary>
		/// <value>The extension handler.</value>
		[Column("extensionHandler")]
		public String ExtensionHandler {
			get;
			set;
		}
	}
}

