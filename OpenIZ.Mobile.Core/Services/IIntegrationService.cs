using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Represents an integration service which is responsible for sending and
    /// pulling data to/from remote sources
    /// </summary>
    public interface IIntegrationService
    {

        /// <summary>
        /// Instructs the integration service to retrieve the specified object
        /// </summary>
        IdentifiedData Get(Type modelType, Guid key, Guid? versionKey);

        /// <summary>
        /// Gets the specified object
        /// </summary>
        TModel Get<TModel>(Guid key, Guid? versionKey) where TModel : IdentifiedData;

        /// <summary>
        /// Instructs the integration service to save the specified object
        /// </summary>
        void Insert(IdentifiedData data);

        /// <summary>
        /// Instructs the integration service to save the specified object
        /// </summary>
        void Update(IdentifiedData data);

        /// <summary>
        /// Instructs the integration service to save the specified object
        /// </summary>
        void Obsolete(IdentifiedData data);

        /// <summary>
        /// Instructs the integration service to locate a specified object(s)
        /// </summary>
        IdentifiedData Find<TModel>(Expression<Func<TModel, bool>> predicate, int offset, int? count) where TModel : IdentifiedData;

        /// <summary>
        /// Determines if the integration target is available
        /// </summary>
        /// <returns></returns>
        bool IsAvailable();
    }
}
