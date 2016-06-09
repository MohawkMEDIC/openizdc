using System;
using System.Linq;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Map;
using System.Reflection;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Synchronization.Model;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model;
using System.Collections;
using System.Collections.Generic;
using SQLite;
using OpenIZ.Mobile.Core.Data.Model.Security;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Exceptions;

namespace OpenIZ.Mobile.Core.Data
{
	/// <summary>
	/// Represents a data persistence service which stores data in the local SQLite data store
	/// </summary>
	public abstract class LocalPersistenceServiceBase<TData> : IDataPersistenceService<TData> where TData : IdentifiedData
	{

		// Get tracer
		protected Tracer m_tracer = Tracer.GetTracer (typeof(LocalPersistenceServiceBase<TData>));

		// Configuration
		protected static DataConfigurationSection m_configuration = ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>();

		// Mapper
		protected static ModelMapper m_mapper = new ModelMapper(typeof(LocalPersistenceServiceBase<TData>).GetTypeInfo().Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Data.Map.ModelMap.xml"));

		#region IDataPersistenceService implementation
		/// <summary>
		/// Occurs when inserted.
		/// </summary>
		public event EventHandler<DataPersistenceEventArgs<TData>> Inserted;
		/// <summary>
		/// Occurs when inserting.
		/// </summary>
		public event EventHandler<DataPersistencePreEventArgs<TData>> Inserting;
		/// <summary>
		/// Occurs when updated.
		/// </summary>
		public event EventHandler<DataPersistenceEventArgs<TData>> Updated;
		/// <summary>
		/// Occurs when updating.
		/// </summary>
		public event EventHandler<DataPersistencePreEventArgs<TData>> Updating;
		/// <summary>
		/// Occurs when obsoleted.
		/// </summary>
		public event EventHandler<DataPersistenceEventArgs<TData>> Obsoleted;
		/// <summary>
		/// Occurs when obsoleting.
		/// </summary>
		public event EventHandler<DataPersistencePreEventArgs<TData>> Obsoleting;
		/// <summary>
		/// Occurs when queried.
		/// </summary>
		public event EventHandler<EventArgs> Queried;
		/// <summary>
		/// Occurs when querying.
		/// </summary>
		public event EventHandler<EventArgs> Querying;

		/// <summary>
		/// Creates the connection.
		/// </summary>
		/// <returns>The connection.</returns>
		private SQLiteConnection CreateConnection()
		{
			return new SQLiteConnection (ApplicationContext.Current.Configuration.GetConnectionString (m_configuration.MainDataSourceConnectionStringName).Value);
		}

		/// <summary>
		/// Insert the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		public TData Insert (TData data)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			DataPersistencePreEventArgs<TData> preArgs = new DataPersistencePreEventArgs<TData> (data);
			this.Inserting?.Invoke (this, preArgs);
			if (preArgs.Cancel) {
				this.m_tracer.TraceWarning ("Pre-Event handler indicates abort insert for {0}", data);
				return data;
			}

			// Persist object
			using (var connection = this.CreateConnection ()) 
				try
				{
					this.m_tracer.TraceVerbose("INSERT {0}", data);

					connection.BeginTransaction ();

					data = this.Insert(connection, data);

					connection.Commit();

                    this.Inserted?.Invoke(this, new DataPersistenceEventArgs<TData>(data));

                    return data;

                }
                catch (Exception e) {
					this.m_tracer.TraceError("Error : {0}", e);
					connection.Rollback ();
					throw new LocalPersistenceException(DataOperationType.Insert, data, e);
					
				}



		}
		/// <summary>
		/// Update the specified data
		/// </summary>
		/// <param name="data">Data.</param>
		public TData Update (TData data)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));
			else if (data.Key == Guid.Empty)
				throw new InvalidOperationException ("Data missing key");

			DataPersistencePreEventArgs<TData> preArgs = new DataPersistencePreEventArgs<TData> (data);
			this.Updating?.Invoke (this, preArgs);
			if (preArgs.Cancel) {
				this.m_tracer.TraceWarning ("Pre-Event handler indicates abort update for {0}", data);
				return data;
			}

			// Persist object
			using (var connection = this.CreateConnection ()) 
				try
				{
					this.m_tracer.TraceVerbose("UPDATE {0}", data);
					connection.BeginTransaction ();

					data = this.Update(connection, data);

					connection.Commit();

                    this.Updated?.Invoke(this, new DataPersistenceEventArgs<TData>(data));

                    return data;
                }
                catch (Exception e) {
					this.m_tracer.TraceError("Error : {0}", e);
					connection.Rollback ();
					throw new LocalPersistenceException(DataOperationType.Update, data, e);

				}

		}

		/// <summary>
		/// Obsolete the specified identified data
		/// </summary>
		/// <param name="data">Data.</param>
		public TData Obsolete (TData data)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));
			else if (data.Key == Guid.Empty)
				throw new InvalidOperationException ("Data missing key");

			DataPersistencePreEventArgs<TData> preArgs = new DataPersistencePreEventArgs<TData> (data);
			this.Obsoleting?.Invoke (this, preArgs);
			if (preArgs.Cancel) {
				this.m_tracer.TraceWarning ("Pre-Event handler indicates abort for {0}", data);
				return data;
			}

			// Obsolete object
			using (var connection = this.CreateConnection ()) 
				try
				{
					this.m_tracer.TraceVerbose("OBSOLETE {0}", data);
					connection.BeginTransaction ();

					data = this.Obsolete(connection, data);

					connection.Commit();

                    this.Obsoleted?.Invoke(this, new DataPersistenceEventArgs<TData>(data));

                    return data;
                }
                catch (Exception e) {
					this.m_tracer.TraceError("Error : {0}", e);
					connection.Rollback ();
					throw new LocalPersistenceException(DataOperationType.Obsolete, data, e);
				}

		}

		/// <summary>
		/// Get the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		public TData Get(Guid key)
		{
			return this.Query (o => o.Key == key)?.SingleOrDefault ();
		}

		/// <summary>
		/// Query the specified data
		/// </summary>
		/// <param name="query">Query.</param>
		public System.Collections.Generic.IEnumerable<TData> Query (System.Linq.Expressions.Expression<Func<TData, bool>> query) 
		{
			if (query == null)
				throw new ArgumentNullException (nameof (query));

			DataQueryPreEventArgs<TData> preArgs = new DataQueryPreEventArgs<TData> (query);
			this.Querying?.Invoke (this, preArgs);
			if (preArgs.Cancel) {
				this.m_tracer.TraceWarning ("Pre-Event handler indicates abort query {0}", query);
				return preArgs.Results;
			}

			// Query object
			using (var connection = this.CreateConnection ()) 
				try
				{
					this.m_tracer.TraceVerbose("QUERY {0}", query);

					var results = this.Query(connection, query);

					this.Queried?.Invoke (this, new DataQueryResultEventArgs<TData> (query, results));

					return results;

				}
				catch(Exception e) {
					this.m_tracer.TraceError("Error : {0}", e);
					throw;
				}

		}

		/// <summary>
		/// Query this instance.
		/// </summary>
		public virtual IEnumerable<TData> Query(String storedQueryName, IDictionary<String, Object> parms)
		{
			if (String.IsNullOrEmpty (storedQueryName))
				throw new ArgumentNullException (nameof (storedQueryName));
			else if (parms == null)
				throw new ArgumentNullException (nameof (parms));
			
			DataStoredQueryPreEventArgs<TData> preArgs = new DataStoredQueryPreEventArgs<TData> (storedQueryName, parms);
			this.Querying?.Invoke (this, preArgs);
			if (preArgs.Cancel) {
				this.m_tracer.TraceWarning ("Pre-Event handler indicates abort query {0}", storedQueryName);
				return preArgs.Results;
			}

			// Query object
			using (var connection = this.CreateConnection ()) 
				try
			{
				this.m_tracer.TraceVerbose("STORED QUERY {0}", storedQueryName);

				var results = this.Query(connection, storedQueryName, parms).ToList();

				this.Queried?.Invoke (this, new DataStoredQueryResultEventArgs<TData> (storedQueryName, parms, results));

				return results;

			}
			catch(Exception e) {
				this.m_tracer.TraceError("Error : {0}", e);
				throw;
			}

		}

		#endregion

		/// <summary>
		/// Get the current user UUID.
		/// </summary>
		/// <returns>The user UUID.</returns>
		protected Guid CurrentUserUuid(SQLiteConnection context)
		{
			if (ApplicationContext.Current.Principal == null)
				return Guid.Empty;
			var securityUser = context.Table<DbSecurityUser>().SingleOrDefault(o=>o.UserName == ApplicationContext.Current.Principal.Identity.Name);
			if (securityUser == null)
				throw new InvalidOperationException("Constraint Violation: User doesn't exist locally");
			return securityUser.Key;
		}

		/// <summary>
		/// Maps the data to a model instance
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="dataInstance">Data instance.</param>
		public abstract TData ToModelInstance(Object dataInstance, SQLiteConnection context);

		/// <summary>
		/// Froms the model instance.
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="modelInstance">Model instance.</param>
		public abstract Object FromModelInstance (TData modelInstance, SQLiteConnection context);

		/// <summary>
		/// Performthe actual insert.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		internal abstract TData Insert(SQLiteConnection context, TData data);
		/// <summary>
		/// Perform the actual update.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		internal abstract TData Update(SQLiteConnection context, TData data);
		/// <summary>
		/// Performs the actual obsoletion
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		internal abstract TData Obsolete(SQLiteConnection context, TData data);
		/// <summary>
		/// Performs the actual query
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="query">Query.</param>
		internal abstract IEnumerable<TData> Query(SQLiteConnection context, Expression<Func<TData, bool>> query, int offset = 0, int count = -1);
		/// <summary>
		/// Performs the actual query
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="query">Query.</param>
		internal abstract IEnumerable<TData> Query(SQLiteConnection context, String storedQueryName, IDictionary<String, Object> parms, int offset = 0, int count = -1);
	}
}

