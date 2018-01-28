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
using System;
using OpenIZ.Core.Model;
using OpenIZ.Core.Services;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Services
{

    /// <summary>
    /// Base class for query event args
    /// </summary>
    public abstract class DataQueryEventArgsBase<TData> : EventArgs where TData : class
    {

        /// <summary>
        /// Data query event ctor
        /// </summary>
        public DataQueryEventArgsBase(int offset, int? count)
        {
            this.Offset = offset;
            this.Count = count;
        }

        /// <summary>
        /// Gets the offset requested
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets the count requested
        /// </summary>
        public int? Count { get; set; }


        /// <summary>
        /// Gets the total amount of results
        /// </summary>
        public int TotalResults { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public IEnumerable<TData> Results
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Data Query pre-event args
	/// </summary>
	public abstract class DataQueryEventArgs<TData> : DataQueryEventArgsBase<TData> where TData : class
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.DataQueryPreEventArgs"/> class.
		/// </summary>
		/// <param name="query">Query.</param>
		public DataQueryEventArgs (Expression<Func<TData, bool>> query, int offset, int? count) : base(offset, count)
		{
			this.Query = query;
        }

       
		/// <summary>
		/// Gets or sets the query.
		/// </summary>
		/// <value>The query.</value>
		public Expression<Func<TData, bool>> Query {
			get;
			set;
		}

    }

	/// <summary>
	/// Data query result event arguments.
	/// </summary>
	public class DataQueryResultEventArgs<TData> : DataQueryEventArgs<TData> where TData : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.DataQueryResultEventArgs"/> class.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="results">Results.</param>
		public DataQueryResultEventArgs (Expression<Func<TData, bool>> query, IEnumerable<TData> results, int offset, int? count, int totalResults) : base(query, offset, count)
		{
			this.Results = results;
            this.TotalResults = totalResults;
		}

    }

	/// <summary>
	/// Data query pre event arguments.
	/// </summary>
	public class DataQueryPreEventArgs<TData> : DataQueryEventArgs<TData> where TData : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.DataQueryPreEventArgs"/> class.
		/// </summary>
		/// <param name="query">Query.</param>
		public DataQueryPreEventArgs (Expression<Func<TData, bool>> query, int offset, int? count) : base(query, offset, count)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance cancel.
		/// </summary>
		/// <value><c>true</c> if this instance cancel; otherwise, <c>false</c>.</value>
		public bool Cancel {
			get;
			set;
		}
	}

    /// <summary>
    /// Data query result event arguments.
    /// </summary>
    public abstract class DataStoredQueryEventArgsBase<TData> : DataQueryEventArgsBase<TData> where TData : class
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.DataQueryResultEventArgs"/> class.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="results">Results.</param>
		public DataStoredQueryEventArgsBase(String query, IDictionary<String, Object> parms, int offset, int? count) : base(offset, count)
		{
            this.StoredQueryName = query;
            this.Parameters = parms;
        }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IDictionary<String, Object> Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public String StoredQueryName
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Data query result event arguments.
    /// </summary>
    public class DataStoredQueryResultEventArgs<TData> : DataStoredQueryEventArgsBase<TData> where TData : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.DataQueryResultEventArgs"/> class.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="results">Results.</param>
		public DataStoredQueryResultEventArgs (String query, IDictionary<String, Object> parms, IEnumerable<TData> results, int offset, int? count, int totalResults) : base(query, parms, offset, count)
		{
			this.Results = results;
            this.TotalResults = totalResults;
		}

	}

	/// <summary>
	/// Data query pre event arguments.
	/// </summary>
	public class DataStoredQueryPreEventArgs<TData> : DataStoredQueryEventArgsBase<TData> where TData : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.DataQueryResultEventArgs"/> class.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="results">Results.</param>
		public DataStoredQueryPreEventArgs (String query, IDictionary<String, Object> parms, int offset, int? count) : base(query, parms, offset, count) 
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance cancel.
		/// </summary>
		/// <value><c>true</c> if this instance cancel; otherwise, <c>false</c>.</value>
		public bool Cancel {
			get;
			set;
		}
	}

	/// <summary>
	/// Data persistence pre event arguments.
	/// </summary>
	public class DataPersistencePreEventArgs<TData> : DataPersistenceEventArgs<TData> where TData : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.DataPersistencePreEventArgs"/> class.
		/// </summary>
		/// <param name="data">Data.</param>
		public DataPersistencePreEventArgs (TData data) : base(data)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance cancel.
		/// </summary>
		/// <value><c>true</c> if this instance cancel; otherwise, <c>false</c>.</value>
		public bool Cancel {
			get;
			set;
		}

	}

	/// <summary>
	/// Data persistence event arguments.
	/// </summary>
	public class DataPersistenceEventArgs<TData> : EventArgs where TData : class
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Services.DataPersistenceEventArgs"/> class.
		/// </summary>
		/// <param name="data">Data.</param>
		public DataPersistenceEventArgs (TData data)
		{
			this.Data = data;
		}
			
		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>The data.</value>
		public TData Data {
			get;
			set;
		}

	}
    
	/// <summary>
	/// Represents a data persistence service which is capable of storing and retrieving data
	/// to/from a data store
	/// </summary>
	public interface IDataPersistenceService<TData> : IDataPersistenceService where TData : IdentifiedData
	{
		/// <summary>
		/// Occurs when inserted.
		/// </summary>
		event EventHandler<DataPersistenceEventArgs<TData>> Inserted;
		/// <summary>
		/// Occurs when inserting.
		/// </summary>
		event EventHandler<DataPersistencePreEventArgs<TData>> Inserting;
		/// <summary>
		/// Occurs when updated.
		/// </summary>
		event EventHandler<DataPersistenceEventArgs<TData>> Updated;
		/// <summary>
		/// Occurs when updating.
		/// </summary>
		event EventHandler<DataPersistencePreEventArgs<TData>> Updating;
		/// <summary>
		/// Occurs when obsoleted.
		/// </summary>
		event EventHandler<DataPersistenceEventArgs<TData>> Obsoleted;
		/// <summary>
		/// Occurs when obsoleting.
		/// </summary>
		event EventHandler<DataPersistencePreEventArgs<TData>> Obsoleting;
		/// <summary>
		/// Occurs when queried.
		/// </summary>
		event EventHandler<DataQueryEventArgsBase<TData>> Queried;
		/// <summary>
		/// Occurs when querying.
		/// </summary>
		event EventHandler<DataQueryEventArgsBase<TData>> Querying;

		/// <summary>
		/// Insert the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		TData Insert(TData data);

        /// <summary>
        /// Update the specified data
        /// </summary>
        /// <param name="data">Data.</param>
        TData Update(TData data);

        /// <summary>
        /// Obsolete the specified identified data
        /// </summary>
        /// <param name="data">Data.</param>
        TData Obsolete(TData data);

		/// <summary>
		/// Get the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		TData Get(Guid key);

		/// <summary>
		/// Query the specified data
		/// </summary>
		/// <param name="query">Query.</param>
		IEnumerable<TData> Query(Expression<Func<TData, bool>> query);

        /// <summary>
        /// Query the specified data
        /// </summary>
        /// <param name="query">Query.</param>
        IEnumerable<TData> Query(Expression<Func<TData, bool>> query, int offset, int? count, out int totalResults, Guid queryId);

        /// <summary>
        /// Query the specified data
        /// </summary>
        /// <param name="query">Query.</param>
        IEnumerable<TData> QueryFast(Expression<Func<TData, bool>> query, int offset, int? count, out int totalResults, Guid queryId);

        /// <summary>
        /// Query the specified data
        /// </summary>
        /// <param name="query">Query.</param>
        IEnumerable<TData> QueryExplicitLoad(Expression<Func<TData, bool>> query, int offset, int? count, out int totalResults, Guid queryId, IEnumerable<String> expandProperties);
        
        /// <summary>
        /// Executes a stored query
        /// </summary>
        IEnumerable<TData> Query(String queryName, IDictionary<String, Object> parameters);

        /// <summary>
        /// Executes a stored query
        /// </summary>
        IEnumerable<TData> Query(String queryName, IDictionary<String, Object> parameters, int offset, int? count, out int totalResults, Guid queryId);

        /// <summary>
        /// Performs a fast count
        /// </summary>
        int Count(Expression<Func<TData, bool>> p);
    }
}


