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

namespace OpenIZ.Mobile.Core.Data.Model.DataType
{
	/// <summary>
	/// Represents an identifier
	/// </summary>
	public abstract class DbIdentifier : DbIdentified
	{

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		[Column("value"), Indexed, NotNull]
		public String Value {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type identifier.
		/// </summary>
		/// <value>The type identifier.</value>
		[Column("type"), MaxLength(16)]
		public byte[] TypeUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the authority identifier.
		/// </summary>
		/// <value>The authority identifier.</value>
		[Column("authority"), NotNull, MaxLength(16)]
		public byte[] AuthorityUuid {
			get;
			set;
		}

	}

	/// <summary>
	/// Entity identifier storage.
	/// </summary>
	[Table("entity_identifier")]
	public class DbEntityIdentifier : DbIdentifier
	{
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("entity_uuid"), Indexed, MaxLength(16)]
        public byte[] EntityUuid
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Act identifier storage.
	/// </summary>
	[Table("act_identifier")]
	public class DbActIdentifier : DbIdentifier
	{
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("act_uuid"), Indexed, MaxLength(16)]
        public byte[] ActUuid
        {
            get;
            set;
        }
    }
}

