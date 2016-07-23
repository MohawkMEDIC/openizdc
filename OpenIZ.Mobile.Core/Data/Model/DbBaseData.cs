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
 * User: justi
 * Date: 2016-6-14
 */
using System;
using SQLite;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model.Security;

namespace OpenIZ.Mobile.Core.Data.Model
{
	/// <summary>
	/// Represents data which has authorship information attached
	/// </summary>
	public abstract class DbBaseData : DbIdentified
	{
		/// <summary>
		/// Gets or sets the creation time
		/// </summary>
		/// <value>The creation time.</value>
		[Column("creationTime"), NotNull]
		public DateTime? CreationTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the time that the record was obsoleted
		/// </summary>
		/// <value>The obsoletion time.</value>
		[Column("obsoletionTime")]
		public DateTime? ObsoletionTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the updated time.
		/// </summary>
		/// <value>The updated time.</value>
		[Column("updatedTime")]
		public DateTime? UpdatedTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the user that created the data
		/// </summary>
		/// <value>The created by identifier.</value>
		[Column("createdBy"), NotNull, MaxLength(16)]
		public byte[] CreatedByUuid {
			get;
			set;
		}

		/// <summary>
		/// Obsoletion user
		/// </summary>
		/// <value>The obsoleted by identifier.</value>
		[Column("obsoletedBy"), MaxLength(16)]
		public byte[] ObsoletedByUuid { 
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the updated by identifier.
		/// </summary>
		/// <value>The updated by identifier.</value>
		[Column("updatedBy"), MaxLength(16)]
		public byte[] UpdatedByUuid {
			get;
			set;
		}	

		/// <summary>
		/// Sets the created by key.
		/// </summary>
		/// <value>The created by key.</value>
		[Ignore]
		internal Guid CreatedByKey {
			get { return this.CreatedByUuid.ToGuid() ?? Guid.Empty; }
			set { this.CreatedByUuid = value.ToByteArray (); }
		}
			
		/// <summary>
		/// Gets or sets the updated by key.
		/// </summary>
		/// <value>The updated by key.</value>
		[Ignore]
		internal Guid? UpdatedByKey {
			get { return this.UpdatedByUuid?.ToGuid(); }
			set { this.UpdatedByUuid = value?.ToByteArray (); }
		}

		/// <summary>
		/// Gets or sets the obsoleted by key.
		/// </summary>
		/// <value>The obsoleted by key.</value>
		[Ignore]
		internal Guid? ObsoletedByKey {
			get { return this.CreatedByUuid?.ToGuid(); }
			set { this.ObsoletedByUuid = value?.ToByteArray (); }
		}
	}
}

