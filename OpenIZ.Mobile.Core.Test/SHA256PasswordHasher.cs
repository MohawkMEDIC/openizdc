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
using OpenIZ.Mobile.Core.Serices;
using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenIZ.Mobile.Core.Test
{
    /// <summary>
    /// SHA256 password hasher service
    /// </summary>
    public class SHA256PasswordHasher : IPasswordHashingService
    {
        #region IPasswordHashingService implementation
        /// <summary>
        /// Compute hash
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="password">Password.</param>
        public string ComputeHash(string password)
        {
            SHA256 hasher = SHA256.Create();
            return BitConverter.ToString(hasher.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();
        }
        #endregion
    }
}