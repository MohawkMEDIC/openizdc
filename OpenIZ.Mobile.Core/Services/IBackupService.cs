using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Gets or sets the backup media
    /// </summary>
    public enum BackupMedia
    {
        ExternalPublic,
        Public,
        Private
    }

    /// <summary>
    /// Represents a service that can back-up data to/from another location
    /// </summary>
    public interface IBackupService
    {

        /// <summary>
        /// Backup media
        /// </summary>
        void Backup(BackupMedia media, String password = null);

        /// <summary>
        /// Restore from media
        /// </summary>
        void Restore(BackupMedia media, String password = null);

        /// <summary>
        /// Has backup on the specified media
        /// </summary>
        bool HasBackup(BackupMedia media);
    }
}
