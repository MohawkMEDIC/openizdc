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
 * User: justi
 * Date: 2017-2-4
 */
using System;
using SQLite.Net;
using SQLite.Net.Attributes;
using OpenIZ.Core.Data.QueryBuilder.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Security
{
	/// <summary>
	/// Represents a security device. This table should only have one row (the current device)
	/// </summary>
	[Table("security_device")]
    [AssociativeTable(typeof(DbSecurityPolicy), typeof(DbSecurityDevicePolicy))]
	public class DbSecurityDevice : DbBaseData
	{
		
		/// <summary>
		/// Gets or sets the public identifier.
		/// </summary>
		/// <value>The public identifier.</value>
		[Column("public_id"), Unique]
		public String PublicId {
			get;
			set;
		}


	}
}

