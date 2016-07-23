using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Android.Security
{
    /// <summary>
    /// System principal is used in the Android Security assembly only 
    /// </summary>
    internal class SystemPrincipal : GenericPrincipal
    {

        public SystemPrincipal() : base(new SystemIdentity(), null)
        {

        }
    }

    /// <summary>
    /// Identity representing SYSTEM user
    /// </summary>
    internal class SystemIdentity : GenericIdentity
    {

        /// <summary>
        /// System identity
        /// </summary>
        public SystemIdentity() : base("SYSTEM")
        {

        }

        /// <summary>
        /// SYSTEM is always SYSTEM
        /// </summary>
        public override string Name
        {
            get
            {
                return "SYSTEM";
            }
        }

        /// <summary>
        /// SYSTEM is always authenticated
        /// </summary>
        public override bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }
    }
}