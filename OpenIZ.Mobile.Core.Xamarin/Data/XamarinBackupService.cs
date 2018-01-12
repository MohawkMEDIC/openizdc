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
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using SharpCompress.Archives.Tar;
using System.IO.Compression;
using SharpCompress.Readers.Tar;
using SharpCompress.Writers.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using System.Xml.Serialization;
using OpenIZ.Mobile.Core.Interop.AMI;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Xamarin.Security;
using OpenIZ.Mobile.Core.Security.Audit;
using System.Net.Security;

namespace OpenIZ.Mobile.Core.Xamarin.Data
{
    /// <summary>
    /// Xamarin backup service
    /// </summary>
    public class XamarinBackupService : IBackupService
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(XamarinBackupService));

        /// <summary>
        /// Get backup directory
        /// </summary>
        protected virtual string GetBackupDirectory(BackupMedia media)
        {
            switch (media)
            {
                case BackupMedia.Private:
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                case BackupMedia.Public:
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                default:
                    throw new PlatformNotSupportedException("Don't support external media on this platform");
            }
        }

        /// <summary>
        /// Get the last backup date
        /// </summary>
        private string GetLastBackup(BackupMedia media)
        {
            var directoryName = this.GetBackupDirectory(media);
            return Directory.GetFiles(directoryName, "*.oiz.tar.gz").OrderByDescending(o => o).FirstOrDefault();

        }

        /// <summary>
        /// TAR Archive
        /// </summary>
        private void BackupDirectory(TarWriter archive, String dir, String rootDirectory)
        {
            if (Path.GetFileName(dir) == "log") return;

            // Backup
            foreach (var itm in Directory.GetDirectories(dir))
                this.BackupDirectory(archive, itm, rootDirectory);

            // Add files
            foreach (var itm in Directory.GetFiles(dir))
            {
                this.m_tracer.TraceVerbose("Backing up {0}", itm);
                archive.Write(itm.Replace(rootDirectory, ""), File.OpenRead(itm), DateTime.Now);
            }
        }

        /// <summary>
        /// Perform backup on the specified media
        /// </summary>
        public void Backup(BackupMedia media, String password = null)
        {

            // Make a determination that the user is allowed to perform this action
            if(AuthenticationContext.Current.Principal != AuthenticationContext.SystemPrincipal)
                new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PolicyIdentifiers.ExportClinicalData).Demand();

            // Get the output medium
            var directoryName = this.GetBackupDirectory(media);
            string fileName = Path.Combine(directoryName, $"oizdc-{DateTime.Now.ToString("yyyyMMddHHmm")}.oiz.tar");

            // Confirm if the user really really wants to backup
            if (String.IsNullOrEmpty(password) &&
                !ApplicationContext.Current.Confirm(Strings.locale_backup_confirm)) return;

            // TODO: Audit the backup to the data to the central server
            AuditUtil.AuditDataExport();

            // Try to backup the data
            try
            {

                this.m_tracer.TraceInfo("Beginning backup to {0}..", fileName);
                ApplicationContext.Current?.SetProgress(Strings.locale_backup, 0.25f);
                // Backup folders first
                var sourceDirectory = XamarinApplicationContext.Current.ConfigurationManager.ApplicationDataDirectory;

                using (var fs = File.Create(fileName))
                using (var writer = new SharpCompress.Writers.Tar.TarWriter(fs, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.None)))
                {
                    this.BackupDirectory(writer, sourceDirectory, sourceDirectory);

                    var appInfo = new ApplicationInfo(false).ToDiagnosticReport();
                    // Output appInfo
                    using (var ms = new MemoryStream()) {
                        XmlSerializer xsz = new XmlSerializer(appInfo.GetType());
                        xsz.Serialize(ms, appInfo);
                        ms.Flush();
                        ms.Seek(0, SeekOrigin.Begin);
                        writer.Write(".appinfo.xml", ms, DateTime.Now);
                    }

                    // Output declaration statement
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes($"User {(AuthenticationContext.Current?.Principal?.Identity?.Name ?? "SYSTEM")} created this backup on {DateTime.Now}. The end user was asked to confirm this decision to backup and acknolwedges all responsibilities for guarding this file.")))
                        writer.Write("DISCLAIMER.TXT", ms, DateTime.Now);
                }

                this.m_tracer.TraceInfo("Beginning compression {0}..", fileName);
                using (var fs = File.OpenRead(fileName))
                using (var gzs = new GZipStream(File.Create(fileName + ".gz"), CompressionMode.Compress))
                {
                    int br = 4096;
                    byte[] buffer = new byte[br];
                    while(br == 4096)
                    {
                        br = fs.Read(buffer, 0, 4096);
                        gzs.Write(buffer, 0, br);
                        ApplicationContext.Current?.SetProgress(Strings.locale_backup_compressing, (float)fs.Position / (float)fs.Length * 0.5f + 0.5f);
                    }
                }
                File.Delete(fileName);
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error backing up to {0}: {1}", fileName, ex);
                throw;
            }
        }

        /// <summary>
        /// Restore the specified data
        /// </summary>
        public void Restore(BackupMedia media, String password = null)
        {
            // Get the last backup
            var lastBackup = this.GetLastBackup(media);

            if (!ApplicationContext.Current.Confirm(String.Format(Strings.locale_backup_restore_confirm, new FileInfo(lastBackup).CreationTime.ToString("ddd MMM dd, yyyy"))))
                return;

            try
            {
                this.m_tracer.TraceInfo("Beginning restore of {0}...", lastBackup);
                ApplicationContext.Current?.SetProgress(Strings.locale_backup_restore, 0.0f);
                var sourceDirectory = XamarinApplicationContext.Current.ConfigurationManager.ApplicationDataDirectory;

                using (var fs = File.OpenRead(lastBackup))
                using (var gzs = new GZipStream(fs, CompressionMode.Decompress))
                using (var tr = TarReader.Open(gzs))
                {
                                        
                    // Move to next entry & copy 
                    while (tr.MoveToNextEntry())
                    {

                        this.m_tracer.TraceVerbose("Extracting : {0}", tr.Entry.Key);
                        if (tr.Entry.Key == "DISCLAIMER.TXT" || tr.Entry.Key == ".appinfo.xml") continue;
                        var destDir = Path.Combine(sourceDirectory, tr.Entry.Key.Replace('/', Path.DirectorySeparatorChar));
                        if (!Directory.Exists(Path.GetDirectoryName(destDir)))
                            Directory.CreateDirectory(Path.GetDirectoryName(destDir));
                        if (!tr.Entry.IsDirectory)
                            using (var s = tr.OpenEntryStream())
                            using (var ofs = File.Create(Path.Combine(sourceDirectory, tr.Entry.Key)))
                                s.CopyTo(ofs);
                    }
                }
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error restoring backup {0}: {1}", lastBackup, ex);
                throw;
            }
        }

        /// <summary>
        /// Determine if backup is available on the specified media
        /// </summary>
        public bool HasBackup(BackupMedia media)
        {
            return this.GetLastBackup(media) != null;
        }
    }
}
