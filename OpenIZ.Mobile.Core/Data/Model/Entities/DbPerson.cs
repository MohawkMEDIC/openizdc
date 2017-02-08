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
using OpenIZ.Core.Model.DataTypes;
using SQLite.Net.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents a person
	/// </summary>
	[Table("person")]
	public class DbPerson : DbEntitySubTable
	{

		/// <summary>
		/// Gets or sets the date of birth.
		/// </summary>
		/// <value>The date of birth.</value>
		[Column("dateOfBirth")]
		public DateTime? DateOfBirth {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the date of birth precision.
		/// </summary>
		/// <value>The date of birth precision.</value>
		[Column("dateOfBirthPrecision"), MaxLength(1)]
		public string DateOfBirthPrecision {
			get;
			set;
		}

        /// <summary>
        /// Quey result
        /// </summary>
        public class QueryResult : DbEntity
        {
            /// <summary>
            /// Gets or sets the date of birth.
            /// </summary>
            /// <value>The date of birth.</value>
            [Column("dateOfBirth")]
            public DateTime? DateOfBirth
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the date of birth precision.
            /// </summary>
            /// <value>The date of birth precision.</value>
            [Column("dateOfBirthPrecision"), MaxLength(1)]
            public string DateOfBirthPrecision
            {
                get;
                set;
            }
        }

	}
}

