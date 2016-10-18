using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Security
{

    /// <summary>
    /// Indicates whether a remote symmetric key validation error should be ignored
    /// </summary>
    public delegate bool SymmetricKeyValidationCallback(Object source, int keyId, String issuer);

    /// <summary>
    /// Symmetric key validation manager
    /// </summary>
    public static class TokenValidationManager
    {

        /// <summary>
        /// Symmtric key validation callback
        /// </summary>
        public static event SymmetricKeyValidationCallback SymmetricKeyValidationCallback;

        /// <summary>
        /// Server URI
        /// </summary>
        internal static bool OnSymmetricKeyValidationCallback(Object source, int keyId, String issuer)
        {
            return SymmetricKeyValidationCallback?.Invoke(source, keyId, issuer) == true;
        } 

    }
}
