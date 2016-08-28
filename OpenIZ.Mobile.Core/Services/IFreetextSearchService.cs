using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Free text search service
    /// </summary>
    public interface IFreetextSearchService
    {

        /// <summary>
        /// Performs a full index scan
        /// </summary>
        bool Index();

        /// <summary>
        /// Performs a freetext search 
        /// </summary>
        IEnumerable<TEntity> Search<TEntity>(String term, int offset, int? count, out int totalResults) where TEntity : Entity;
        /// <summary>
        /// Search based on tokens
        /// </summary>
        IEnumerable<TEntity> Search<TEntity>(String[] term, int offset, int? count, out int totalResults) where TEntity : Entity;

    }
}
