using OpenIZ.Mobile.Core.Synchronization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using Newtonsoft.Json;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Diagnostics;

namespace OpenIZ.Mobile.Core.Xamarin.Services
{
    /// <summary>
    /// Represents a file provider which can be used to store/retrieve queue objects
    /// </summary>
    public class SimpleQueueFileProvider : IQueueFileProvider
    {

        // Serializers
        private Dictionary<Type, XmlSerializer> m_serializers = new Dictionary<Type, XmlSerializer>();

        /// <summary>
        /// Copy queue data 
        /// </summary>
        public string CopyQueueData(string data)
        {
            var sqlitePath = ApplicationContext.Current.Configuration.GetConnectionString(ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().MessageQueueConnectionStringName).Value;

            // Create blob path
            var blobPath = Path.Combine(Path.GetDirectoryName(sqlitePath), "blob");
            if (!Directory.Exists(blobPath))
                Directory.CreateDirectory(blobPath);

            data = Path.Combine(blobPath, data);
            blobPath = Path.Combine(blobPath, Guid.NewGuid().ToString() + ".dat");
            File.Copy(data, blobPath);
            return Path.GetFileName(blobPath);
        }

        /// <summary>
        /// Get Queue Data
        /// </summary>
        public IdentifiedData GetQueueData(string pathSpec, Type typeSpec)
        {
#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                XmlSerializer xsz = null;
                if (!this.m_serializers.TryGetValue(typeSpec, out xsz))
                {
                    xsz = new XmlSerializer(typeSpec);
                    lock (this.m_serializers)
                        if(!this.m_serializers.ContainsKey(typeSpec))
                            this.m_serializers.Add(typeSpec, xsz);
                }

                var sqlitePath = ApplicationContext.Current.Configuration.GetConnectionString(ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().MessageQueueConnectionStringName).Value;

                // Create blob path
                var blobPath = Path.Combine(Path.GetDirectoryName(sqlitePath), "blob");
                if (!Directory.Exists(blobPath))
                    Directory.CreateDirectory(blobPath);

                blobPath = Path.Combine(blobPath, pathSpec);
                using (FileStream fs = File.OpenRead(blobPath))
                using (GZipStream gz = new GZipStream(fs, CompressionMode.Decompress))
                using (TextReader tr = new StreamReader(gz))
                    return xsz.Deserialize(tr) as IdentifiedData;
#if PERFMON
            }
            finally
            {
                sw.Stop();
                ApplicationContext.Current.PerformanceLog(nameof(SimpleQueueFileProvider), nameof(GetQueueData), typeSpec.Name, sw.Elapsed);
            }
#endif
        }


        /// <summary>
        /// Get Queue Data
        /// </summary>
        public byte[] GetQueueData(string pathSpec)
        {
#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
            

            var sqlitePath = ApplicationContext.Current.Configuration.GetConnectionString(ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().MessageQueueConnectionStringName).Value;

            // Create blob path
            var blobPath = Path.Combine(Path.GetDirectoryName(sqlitePath), "blob");
            if (!Directory.Exists(blobPath))
                Directory.CreateDirectory(blobPath);

            blobPath = Path.Combine(blobPath, pathSpec);
            using (var fs = File.OpenRead(blobPath))
            using (var gzs = new GZipStream(fs, CompressionMode.Decompress))
            using(var ms = new MemoryStream())
            {
                gzs.CopyTo(ms);
                ms.Flush();
                return ms.ToArray();                    
            }
#if PERFMON
            }
            finally
            {
                sw.Stop();
                ApplicationContext.Current.PerformanceLog(nameof(SimpleQueueFileProvider), nameof(GetQueueData), "Raw", sw.Elapsed);
            }
#endif
        }

        /// <summary>
        /// Remove queue data
        /// </summary>
        public void RemoveQueueData(String pathSpec)
        {
            var sqlitePath = ApplicationContext.Current.Configuration.GetConnectionString(ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().MessageQueueConnectionStringName).Value;

            var blobPath = Path.Combine(Path.GetDirectoryName(sqlitePath), "blob");
            if (!Directory.Exists(blobPath))
                Directory.CreateDirectory(blobPath);

            blobPath = Path.Combine(blobPath, pathSpec);
            if (File.Exists(blobPath))
                File.Delete(blobPath);
        }

        /// <summary>
        /// Save queue data
        /// </summary>
        public string SaveQueueData(IdentifiedData data)
        {
#if PERFMON
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                XmlSerializer xsz = null;
                if (!this.m_serializers.TryGetValue(data.GetType(), out xsz))
                {
                    xsz = new XmlSerializer(data.GetType());
                    lock (this.m_serializers)
                        if (!this.m_serializers.ContainsKey(data.GetType()))
                            this.m_serializers.Add(data.GetType(), xsz);
                }

                var sqlitePath = ApplicationContext.Current.Configuration.GetConnectionString(ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>().MessageQueueConnectionStringName).Value;

                // Create blob path
                var blobPath = Path.Combine(Path.GetDirectoryName(sqlitePath), "blob");
                if (!Directory.Exists(blobPath))
                    Directory.CreateDirectory(blobPath);

                blobPath = Path.Combine(blobPath, Guid.NewGuid().ToString() + ".dat");
                using (FileStream fs = File.Create(blobPath))
                using (GZipStream gz = new GZipStream(fs, CompressionMode.Compress))
                using (TextWriter tw = new StreamWriter(gz))
                    xsz.Serialize(tw, data);
                return Path.GetFileName(blobPath);
#if PERFMON
            }
            finally
            {
                sw.Stop();
                ApplicationContext.Current.PerformanceLog(nameof(SimpleQueueFileProvider), nameof(SaveQueueData), data.GetType().Name, sw.Elapsed);
            }
#endif
        }
    }
}
