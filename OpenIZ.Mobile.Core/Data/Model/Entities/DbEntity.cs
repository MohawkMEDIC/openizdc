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
	/// Represents an entity in the database
	/// </summary>
	[Table("entity")]
	public class DbEntity : DbVersionedData
	{
        /// <summary>
        /// Gets or sets the template
        /// </summary>
        [Column("template"), MaxLength(16)]
        public byte[] TemplateUuid { get; set; }

        /// <summary>
        /// Gets or sets the class concept identifier.
        /// </summary>
        /// <value>The class concept identifier.</value>
        [Column("classConcept"), MaxLength(16), NotNull, Indexed]
		public byte[] ClassConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the determiner concept identifier.
		/// </summary>
		/// <value>The determiner concept identifier.</value>
		[Column("determinerConcept"), MaxLength(16)]
		public byte[] DeterminerConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the status concept identifier.
		/// </summary>
		/// <value>The status concept identifier.</value>
		[Column("statusConcept"), MaxLength(16), Indexed]
		public byte[] StatusConceptUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type concept identifier.
		/// </summary>
		/// <value>The type concept identifier.</value>
		[Column("typeConcept"), MaxLength(16)]
		public byte[] TypeConceptUuid {
			get;
			set;
		}


	}
}

