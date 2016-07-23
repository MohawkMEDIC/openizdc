﻿/*
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

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents an entity name related to an entity
	/// </summary>
	[Table("entity_name")]
	public class DbEntityName : DbEntityLink
	{
		
		/// <summary>
		/// Gets or sets the use concept.
		/// </summary>
		/// <value>The use concept.</value>
		[Column("use"), MaxLength(16)]
		public byte[] UseConceptUuid {
			get;
			set;
		}
	}

	/// <summary>
	/// Represents a component of a name
	/// </summary>
	[Table("entity_name_comp")]
	public class DbEntityNameComponent : DbGenericNameComponent
	{

		/// <summary>
		/// Gets or sets the name identifier.
		/// </summary>
		/// <value>The name identifier.</value>
		[Column("name_uuid"), MaxLength(16), NotNull]
		public byte[] NameUuid {
			get;
			set;
		}
        
		/// <summary>
		/// Gets or sets the phonetic code.
		/// </summary>
		/// <value>The phonetic code.</value>
		[Column("phoneticCode"), Indexed]
		public String PhoneticCode {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the phonetic algorithm identifier.
		/// </summary>
		/// <value>The phonetic algorithm identifier.</value>
		[Column("phoneticAlgorithm")]
		public byte[] PhoneticAlgorithmUuid {
			get;
			set;
		}
	}

}

