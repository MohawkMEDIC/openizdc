using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Core.Services;
using OpenIZ.Core.Protocol;
using OpenIZ.Core.Model.Acts;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using System.Xml.Serialization;
using OpenIZ.Core.Diagnostics;

namespace OpenIZ.Mobile.Core.Android.Services
{
    /// <summary>
    /// Represents a clinical repository service which uses the file system
    /// </summary>
    public class AndroidClinicalProtocolRepositoryService : IClinicalProtocolRepositoryService
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(AndroidClinicalProtocolRepositoryService));

        // Protocols loaded
        private List<OpenIZ.Core.Model.Acts.Protocol> m_protocols = new List<OpenIZ.Core.Model.Acts.Protocol>();

        /// <summary>
        /// Clinical repository service
        /// </summary>
        public AndroidClinicalProtocolRepositoryService()
        {
            try
            {
                var section = ApplicationContext.Current.Configuration.GetSection<ForecastingConfigurationSection>();

                if (!Directory.Exists(section.ProtocolSourceDirectory))
                    Directory.CreateDirectory(section.ProtocolSourceDirectory);
                foreach (var f in Directory.GetFiles(section.ProtocolSourceDirectory))
                {
                    XmlSerializer xsz = new XmlSerializer(typeof(OpenIZ.Core.Model.Acts.Protocol));
                    using (var stream = File.OpenRead(f))
                        this.m_protocols.Add(xsz.Deserialize(stream) as OpenIZ.Core.Model.Acts.Protocol);
                }
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error loading protocols: {0}", e);
            }
        }

        /// <summary>
        /// Find the specified protocol
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OpenIZ.Core.Model.Acts.Protocol> FindProtocol(Expression<Func<OpenIZ.Core.Model.Acts.Protocol, bool>> predicate, int offset, int? count, out int totalResults)
        {
            var retVal = this.m_protocols.Where(predicate.Compile()).Skip(offset).Take(count ?? 100);
            totalResults = retVal.Count();
            return retVal;
        }

        /// <summary>
        /// Insert the specified protocol
        /// </summary>
        public OpenIZ.Core.Model.Acts.Protocol InsertProtocol(OpenIZ.Core.Model.Acts.Protocol data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            // Generate key
            if (data.Key == null) data.Key = Guid.NewGuid();
            data.CreationTime = DateTime.Now;

            // Section and save
            var section = ApplicationContext.Current.Configuration.GetSection<ForecastingConfigurationSection>();
            if (!Directory.Exists(section.ProtocolSourceDirectory))
                Directory.CreateDirectory(section.ProtocolSourceDirectory);

            XmlSerializer xsz = new XmlSerializer(typeof(OpenIZ.Core.Model.Acts.Protocol));
            using (var stream = File.Create(Path.Combine(section.ProtocolSourceDirectory, data.Key.ToString())))
                xsz.Serialize(stream, data);

            if(!this.m_protocols.Any(o=>o.Key == data.Key))
                this.m_protocols.Add(data);

            return data;
        }
    }
}