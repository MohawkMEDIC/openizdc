using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Permission types
    /// </summary>
    public enum PermissionType
    {
        GeoLocation,
        FileSystem
    }

    /// <summary>
    /// Represents a security service for the operating system
    /// </summary>
    public interface IOperatingSystemSecurityService
    {

        /// <summary>
        /// True if the current execution context has the requested permission
        /// </summary>
        bool HasPermission(PermissionType permission);

        /// <summary>
        /// Request permission
        /// </summary>
        bool RequestPermission(PermissionType permission);
    }
}
