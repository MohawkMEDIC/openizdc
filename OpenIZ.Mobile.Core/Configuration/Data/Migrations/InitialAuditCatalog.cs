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