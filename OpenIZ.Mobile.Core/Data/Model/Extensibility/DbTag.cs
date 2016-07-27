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

namespace OpenIZ.Mobile.Core.Data.Model.Extensibility
{
	/// <summary>
	/// Represents a simpe tag (version independent)
	/// </summary>
	public abstract class DbTag : DbIdentified
	{


		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		[Column("key"), Indexed, NotNull]
		public String TagKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[Column("value"), NotNull]
		public String Value {
			get;
			set;
		}
	}

	/// <summary>
	/// Represents a tag associated with an enttiy
	/// </summary>
	[Table("entity_tag")]
	public class DbEntityTag : DbTag
	{
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [Column("entity_uuid"), NotNull, Indexed, MaxLength(16)]
        public byte[] EntityUuid
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Represents a tag associated with an act
	/// </summary>
	[Table("act_tag")]
	public class DbActTag : DbTag
	{
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [Column("act_uuid"), NotNull, Indexed, MaxLength(16)]
        public byte[] ActUuid
        {
            get;
            set;
        }
    }

}

