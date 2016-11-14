/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-10-11
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenIZ.Core.Services;
using OpenIZ.Core.Protocol;
using OpenIZ.Core.Model.Acts;
using System.Linq.Expressions;
using OpenIZ.Mobile.Core.Configuration;
using System.IO;
using System.Xml.Serialization;
using OpenIZ.Core.Diagnostics;

namespace OpenIZ.Mobile.Core.Xamarin.Services
{
    /// <summary>
    /// Represents a clinical repository service which uses the file system
    /// </summary>
    public class SimpleClinicalProtocolRepositoryService : IClinicalProtocolRepositoryService
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SimpleClinicalProtocolRepositoryService));

        // Protocols loaded
        private List<OpenIZ.Core.Model.Acts.Protocol> m_protocols = new List<OpenIZ.Core.Model.Acts.Protocol>();

        /// <summary>
        /// Clinical repository service
        /// </summary>
        public SimpleClinicalProtocolRepositoryService()
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