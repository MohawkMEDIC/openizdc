using OpenIZ.Mobile.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Exceptions
{
    /// <summary>
    /// Session expired exception
    /// </summary>
    public class SessionExpiredException : SecurityException
    {
        /// <summary>
        /// Session has expired
        /// </summary>
        public SessionExpiredException() : base(Strings.locale_session_expired)
        {

        }
    }
}
