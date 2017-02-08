using OpenIZ.Core.Services;
using System;
using System.Collections;
using System.Security.Principal;
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.Mobile.Core.Data
{
    /// <summary>
    /// Represents an ADO based IDataPersistenceServie
    /// </summary>
    public interface ILocalPersistenceService : IDataPersistenceService
    {
        /// <summary>
        /// Inserts the specified object
        /// </summary>
        Object Insert(LocalDataContext context, Object data);

        /// <summary>
        /// Updates the specified data
        /// </summary>
        Object Update(LocalDataContext context, Object data);

        /// <summary>
        /// Obsoletes the specified data
        /// </summary>
        Object Obsolete(LocalDataContext context, Object data);

        /// <summary>
        /// Gets the specified data
        /// </summary>
        Object Get(LocalDataContext context, Guid id);

        /// <summary>
        /// Map to model instance
        /// </summary>
        Object ToModelInstance(object domainInstance, LocalDataContext context);
    }

    /// <summary>
    /// ADO associative persistence service
    /// </summary>
    public interface ILocalAssociativePersistenceService : ILocalPersistenceService
    {
        /// <summary>
        /// Get the set objects from the source
        /// </summary>
        IEnumerable GetFromSource(LocalDataContext context, Guid id, decimal? versionSequenceId);
    }
}