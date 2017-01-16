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
 * User: fyfej
 * Date: 2016-10-25
 */
using System;
using SQLite.Net;
using SQLite.Net.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents an entity which is used to represent an application
	/// </summary>
	[Table("application")]
	public class DbApplicationEntity : DbIdentified
    {
		/// <summary>
		/// Gets or sets the security application.
		/// </summary>
		/// <value>The security application.</value>
		[Column("securityApplication"), MaxLength(16), NotNull]
		public byte[] SecurityApplicationUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the software.
		/// </summary>
		/// <value>The name of the software.</value>
		[Column("name"), NotNull]
		public String SoftwareName {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the version.
		/// </summary>
		/// <value>The name of the version.</value>
		[Column("version")]
		public String VersionName {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the vendor.
		/// </summary>
		/// <value>The name of the vendor.</value>
		[Column("vendor")]
		public String VendorName {
			get;
			set;
		}
	}
}

