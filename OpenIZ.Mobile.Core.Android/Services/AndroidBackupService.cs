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
                    var retVal = System.IO.Path.Combine(
                        A.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                        A.OS.Environment.DirectoryDocuments, 
                        (AndroidApplicationContext.Current as AndroidApplicationContext).AndroidApplication.PackageName);
                    if (!System.IO.Directory.Exists(retVal))
                        System.IO.Directory.CreateDirectory(retVal);
                    return retVal;
                default:
                    throw new PlatformNotSupportedException("Don't support external media on this platform");
            }
        }
    }
}