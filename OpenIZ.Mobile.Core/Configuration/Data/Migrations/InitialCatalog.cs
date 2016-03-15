using System;
using System.Linq;
using SQLite;
using OpenIZ.Mobile.Core.Data.Model.Concepts;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Data.Model.DataType;
using OpenIZ.Mobile.Core.Data.Model.Extensibility;
using OpenIZ.Mobile.Core.Data.Model.Security;

namespace OpenIZ.Mobile.Core.Configuration.Data.Migrations
{
	/// <summary>
	/// This class is responsible for setting up an initial catalog of items in the SQL Lite database
	/// </summary>
	public class InitialCatalog : IDbMigration
	{
		
		#region IDbMigration implementation

		/// <summary>
		/// Install the initial catalog
		/// </summary>
		public bool Install ()
		{

			var tracer = Tracer.GetTracer (this.GetType ());

			// Database for the SQL Lite connection
			using (var db = new SQLiteConnection (ApplicationContext.Current?.Configuration.GetConnectionString(ApplicationContext.Current?.Configuration.GetSection<DataConfigurationSection>().MainDataSourceConnectionStringName).Value)) {


				db.TableChanged += (s, e) => tracer.TraceInfo ("Updating {0}", e.Table.TableName);
				// Create tables
				tracer.TraceInfo("Installing Concept Tables...");
				db.CreateTable<DbConcept> (CreateFlags.None);
				db.CreateTable<DbConceptName> (CreateFlags.None);
				db.CreateTable<DbConceptClass> (CreateFlags.None);
				db.CreateTable<DbConceptRelationship> (CreateFlags.None);
				db.CreateTable<DbConceptRelationshipType> (CreateFlags.None);
				db.CreateTable<DbConceptSet> (CreateFlags.None);
				db.CreateTable<DbConceptSetConceptAssociation> (CreateFlags.None);

				tracer.TraceInfo ("Installing Identiifers Tables...");
				db.CreateTable<DbEntityIdentifier> (CreateFlags.None);
				db.CreateTable<DbActIdentifier> (CreateFlags.None);
				db.CreateTable<DbIdentifierType> (CreateFlags.None);

				tracer.TraceInfo ("Installing Extensability Tables...");
				db.CreateTable<DbActExtension> (CreateFlags.None);
				db.CreateTable<DbActNote> (CreateFlags.None);
				db.CreateTable<DbEntityExtension> (CreateFlags.None);
				db.CreateTable<DbEntityNote> (CreateFlags.None);
				db.CreateTable<DbExtensionType> (CreateFlags.None);

				tracer.TraceInfo ("Installing Security Tables...");
				db.CreateTable<DbSecurityApplication> (CreateFlags.None);
				db.CreateTable<DbSecurityDevice> (CreateFlags.None);
				db.CreateTable<DbSecurityPolicy> (CreateFlags.None);
				db.CreateTable<DbSecurityDevicePolicy> (CreateFlags.None);
				db.CreateTable<DbSecurityRolePolicy> (CreateFlags.None);
				db.CreateTable<DbActSecurityPolicy> (CreateFlags.None);
				db.CreateTable<DbEntitySecurityPolicy> (CreateFlags.None);
				db.CreateTable<DbSecurityRole> (CreateFlags.None);
				db.CreateTable<DbSecurityUser> (CreateFlags.None);
				db.CreateTable<DbSecurityUserRole> (CreateFlags.None);

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

