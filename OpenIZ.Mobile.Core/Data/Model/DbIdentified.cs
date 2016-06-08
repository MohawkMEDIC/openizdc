using System;
using SQLite;
using OpenIZ.Core.Model;
using System.Collections.Generic;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Configuration;

namespace OpenIZ.Mobile.Core.Data.Model
{
	/// <summary>
	/// Represents data that is identified in some way
	/// </summary>
	public abstract class DbIdentified
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Data.Model.Model.DbIdentified"/> class.
		/// </summary>
		public DbIdentified ()
		{
			this.Uuid = Guid.NewGuid ().ToByteArray ();
		}

		/// <summary>
		/// Gets or sets the universal identifier for the object
		/// </summary>
		[PrimaryKey, Column("uuid"), MaxLength(16), Indexed, NotNull]
		public byte[] Uuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the key (GUID) on the persistence item
		/// </summary>
		/// <value>The key.</value>
		[Ignore]
		public Guid Key
		{
			get { return this.Uuid.ToGuid() ?? Guid.Empty; }
			set { this.Uuid = value.ToByteArray (); }
		}

        /// <summary>
		/// Creates the connection.
		/// </summary>
		/// <returns>The connection.</returns>
		protected SQLiteConnection CreateConnection()
        {
            var config = ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>();
            return new SQLiteConnection(ApplicationContext.Current.Configuration.GetConnectionString(config.MainDataSourceConnectionStringName).Value);
        }

        /// <summary>
        /// Delay load a collection
        /// </summary>
        protected List<T> DelayLoadCollection<T>(List<T> current, Expression<Func<T, bool>> query) where T : new()
        {
            using (var conn = this.CreateConnection())
            {
                if (current != null)
                    return current;
                return new List<T>(conn.Table<T>().Where(query));
            }
        }

        /// <summary>
        /// Delay load a collection
        /// </summary>
        protected List<T> DelayLoadCollection<T>(List<T> current, String sqlQuery, params Object[] args) where T : new()
        {
            using (var conn = this.CreateConnection())
            {
                if (current != null)
                    return current;
                return conn.Query<T>(sqlQuery, args);
            }
        }

        /// <summary>
        /// Delay load a collection
        /// </summary>
        protected T DelayLoad<T>(T current, Expression<Func<T, bool>> query) where T : new()
        {
            using (var conn = this.CreateConnection())
            {
                if (current != null)
                    return current;
                return conn.Table<T>().Where(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Delay load a collection
        /// </summary>
        protected T DelayLoad<T>(T current, String sqlQuery, params Object[] args) where T : new()
        {
            using (var conn = this.CreateConnection())
            {
                if (current != null)
                    return current;
                return conn.Query<T>(sqlQuery, args)[0];
            }
        }


    }
}

