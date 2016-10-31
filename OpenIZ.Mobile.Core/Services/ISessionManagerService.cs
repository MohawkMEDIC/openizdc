using OpenIZ.Mobile.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Represents a session manager service
    /// </summary>
    public interface ISessionManagerService
    {

        /// <summary>
        /// Authenticates the specified username/password pair
        /// </summary>
        SessionInfo Authenticate(String userName, String password);

        /// <summary>
        /// Authenticates the specified username/password/tfasecret pair
        /// </summary>
        SessionInfo Authenticate(String userName, String password, String tfaSecret);

        /// <summary>
        /// Refreshes the specified session
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        SessionInfo Refresh(SessionInfo session, String password);
       
        /// <summary>
        /// Deletes (abandons) the session
        /// </summary>
        SessionInfo Delete(Guid sessionId);

        /// <summary>
        /// Gets the session
        /// </summary>
        SessionInfo Get(Guid sessionId);

    }
}
