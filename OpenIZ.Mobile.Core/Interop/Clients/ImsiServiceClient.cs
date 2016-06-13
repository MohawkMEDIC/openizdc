using System;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Http;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;
using OpenIZ.Core.Model;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;

namespace OpenIZ.Mobile.Core.Interop.Clients
{
	/// <summary>
	/// Represents the IMSI service client 
	/// </summary>
	public class ImsiServiceClient : ServiceClientBase
	{

		/// <summary>
		/// Creates a new service client
		/// </summary>
		/// <param name="clientName">Client name.</param>
		public ImsiServiceClient (String clientName) : base(clientName)
		{
			this.Client.Accept = "application/json";
		}

		/// <summary>
		/// Perform a query
		/// </summary>
		/// <param name="query">Query.</param>
		public Bundle Query<TModel>(Expression<Func<TModel, bool>> query) where TModel : IdentifiedData
		{
			// Map the query to HTTP parameters
			var queryParms = QueryExpressionBuilder.BuildQuery(query);

			// Resource name
			String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// The IMSI uses the XMLName as the root of the request
			var retVal = this.Client.Get<Bundle>(resourceName, queryParms.ToArray());

			// Queue up the result
			SynchronizationQueue.Inbound.Enqueue(retVal, OpenIZ.Mobile.Core.Synchronization.Model.DataOperationType.Sync);

			// Return value
			return retVal;
		}

	}
}

