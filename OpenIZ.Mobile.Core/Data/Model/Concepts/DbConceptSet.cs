﻿/*
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
using SQLite.Net;
using SQLite.Net.Attributes;
using OpenIZ.Core.Data.QueryBuilder.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Concepts
{
	/// <summary>
	/// Concept set
	/// </summary>
	[Table("concept_set")]
    [AssociativeTable(typeof(DbConcept), typeof(DbConceptSetConceptAssociation))]
	public class DbConceptSet : DbIdentified
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
		/// Gets or sets the mnemonic.
		/// </summary>
		/// <value>The mnemonic.</value>
		[Column("mnemonic"), Indexed, NotNull]
		public String Mnemonic {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the oid of the concept set
        /// </summary>
        [Column("oid")]
        public String Oid { get; set; }

        /// <summary>
        /// Gets or sets the url of the concept set
        /// </summary>
        [Column("url")]
        public String Url { get; set; }

    }

	/// <summary>
	/// Concept set concept association.
	/// </summary>
	[Table("concept_concept_set")]
	public class DbConceptSetConceptAssociation : DbIdentified
	{

		/// <summary>
		/// Gets or sets the concept identifier.
		/// </summary>
		/// <value>The concept identifier.</value>
		[Column("concept_uuid"), NotNull, Indexed(Name = "concept_concept_set_concept_set", Unique = true), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
		public byte[] ConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the concept set identifier.
		/// </summary>
		/// <value>The concept set identifier.</value>
		[Column("concept_set_uuid"), Indexed(Name = "concept_concept_set_concept_set", Unique = true), MaxLength(16), ForeignKey(typeof(DbConceptSet), nameof(DbConceptSet.Uuid))]
		public byte[] ConceptSetUuid {
			get;
			set;
		}
	}
}

