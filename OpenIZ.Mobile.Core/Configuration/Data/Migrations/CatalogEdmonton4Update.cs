using OpenIZ.Core.Diagnostics;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Configuration.Data.Migrations
{
    /// <summary>
    /// Update for migrating Edmonton CTP5 to f4
    /// </summary>
    public class CatalogEdmonton4Update : IDbMigration
    {
        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description
        {
            get
            {
                return "Updates Edmonton CTP5 to CTP5 database";
            }
        }

        /// <summary>
        /// Gets the identifier of the migration
        /// </summary>
        public string Id
        {
            get
            {
                return "openiz-edmonton-ctp3-CTP5";
            }
        }

        /// <summary>
        /// Install the migration
        /// </summary>
        public bool Install()
        {
            var tracer = Tracer.GetTracer(this.GetType());

            // Database for the SQL Lite connection
            var db = SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current?.Configuration.GetConnectionString(ApplicationContext.Current?.Configuration.GetSection<DataConfigurationSection>().MainDataSourceConnectionStringName).Value);
            using (db.Lock())
            {
                try
                {
                    db.Execute("ALTER TABLE act_participation ADD sequence NUMERIC(10,0)");
                }
                catch { }
                try
                {
                    db.BeginTransaction();

                    // Now we query and set the sequencing
                    int seq = 0;
                    foreach (var itm in db.Table<DbActParticipation>())
                    {
                        itm.Sequence = seq++;
                        db.Update(itm);
                    }
                    db.Commit();
                    return true;
                }
                catch(Exception e)
                {
                    db.Rollback();
                    tracer.TraceError("Error installing CTP5 update: {0}", e);
                    return true;
                    //throw new InvalidOperationException("Could not install Edmonton CTP5 update");
                }
            }
        }
    }
}
