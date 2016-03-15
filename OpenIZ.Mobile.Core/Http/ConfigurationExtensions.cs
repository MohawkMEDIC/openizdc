using System;
using OpenIZ.Mobile.Core.Configuration;

namespace OpenIZ.Mobile.Core.Http
{
	/// <summary>
	/// Configuration extensions.
	/// </summary>
	public static class ConfigurationExtensions
	{

		/// <summary>
		/// Gets the service description.
		/// </summary>
		/// <returns>The service description.</returns>
		/// <param name="me">Me.</param>
		public static ServiceClient GetServiceDescription(this OpenIZConfiguration me, String clientName)
		{

			var configSection = me.GetSection<ServiceClientConfigurationSection> ();
			return configSection.Client.Find (o => clientName == o.Name);

		}

		/// <summary>
		/// Gets the rest client.
		/// </summary>
		/// <returns>The rest client.</returns>
		/// <param name="me">Me.</param>
		/// <param name="clientName">Client name.</param>
		public static IRestClient GetRestClient(this ApplicationContext me, String clientName)
		{
			var configSection = me.Configuration.GetSection<ServiceClientConfigurationSection> ();
			IRestClient client = Activator.CreateInstance (configSection.RestClientType, me.Configuration.GetServiceDescription (clientName)) as IRestClient;
			return client;
		}

	}
}

