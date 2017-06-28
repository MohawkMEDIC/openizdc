using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Exceptions
{
    /// <summary>
    /// Indicates that a data operation failed due to a duplicate key
    /// </summary>
    public class DuplicateKeyException : Exception
    {

        /// <summary>
        /// Duplicate key exception
        /// </summary>
        public DuplicateKeyException(String key) : base (key)
        {

        }
    }
}
