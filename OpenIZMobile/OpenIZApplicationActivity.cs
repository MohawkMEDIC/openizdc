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
 * Date: 2017-9-1
 */
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Threading;

namespace OpenIZMobile
{
    /// <summary>
    /// OpenIZ application activity
    /// </summary>
    public abstract class OpenIZApplicationActivity : Activity, IOperatingSystemSecurityService
    {
        private ManualResetEvent m_permissionEvent = new ManualResetEvent(false);

        /// <summary>
        /// Return true if this object has permission
        /// </summary>
        public bool HasPermission(PermissionType permission)
        {
            if ((int)Build.VERSION.SdkInt < 23)
                return true;
            else
                switch (permission)
                {
                    case PermissionType.FileSystem:
                        return this.CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted;
                    case PermissionType.GeoLocation:
                        return this.CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted;
                    default:
                        return false;
                }
        }

        /// <summary>
        /// Requests permission
        /// </summary>
        public bool RequestPermission(PermissionType permission)
        {
            if ((int)Build.VERSION.SdkInt < 23)
                return true;
            else
            {
                this.m_permissionEvent.Reset();
                String permissionString = String.Empty;
                switch (permission)
                {
                    case PermissionType.FileSystem:
                        permissionString = Android.Manifest.Permission.WriteExternalStorage;
                        break;
                    case PermissionType.GeoLocation:
                        permissionString = Android.Manifest.Permission.AccessCoarseLocation;
                        break;
                    default:
                        return false;
                }
                this.RequestPermissions(new string[] { permissionString }, 0);
                this.m_permissionEvent.WaitOne();
                return this.CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted;
            }
        }

        /// <summary>
        /// Request permission result
        /// </summary>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            this.m_permissionEvent.Set();
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}