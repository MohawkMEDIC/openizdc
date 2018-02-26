/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2017-10-30
 */
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
