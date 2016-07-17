using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Core.Http;
using OpenIZ.Core.Model.Collection;
using System.Net.NetworkInformation;
using System.Net;
using OpenIZ.Mobile.Core.Interop;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
    /// Represents an integration service which 
    /// </summary>
    public class ImsiIntegrationService : IIntegrationService
    {

       
        /// <summary>
        /// Finds the specified model
        /// </summary>
        public IdentifiedData Find<TModel>(Expression<Func<TModel, bool>> predicate, int offset, int? count) where TModel : IdentifiedData
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Gets the specified model object
        /// </summary>
        public IdentifiedData Get(Type modelType, Guid key, Guid? version)
        {
            var method = this.GetType().GetRuntimeMethod("Get", new Type[] { typeof(Guid), typeof(Guid?) }).MakeGenericMethod(new Type[] { modelType });
            return method.Invoke(this, new object[] { key, version }) as IdentifiedData;
        }

        /// <summary>
        /// Gets the specified object
        /// </summary>
        public TModel Get<TModel>(Guid key, Guid? versionKey) where TModel : IdentifiedData
        {
            ImsiServiceClient client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));
            var retVal = client.Get<TModel>(key, versionKey);
            if (retVal is Bundle)
            {
                (retVal as Bundle)?.Reconstitute();
                return (retVal as Bundle).Entry as TModel;
            }
            else
                return retVal as TModel;
        }
        
        /// <summary>
        /// Inserts the specified data
        /// </summary>
        public void Insert(IdentifiedData data)
        {
            ImsiServiceClient client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));
            var method = typeof(ImsiServiceClient).GetRuntimeMethods().FirstOrDefault(o=>o.Name == "Create" && o.GetParameters().Length == 1);
            method.MakeGenericMethod(new Type[] { data.GetType() });
            method.Invoke(this, new object[] { data });
        }

        /// <summary>
        /// Makes the determination whether the service is available
        /// </summary>
        public bool IsAvailable()
        {
            var restClient = ApplicationContext.Current.GetRestClient("imsi");
            ImsiServiceClient client = new ImsiServiceClient(restClient);

            INetworkInformationService nis = ApplicationContext.Current.GetService<INetworkInformationService>();

            // 1. Is there a network avaialble?
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return false;
            else if (restClient.Description.Endpoint.All(o=>nis.Ping(o.Address) == 0)) // 2. Can we ping a well-known service?
                return false;
            else // Head is up?
            {
                try
                {
                    restClient.Options<String>("/");
                    return true;
                }
                catch(WebException e)
                { 
                    // Did we get anything back?
                    return e.Response != null && 
                        e.Status != WebExceptionStatus.SendFailure &&
                        e.Status != WebExceptionStatus.ConnectFailure;
                }
            }
        }

        /// <summary>
        /// Obsoletes the specified data
        /// </summary>
        public void Obsolete(IdentifiedData data)
        {
            ImsiServiceClient client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));
            var method = typeof(ImsiServiceClient).GetRuntimeMethods().FirstOrDefault(o => o.Name == "Obsolete" && o.GetParameters().Length == 1);
            method.MakeGenericMethod(new Type[] { data.GetType() });
            method.Invoke(this, new object[] { data });
        }

        /// <summary>
        /// Updates the specified object
        /// </summary>
        public void Update(IdentifiedData data)
        {
            ImsiServiceClient client = new ImsiServiceClient(ApplicationContext.Current.GetRestClient("imsi"));
            var method = typeof(ImsiServiceClient).GetRuntimeMethods().FirstOrDefault(o => o.Name == "Update" && o.GetParameters().Length == 1);
            method.MakeGenericMethod(new Type[] { data.GetType() });
            method.Invoke(this, new object[] { data });
        }
    }
}
