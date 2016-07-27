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
using SQLite.Net;
using SQLite.Net.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Concepts
{
	/// <summary>
	/// Represents concept relationships
	/// </summary>
	[Table("concept_relationship")]
	public class DbConceptRelationship : DbIdentified
	{

		/// <summary>
		/// Gets or sets the source concept.
		/// </summary>
		[Column("source_concept"), Indexed, NotNull, MaxLength(16)]
		public byte[] SourceConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the target concept identifier.
		/// </summary>
		/// <value>The target concept identifier.</value>
		[Column("targetConcept"), Indexed, NotNull, MaxLength(16)]
		public byte[] TargetConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the relationship type identifier.
		/// </summary>
		/// <value>The relationship type identifier.</value>
		[Column("relationshipType"), NotNull, MaxLength(16)]
		public byte[] RelationshipTypeUuid {
			get;
			set;
		}
	}
}

