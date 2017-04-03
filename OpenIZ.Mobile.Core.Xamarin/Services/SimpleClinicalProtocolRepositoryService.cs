/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-3-31
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
using OpenIZ.Core.Applets.Model;
using System.Xml.Linq;
using OpenIZ.Protocol.Xml.Model;
using OpenIZ.Protocol.Xml;

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
            
        }

        /// <summary>
        /// Find the specified protocol
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OpenIZ.Core.Model.Acts.Protocol> FindProtocol(Expression<Func<OpenIZ.Core.Model.Acts.Protocol, bool>> predicate, int offset, int? count, out int totalResults)
        {
            if (this.m_protocols == null || this.m_protocols.Count == 0)
                this.LoadProtocols();
            var retVal = this.m_protocols.Where(predicate.Compile()).Skip(offset).Take(count ?? 100);
            totalResults = retVal.Count();
            return retVal;
        }

        /// <summary>
        /// Load protocols
        /// </summary>
        private void LoadProtocols()
        {
            try
            {
                // Get protocols from the applet
                var protocols = XamarinApplicationContext.Current.LoadedApplets.SelectMany(o => o.Assets).Where(o => o.Name.StartsWith("protocols/"));

                foreach (var f in protocols)
                {

                    XmlSerializer xsz = new XmlSerializer(typeof(ProtocolDefinition));
                    var content = f.Content ?? XamarinApplicationContext.Current.LoadedApplets.Resolver(f);
                    if (content is String)
                        using (var rStream = new StringReader(content as String))
                            this.m_protocols.Add(
                                new XmlClinicalProtocol(xsz.Deserialize(rStream) as ProtocolDefinition).GetProtocolData()
                            );
                    else if (content is byte[])
                        using (var rStream = new MemoryStream(content as byte[]))
                            this.m_protocols.Add(new XmlClinicalProtocol(ProtocolDefinition.Load(rStream)).GetProtocolData());
                    else if (content is XElement)
                        using (var rStream = (content as XElement).CreateReader())
                            this.m_protocols.Add(
                                new XmlClinicalProtocol(xsz.Deserialize(rStream) as ProtocolDefinition).GetProtocolData()
                                );
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error loading protocols: {0}", e);
            }
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

            if (!this.m_protocols.Any(o => o.Key == data.Key))
                this.m_protocols.Add(data);

            return data;
        }
    }
}