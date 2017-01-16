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

namespace OpenIZ.Mobile.Core.Data.Model
{
	/// <summary>
	/// Model extensions.
	/// </summary>
	public static class ModelExtensions
	{
		/// <summary>
		/// Represents a byte array as a guid.
		/// </summary>
		/// <returns>The GUID.</returns>
		/// <param name="me">Me.</param>
		public static Guid? ToGuid(this byte[] me)
		{
			return me == null ? (Guid?)null : new Guid (me);
		}
	}
}

