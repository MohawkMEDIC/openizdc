/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */
using System;

namespace OpenIZ.Mobile.Core.Configuration.Data
{
	/// <summary>
	/// Identifies a database migration script in code
	/// </summary>
	public interface IDbMigration
	{


		/// <summary>
		/// Gets the identifier of the migration
		/// </summary>
		String Id {
			get;
		}


		/// <summary>
		/// A human readable description of the migration
		/// </summary>
		/// <value>The description.</value>
		String Description { get; }

		/// <summary>
		/// Install the migration package
		/// </summary>
		bool Install();

	}

}

