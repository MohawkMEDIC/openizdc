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
using System.Reflection;
using System.Linq;
using SQLite.Net;
using OpenIZ.Mobile.Core.Data.Model.Concepts;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Data.Model.DataType;
using OpenIZ.Mobile.Core.Data.Model.Extensibility;
using OpenIZ.Mobile.Core.Data.Model.Security;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using System.IO;
using OpenIZ.Mobile.Core.Data.Model.Roles;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Serices;
using OpenIZ.Mobile.Core.Resources;
using OpenIZ.Mobile.Core.Data.Connection;

namespace OpenIZ.Mobile.Core.Configuration.Data.Migrations
{
	/// <summary>
	/// This class is responsible for setting up an initial catalog of items in the SQL Lite database
	/// </summary>
	internal class InitialCatalog : IDbMigration
	{
		
		#region IDbMigration implementation

		/// <summary>
		/// Install the initial catalog
		/// </summary>
		public bool Install ()
		{

			var tracer = Tracer.GetTracer (this.GetType ());

            // Database for the SQL Lite connection
            var db = SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current?.Configuration.GetConnectionString(ApplicationContext.Current?.Configuration.GetSection<DataConfigurationSection>().MainDataSourceConnectionStringName).Value);
            using (db.Lock()) { 
                try
                {
                    db.BeginTransaction();

                    // Migration log create and check
                    db.CreateTable<DbMigrationLog>();
                    if (db.Table<DbMigrationLog>().Count(o => o.MigrationId == this.Id) > 0)
                    {
                        tracer.TraceInfo("Migration already installed");
                        return true;
                    }
                    else
                        db.Insert(new DbMigrationLog()
                        {
                            MigrationId = this.Id,
                            InstallationDate = DateTime.Now
                        });

                    ApplicationContext.Current.SetProgress(Strings.locale_setting_table, 0);
                    // Create tables
                    tracer.TraceInfo("Installing Concept Tables...");
                    db.CreateTable<DbConcept>();
                    db.CreateTable<DbConceptName>();
                    db.CreateTable<DbConceptClass>();
                    db.CreateTable<DbConceptRelationship>();
                    db.CreateTable<DbConceptRelationshipType>();
                    db.CreateTable<DbConceptSet>();
                    db.CreateTable<DbConceptSetConceptAssociation>();

                    tracer.TraceInfo("Installing Identifiers Tables...");
                    db.CreateTable<DbEntityIdentifier>();
                    db.CreateTable<DbActIdentifier>();
                    db.CreateTable<DbIdentifierType>();
                    db.CreateTable<DbAssigningAuthority>();

                    tracer.TraceInfo("Installing Extensibility Tables...");
                    db.CreateTable<DbActExtension>();
                    db.CreateTable<DbActNote>();
                    db.CreateTable<DbEntityExtension>();
                    db.CreateTable<DbEntityNote>();
                    db.CreateTable<DbExtensionType>();
                    db.CreateTable<DbTemplateDefinition>();

                    tracer.TraceInfo("Installing Security Tables...");
                    db.CreateTable<DbSecurityApplication>();
                    db.CreateTable<DbSecurityDevice>();
                    db.CreateTable<DbSecurityPolicy>();
                    db.CreateTable<DbSecurityDevicePolicy>();
                    db.CreateTable<DbSecurityRolePolicy>();
                    db.CreateTable<DbActSecurityPolicy>();
                    db.CreateTable<DbEntitySecurityPolicy>();
                    db.CreateTable<DbSecurityRole>();
                    db.CreateTable<DbSecurityUser>();
                    db.CreateTable<DbSecurityUserRole>();
                    db.CreateTable<DbSecurityApplicationPolicy>();
                    db.CreateTable<DbSecurityDevicePolicy>();
                    // Anonymous user
                    db.Insert(new DbSecurityUser()
                    {
                        Key = Guid.Parse("C96859F0-043C-4480-8DAB-F69D6E86696C"),
                        PasswordHash = "XXX",
                        SecurityHash = "XXX",
                        Lockout = DateTime.MaxValue,
                        UserName = "ANONYMOUS",
                        CreationTime = DateTime.Now,
                        CreatedByKey = Guid.Empty
                    });
                    // System user
                    db.Insert(new DbSecurityUser()
                    {
                        Key = Guid.Parse("fadca076-3690-4a6e-af9e-f1cd68e8c7e8"),
                        PasswordHash = "XXXX",
                        SecurityHash = "XXXX",
                        Lockout = DateTime.MaxValue,
                        UserName = "SYSTEM",
                        CreationTime = DateTime.Now,
                        CreatedByKey = Guid.Empty
                    });

                    // Need to create a local admin?
                    if (ApplicationContext.Current.GetService<LocalIdentityService>() != null)
                    {
                        Guid uid = Guid.NewGuid();
                        // System user
                        db.Insert(new DbSecurityUser()
                        {
                            Key = uid,
                            PasswordHash = ApplicationContext.Current.GetService<IPasswordHashingService>().ComputeHash("OpenIZ123"),
                            SecurityHash = Guid.NewGuid().ToString(),
                            Lockout = DateTime.MaxValue,
                            UserName = "LocalAdministrator",
                            CreationTime = DateTime.Now,
                            CreatedByKey = Guid.Empty
                        });
                        db.Insert(new DbSecurityUserRole()
                        {
                            Key = Guid.NewGuid(),
                            RoleUuid = new byte[] { 0x8D, 0xF6, 0xD7, 0xE8, 0xB5, 0x5B, 0xE3, 0x41, 0xB7, 0xFB, 0x2E, 0xC3, 0x24, 0x18, 0xB2, 0xE1 },
                            UserUuid = uid.ToByteArray()
                        });
                    }

                    tracer.TraceInfo("Installing Entity Tables...");
                    db.CreateTable<DbEntity>();
                    db.CreateTable<DbApplicationEntity>();
                    db.CreateTable<DbDeviceEntity>();
                    db.CreateTable<DbEntityAddress>();
                    db.CreateTable<DbEntityAddressComponent>();
                    db.CreateTable<DbEntityName>();
                    db.CreateTable<DbEntityNameComponent>();
                    db.CreateTable<DbEntityRelationship>();
                    db.CreateTable<DbMaterial>();
                    db.CreateTable<DbManufacturedMaterial>();
                    db.CreateTable<DbOrganization>();
                    db.CreateTable<DbPerson>();
                    db.CreateTable<DbPlace>();
                    db.CreateTable<DbTelecomAddress>();
                    db.CreateTable<DbEntityTag>();
                    db.CreateTable<DbUserEntity>();
                    db.CreateTable<DbPlaceService>();
                    db.CreateTable<DbPersonLanguageCommunication>();
                    db.CreateTable<DbAuthorityScope>();
                    tracer.TraceInfo("Installing Role Tables...");
                    db.CreateTable<DbPatient>();
                    db.CreateTable<DbProvider>();

                    tracer.TraceInfo("Installing Act Tables...");
                    db.CreateTable<DbAct>();
                    db.CreateTable<DbObservation>();
                    db.CreateTable<DbCodedObservation>();
                    db.CreateTable<DbQuantityObservation>();
                    db.CreateTable<DbTextObservation>();
                    db.CreateTable<DbSubstanceAdministration>();
                    db.CreateTable<DbPatientEncounter>();
                    db.CreateTable<DbControlAct>();
                    db.CreateTable<DbActRelationship>();
                    db.CreateTable<DbActTag>();
                    db.CreateTable<DbActParticipation>();

                    tracer.TraceInfo("Initializing Data & Views...");

                    // Run SQL Script
                    string[] resourceSql = {
                    "OpenIZ.Mobile.Core.Data.Sql.000_init_openiz_algonquin.sql",
                    "OpenIZ.Mobile.Core.Data.Sql.001_init_openiz_core_data.sql"
                };


                    foreach (var sql in resourceSql)
                    {
                        tracer.TraceInfo("Deploying {0}", sql);
                        ApplicationContext.Current.SetProgress(sql, 0);

                        using (StreamReader sr = new StreamReader(typeof(InitialCatalog).GetTypeInfo().Assembly.GetManifestResourceStream(sql)))
                        {
                            var stmts = sr.ReadToEnd().Split(';').Select(o => o.Trim()).ToArray();
                            for(int i = 0; i < stmts.Length; i++)
                            {
                                var stmt = stmts[i];
                                if(i % 10 == 0)
                                    ApplicationContext.Current.SetProgress(Strings.locale_setting_deploy, (float)i / stmts.Length);
                                if (String.IsNullOrEmpty(stmt)) continue;
                                tracer.TraceVerbose("EXECUTE: {0}", stmt);
                                db.Execute(stmt);
                            }
                        }
                    }
                    db.Commit();
                }
                catch(Exception e)
                {
                    db.Rollback();
                    tracer.TraceError("Error deploying InitialCatalog: {0}", e);
                    throw;
                }
			}
			return true;
		}


		/// <summary>
		/// Configuration identifier
		/// </summary>
		/// <value>The identifier.</value>
		public string Id {
			get {
				return "000-init-openiz-algonquin";
			}
		}

		/// <summary>
		/// A human readable description of the migration
		/// </summary>
		/// <value>The description.</value>
		public string Description {
			get {
				return "OpenIZ Mobile Algonquin (0.1.0.0) data model";
			}
		}


		#endregion
	}
}

