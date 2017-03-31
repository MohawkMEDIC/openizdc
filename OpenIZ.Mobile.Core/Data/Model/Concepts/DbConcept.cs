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
using System.Linq;
using SQLite.Net;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using OpenIZ.Core.Data.QueryBuilder.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Concepts
{
	/// <summary>
	/// Physical data layer implemntation of concept
	/// </summary>
	[Table("concept")]
    [AssociativeTable(typeof(DbConceptSet), typeof(DbConceptSetConceptAssociation))]
	public class DbConcept : DbVersionedData
	{

		/// <summary>
		/// Gets or sets whether the object is a system concept or not
		/// </summary>
		[Column("isReadonly")]
		public bool IsSystemConcept {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the object mnemonic
		/// </summary>
		/// <value>The mnemonic.</value>
		[Column("mnemonic"), Unique, NotNull]
		public string Mnemonic {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the status concept id
		/// </summary>
		[Column("statusConcept"), Indexed, NotNull, MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
		public byte[] StatusUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the concept classification
		/// </summary>
		[Column("class"), NotNull, MaxLength(16), ForeignKey(typeof(DbConceptClass), nameof(DbConceptClass.Uuid))]
		public byte[] ClassUuid {
			get;
			set;
		}

    }
}

