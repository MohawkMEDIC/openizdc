using System;

using System.Linq;
using System.Reflection;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model.Map;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Data.Persistence;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Data.Model;
using SQLite;
using System.Collections.Generic;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Exceptions;
using System.Linq.Expressions;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZ.Mobile.Core.Interop;
using OpenIZ.Core.Model.Collection;

namespace OpenIZ.Mobile.Core.Data
{


	/// <summary>
	/// Represents a persistence service which uses the IMSI only in online mode
	/// </summary>
	public class ImsiPersistenceService
	{

        /// <summary>
        /// Generic versioned persister service for any non-customized persister
        /// </summary>
        internal class GenericImsiPersister<TModel> : IDataPersistenceService<TModel>
            where TModel : IdentifiedData, new()
        {
            /// <summary>
            /// Inserted data
            /// </summary>
            public event EventHandler<DataPersistenceEventArgs<TModel>> Inserted;
            /// <summary>
            /// Inserting data
            /// </summary>
            public event EventHandler<DataPersistencePreEventArgs<TModel>> Inserting;
            /// <summary>
            /// Obsoleted data
            /// </summary>
            public event EventHandler<DataPersistenceEventArgs<TModel>> Obsoleted;
            /// <summary>
            /// Obsoleting data
            /// </summary>
            public event EventHandler<DataPersistencePreEventArgs<TModel>> Obsoleting;
            /// <summary>
            /// Queried data
            /// </summary>
            public event EventHandler<DataQueryEventArgsBase<TModel>> Queried;
            /// <summary>
            /// Querying data
            /// </summary>
            public event EventHandler<DataQueryEventArgsBase<TModel>> Querying;
            /// <summary>
            /// Updated data 
            /// </summary>
            public event EventHandler<DataPersistenceEventArgs<TModel>> Updated;
            /// <summary>
            /// Updating data
            /// </summary>
            public event EventHandler<DataPersistencePreEventArgs<TModel>> Updating;

            // Service client
            private ImsiServiceClient m_client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("ims"));

            /// <summary>
            /// Count the number of items
            /// </summary>
            public int Count(Expression<Func<TModel, bool>> predicate)
            {
                var retVal = this.m_client.Query(predicate);
                return (retVal as Bundle)?.TotalResults ?? ((retVal as IdentifiedData != null) ? 1 : 0);
            }

            /// <summary>
            /// Gets the specified item
            /// </summary>
            public TModel Get(Guid key)
            {
                // Gets the specified data
                return this.m_client.Get<TModel>(key, null) as TModel;
            }

            /// <summary>
            /// Inserts the specified data
            /// </summary>
            public object Insert(object data)
            {
                return this.Insert(data as TModel);
            }

            /// <summary>
            /// Inserts the specified typed data
            /// </summary>
            public TModel Insert(TModel data)
            {
                return this.m_client.Create(data);
            }

            /// <summary>
            /// Obsoletes the specified data
            /// </summary>
            public object Obsolete(object data)
            {
                return this.Obsolete(data as TModel);
            }

            /// <summary>
            /// Obsoletes the specified data
            /// </summary>
            public TModel Obsolete(TModel data)
            {
                return this.m_client.Obsolete(data);
            }

            /// <summary>
            /// Query the specified data
            /// </summary>
            public IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query)
            {
                int t;
                return this.Query(query, 0, null, out t);
            }

            /// <summary>
            /// Executes the specified named query
            /// </summary>
            public IEnumerable<TModel> Query(string queryName, IDictionary<string, object> parameters)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Query the specifie data
            /// </summary>
            public IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query, int offset, int? count, out int totalResults)
            {
                var data = this.m_client.Query(query, offset, count);
                (data as Bundle)?.Reconstitute();
                offset = (data as Bundle)?.Offset ?? offset;
                count = (data as Bundle)?.Count ?? count;
                totalResults = (data as Bundle)?.TotalResults ?? 1;
                return (data as Bundle)?.Item.OfType<TModel>() ?? new List<TModel>() { data as TModel };
            }

            /// <summary>
            /// Executes the specified named query
            /// </summary>
            public IEnumerable<TModel> Query(string queryName, IDictionary<string, object> parameters, int offset, int? count, out int totalResults)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Updates the specified data
            /// </summary>
            public object Update(object data)
            {
                return this.Update(data as TModel);
            }

            /// <summary>
            /// Update the specified data
            /// </summary>
            public TModel Update(TModel data)
            {
                return this.m_client.Update(data);
            }

            /// <summary>
            /// Gets the specified object
            /// </summary>
            object IDataPersistenceService.Get(Guid id)
            {
                return this.Get(id) as TModel;
            }
        }

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(ImsiPersistenceService));

        // Constructor
		public ImsiPersistenceService ()
		{
			var appSection = ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection> ();

            // Now iterate through the map file and ensure we have all the mappings, if a class does not exist create it
            try
            {                
                foreach(var itm in typeof(IdentifiedData).GetTypeInfo().Assembly.ExportedTypes.Where(o=>typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && ! o.GetTypeInfo().IsAbstract))
                {
                    // Is there a persistence service?
                    var idpType = typeof(IDataPersistenceService<>);
                    idpType = idpType.MakeGenericType(itm);

                    this.m_tracer.TraceVerbose("Creating persister {0}", itm);

                    // Is the model class a Versioned entity?
                    var pclass = typeof(GenericImsiPersister<>);
                    pclass = pclass.MakeGenericType(itm);
                    appSection.Services.Add(Activator.CreateInstance(pclass));
                }
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error initializing local persistence: {0}", e);
                throw e;
            }
        }
    }
}

