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
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model
{
    /// <summary>
    /// Keeps track of migrations
    /// </summary>
    [Table("migrations")]
    public class DbMigrationLog
    {

        /// <summary>
        /// Migration log
        /// </summary>
        [PrimaryKey, Column("id")]
        public String MigrationId { get; set; }

        /// <summary>
        /// Installation date
        /// </summary>
        [Column("date")]
        public DateTime InstallationDate { get; set; }
    }
}
