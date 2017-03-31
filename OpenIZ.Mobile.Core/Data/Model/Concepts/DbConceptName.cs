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
using OpenIZ.Core.Data.QueryBuilder.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Concepts
{
	/// <summary>
	/// Represents a concept name
	/// </summary>
	[Table("concept_name")]
	public class DbConceptName : DbIdentified
	{
	
		/// <summary>
		/// Gets or sets the concept identifier.
		/// </summary>
		/// <value>The concept identifier.</value>
		[Column("concept_uuid"), Indexed, NotNull, MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
		public byte[] ConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the language.
		/// </summary>
		/// <value>The language.</value>
		[Column("language")]
		public String Language {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Column("value"), Indexed, NotNull]
		public String Name {
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
		[Column("phoneticAlgorithm"), MaxLength(16)]
		public byte[] PhoneticAlgorithmUuid {
			get;
			set;
		}
	}
}

