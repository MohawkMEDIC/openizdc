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
using SQLite.Net;
using SQLite.Net.Attributes;
using OpenIZ.Mobile.Core.Data.Model.Concepts;
using OpenIZ.Core.Data.QueryBuilder.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents a material in the database
	/// </summary>
	[Table("material")]
	public class DbMaterial : DbEntitySubTable
    {

		/// <summary>
		/// Gets or sets the quantity of an entity within its container.
		/// </summary>
		/// <value>The quantity.</value>
		[Column("quantity")]
		public decimal Quantity {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the form concept.
		/// </summary>
		/// <value>The form concept.</value>
		[Column("form_concept_uuid"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
		public byte[] FormConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the quantity concept.
		/// </summary>
		/// <value>The quantity concept.</value>
		[Column("quantity_concept_uuid"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
		public byte[] QuantityConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the expiry date.
		/// </summary>
		/// <value>The expiry date.</value>
		[Column("expiry")]
		public DateTime ExpiryDate {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is administrative.
		/// </summary>
		/// <value><c>true</c> if this instance is administrative; otherwise, <c>false</c>.</value>
		[Column("isAdministrative")]
		public bool IsAdministrative {
			get;
			set;
		}

        /// <summary>
        /// Query result
        /// </summary>
        public class QueryResult : DbEntity
        {
            /// <summary>
            /// Gets or sets the quantity of an entity within its container.
            /// </summary>
            /// <value>The quantity.</value>
            [Column("quantity")]
            public decimal Quantity
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the form concept.
            /// </summary>
            /// <value>The form concept.</value>
            [Column("form_concept_uuid"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
            public byte[] FormConceptUuid
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the quantity concept.
            /// </summary>
            /// <value>The quantity concept.</value>
            [Column("quantity_concept_uuid"), MaxLength(16), ForeignKey(typeof(DbConcept), nameof(DbConcept.Uuid))]
            public byte[] QuantityConceptUuid
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the expiry date.
            /// </summary>
            /// <value>The expiry date.</value>
            [Column("expiry")]
            public DateTime ExpiryDate
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is administrative.
            /// </summary>
            /// <value><c>true</c> if this instance is administrative; otherwise, <c>false</c>.</value>
            [Column("isAdministrative")]
            public bool IsAdministrative
            {
                get;
                set;
            }
        }
	}

	/// <summary>
	/// Manufactured material.
	/// </summary>
	[Table("manufactured_material")]
	public class DbManufacturedMaterial : DbMaterialSubTable
	{

		/// <summary>
		/// Gets or sets the lot number.
		/// </summary>
		/// <value>The lot number.</value>
		[Column("lotNumber"), Collation("NOCASE")]
		public String LotNumber {
			get;
			set;
		}

        public class QueryResult : DbMaterial.QueryResult
        {
            /// <summary>
            /// Gets or sets the lot number.
            /// </summary>
            /// <value>The lot number.</value>
            [Column("lotNumber"), Collation("NOCASE")]
            public String LotNumber
            {
                get;
                set;
            }
        }
	}

}

