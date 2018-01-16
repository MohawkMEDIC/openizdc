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
using OpenIZ.Core.Diagnostics;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Security.Audit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Configuration.Data.Migrations
{
    /// <summary>
    /// Database migration for audit
    /// </summary>
    public class InitialAuditCatalog : IDbMigration
    {
        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description
        {
            get
            {
                return "Audit Catalog";
            }
        }

        /// <summary>
        /// Get the identifier of the migration
        /// </summary>
        public string Id
        {
            get
            {
                return "000-init-openiz-dalhouse-audit";
            }
        }

        /// <summary>
        /// Install the migration
        /// </summary>
        public bool Install()
        {
            var tracer = Tracer.GetTracer(this.GetType());
            // Database for the SQL Lite connection
            var db = SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current?.Configuration.GetConnectionString("openIzAudit").Value);
            using (db.Lock())
            {
                try
                {

                    db.CreateTable<DbAuditData>();
                    db.CreateTable<DbAuditActor>();
                    db.CreateTable<DbAuditActorAssociation>();
                    db.CreateTable<DbAuditCode>();
                    db.CreateTable<DbAuditObject>();
                    return true;
                }
                catch(Exception e)
                {
                    tracer.TraceError("Error deploying Audit repository: {0}", e);
                    return false;
                }
            }
        }
    }
}