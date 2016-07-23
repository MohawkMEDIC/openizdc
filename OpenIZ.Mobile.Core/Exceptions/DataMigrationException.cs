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
using OpenIZ.Mobile.Core.Configuration.Data;

namespace OpenIZ.Mobile.Core.Exceptions
{
	/// <summary>
	/// Data migration exception
	/// </summary>
	public class DataMigrationException : Exception
	{

		/// <summary>
		/// Gets or sets the migration id
		/// </summary>
		/// <value>The migration identifier.</value>
		public String MigrationId {
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Exceptions.DataMigrationException"/> class.
		/// </summary>
		public DataMigrationException (IDbMigration offender) : this(offender, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Exceptions.DataMigrationException"/> class.
		/// </summary>
		public DataMigrationException (IDbMigration offender, Exception inner) : base(String.Format("Migration of {0} failed", offender.Id), inner)
		{
			this.MigrationId = offender.Id;
		}

	}
}

