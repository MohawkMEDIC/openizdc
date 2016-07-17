using MohawkCollege.Util.Console.Parameters;
using SharpCompress.Reader.Tar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BrainBug
{
    /// <summary>
    /// This is the brain bug... Just like in Starship troopers is
    /// sucks the brains out of the mobile application and extracts them
    /// onto the hardrive for debugging.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Suck the brains out of the app
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("OpenIZ BrainBug - Android Extraction Tool");
            Console.WriteLine("Version {0}", Assembly.GetEntryAssembly().GetName().Version);
            
            var parameters = new ParameterParser<ConsoleParameters>().Parse(args);

            if(parameters.Help)
            {
                new ParameterParser<ConsoleParameters>().WriteHelp(Console.Out);
                return;
            }
            if (parameters.TargetFile == null && parameters.ExtractDir == null)
            {
                Console.WriteLine("Either --tar or --extract must be specified");
                return;
            }

            if (parameters.PackageId != null)
            {
                var exeFile = Path.Combine(parameters.SdkPath, "platform-tools", "adb.exe");
                StringBuilder argStr = new StringBuilder("backup ");
                argStr.AppendFormat("-f \"backup.ab\"", parameters.BackupFile);
                argStr.Append(" -noapk -noobb -shared ");
                argStr.Append(parameters.PackageId);
                Console.WriteLine("Starting {0} {1}", exeFile, argStr.ToString());
                var pi = new Process();
                pi.StartInfo.FileName = String.Format("\"{0}\"", exeFile);
                pi.StartInfo.Arguments = argStr.ToString();
                pi.StartInfo.CreateNoWindow = true;
                pi.StartInfo.RedirectStandardError = true;
                pi.StartInfo.RedirectStandardOutput = true;
                pi.StartInfo.UseShellExecute = false;
                pi.Start();
                Console.WriteLine(pi.StandardOutput.ReadToEnd());
                Console.WriteLine(pi.StandardError.ReadToEnd());
                pi.WaitForExit();

                if (File.Exists(parameters.BackupFile))
                    File.Delete(parameters.BackupFile);
                File.Move("backup.ab", parameters.BackupFile);
            }

            if (!File.Exists(parameters.BackupFile))
            {
                Console.WriteLine("Cannot find specified backup file!");
                return;
            }

            try
            {
                Console.WriteLine("Extracting {0}...", parameters.BackupFile);
                byte[] buffer = new byte[1024];
                using (FileStream ins = File.OpenRead(parameters.BackupFile))
                {
                    ins.Read(buffer, 0, 24);
                    String magic = System.Text.Encoding.UTF8.GetString(buffer, 0, 24);
                    //ins.Seek(24, SeekOrigin.Begin);
                    using (FileStream outs = File.Create(parameters.TargetFile))
                    {
                        using (ZLibNet.ZLibStream df = new ZLibNet.ZLibStream(ins, ZLibNet.CompressionMode.Decompress))
                        {
                            int br = 1024;
                            while (br == 1024)
                            {
                                br = df.Read(buffer, 0, 1024);
                                outs.Write(buffer, 0, br);
                            }
                        }
                    }
                }

                // Extract
                if (parameters.ExtractDir != null)
                {
                    if (!Directory.Exists(parameters.ExtractDir))
                        Directory.CreateDirectory(parameters.ExtractDir);
                    using (var fs = File.OpenRead(parameters.TargetFile))
                    using (var tar = TarReader.Open(fs))
                        while (tar.MoveToNextEntry())
                        {

                            string outName = Path.Combine(parameters.ExtractDir, tar.Entry.FilePath);
                            if (!Directory.Exists(Path.GetDirectoryName(outName)))
                                Directory.CreateDirectory(Path.GetDirectoryName(outName));
                            Console.WriteLine("{0} > {1}", tar.Entry.FilePath, outName);

                            if (!tar.Entry.IsDirectory)
                                using (var ofs = File.Create(outName))
                                    tar.WriteEntryTo(ofs);
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
