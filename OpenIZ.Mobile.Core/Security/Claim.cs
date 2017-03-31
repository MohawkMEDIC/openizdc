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
 * User: justi
 * Date: 2016-6-14
 */
using System;
using System.Security.Principal;
using System.Linq;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Security
{

	/// <summary>
	/// Represents a single claim made about a user
	/// </summary>
	public class Claim
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Security.Claim"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		public Claim (String type, String value)
		{
			this.Type = type;
			this.Value = value;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public String Type {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public String Value {
			get;
			set;
		}

        /// <summary>
        /// Claim value as date time
        /// </summary>
        public DateTime AsDateTime()
        {
            DateTime value = DateTime.MinValue;
            if(!DateTime.TryParse(this.Value, out value))
            {
                int offset = 0;
                if (Int32.TryParse(this.Value, out offset))
                    value = new DateTime(1970, 1, 1).AddSeconds(offset).ToLocalTime();
                else
                    throw new ArgumentOutOfRangeException(nameof(Value));
            }
            return value;
        }
	}


}

