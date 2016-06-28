using OpenIZ.Core.PCL.Http;
using OpenIZ.Mobile.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Interop
{
    /// <summary>
    /// Configuration sections
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the service description.
        /// </summary>
        /// <returns>The service description.</returns>
        /// <param name="me">Me.</param>
        public static ServiceClientDescription GetServiceDescription(this OpenIZConfiguration me, String clientName)
        {

            var configSection = me.GetSection<ServiceClientConfigurationSection>();
            return configSection.Client.Find(o => clientName == o.Name);

        }

        /// <summary>
        /// Gets the rest client.
        /// </summary>
        /// <returns>The rest client.</returns>
        /// <param name="me">Me.</param>
        /// <param name="clientName">Client name.</param>
        public static IRestClient GetRestClient(this ApplicationContext me, String clientName)
        {
            var configSection = me.Configuration.GetSection<ServiceClientConfigurationSection>();
            IRestClient client = Activator.CreateInstance(configSection.RestClientType, me.Configuration.GetServiceDescription(clientName)) as IRestClient;
            return client;
        }
    }
}
