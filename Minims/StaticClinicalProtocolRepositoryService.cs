using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Services;

namespace Minims
{
    /// <summary>
    /// Static clinical protocol repository
    /// </summary>
    public class StaticClinicalProtocolRepositoryService : IClinicalProtocolRepositoryService
    {

        public IEnumerable<Protocol> FindProtocol(Expression<Func<Protocol, bool>> predicate, int offset, int? count, out int totalResults)
        {
            throw new NotImplementedException();
        }

        public Protocol InsertProtocol(Protocol data)
        {
            throw new NotImplementedException();
        }
    }
}