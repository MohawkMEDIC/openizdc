using OpenIZ.Core.Diagnostics;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Search;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Configuration.Data.Migrations
{
    /// <summary>
    /// Data migration that inserts the initial search catalog
    /// </summary>
    public class InitialSearchCatalog : IDbMigration
    {
        // Initial search catalog
        private Tracer m_tracer = Tracer.GetTracer(typeof(InitialSearchCatalog));

        /// <summary>
        /// Gets the description of the migration
        /// </summary>
        public string Description
        {
            get
            {
                return "FreeText Search Catalog";
            }
        }

        /// <summary>
        /// Identifier of the migration
        /// </summary>
        public string Id
        {
            get
            {
                return "000-init-search";
            }
        }

        /// <summary>
        /// Perform the installation
        /// </summary>
        public bool Install()
        {
            try
            {

                // Is the search service registered?
                if (ApplicationContext.Current.GetService<IFreetextSearchService>() == null)
                {
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(SearchIndexService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Services.Add(new SearchIndexService());
                }

                // Get a connection to the search database
                var conn = SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current.Configuration.GetConnectionString("openIzSearch").Value);
                using (conn.Lock())
                {
                    conn.CreateTable<Search.Model.SearchEntityType>();
                    conn.CreateTable<Search.Model.SearchTerm>();
                    conn.CreateTable<Search.Model.SearchTermEntity>();
                } // release lock

                // Perform an initial indexing
                return ApplicationContext.Current.GetService<IFreetextSearchService>().Index();
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error installing search tables {0}", e);
                throw;
            }
        }
    }
}
