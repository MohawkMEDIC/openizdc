using System;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Http;
using OpenIZ.Core.Model;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Interop.Clients
{
	/// <summary>
	/// Represents a basic service client
	/// </summary>
	public abstract class ServiceClientBase
	{
		// The configuration
		private ServiceClientDescription m_configuration;
		// The rest client
		private IRestClient m_restClient;

		/// <summary>
		/// Gets the client.
		/// </summary>
		/// <value>The client.</value>
		protected IRestClient Client { 
			get { return this.m_restClient;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Interop.Clients.ServiceClientBase"/> class.
		/// </summary>
		/// <param name="clientName">Client name.</param>
		public ServiceClientBase(String clientName)
		{

			this.m_configuration = ApplicationContext.Current.Configuration.GetServiceDescription (clientName);
			this.m_restClient = (IRestClient)Activator.CreateInstance(ApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection> ().RestClientType, this.m_configuration);

		}

	}
}

