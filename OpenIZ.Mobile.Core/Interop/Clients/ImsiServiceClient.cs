using System;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Http;
using OpenIZ.Mobile.Core.Interop.Util;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;
using OpenIZ.Core.Model;
using System.Linq.Expressions;

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
		public IEnumerable<TModel> Query<TModel>(Expression<Func<TModel, bool>> query) where TModel : IdentifiedData
		{
			// Map the query to HTTP parameters
			var queryParms = HttpQueryExpressionBuilder.BuildQuery(query);

			// Resource name
			String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// The IMSI uses the XMLName as the root of the request
			return this.Client.Get<List<TModel>>(resourceName, queryParms.ToArray());
		}

	}
}

