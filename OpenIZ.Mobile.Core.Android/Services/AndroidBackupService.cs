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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Xamarin.Data;
using A = Android;

namespace OpenIZ.Mobile.Core.Android.Services
{
    /// <summary>
    /// Android 
    /// </summary>
    public class AndroidBackupService : XamarinBackupService
    {
        /// <summary>
        /// Get backup directory
        /// </summary>
        protected override string GetBackupDirectory(BackupMedia media)
        {
            switch (media)
            {
                case BackupMedia.Private:
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                case BackupMedia.Public:
                    var ossec = (ApplicationContext.Current as AndroidApplicationContext).CurrentActivity as IOperatingSystemSecurityService;
                    if (ossec.HasPermission(PermissionType.FileSystem) ||
                        ossec.RequestPermission(PermissionType.FileSystem))
                    {
                        var retVal = System.IO.Path.Combine(
                            A.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                            A.OS.Environment.DirectoryDocuments,
                            (AndroidApplicationContext.Current as AndroidApplicationContext).AndroidApplication.PackageName);
                        if (!System.IO.Directory.Exists(retVal))
                            System.IO.Directory.CreateDirectory(retVal);
                        return retVal;
                    }
                    else 
                        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                default:
                    throw new PlatformNotSupportedException("Don't support external media on this platform");
            }
        }
    }
}