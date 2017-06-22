using OpenIZ.Core.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Null credentials
    /// </summary>
    public class NullCredentials : Credentials
    {

        /// <summary>
        /// Null credentials ctor
        /// </summary>
        public NullCredentials() : base(null)
        {

        }

        /// <summary>
        /// Get HTTP headers
        /// </summary>
        public override Dictionary<string, string> GetHttpHeaders()
        {
            return new Dictionary<string, string>();
        }
    }
}
