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
using System;
using SQLite.Net;
using System.Xml.Serialization;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Configuration.Data;
using System.IO;
using Newtonsoft.Json;
using OpenIZ.Core.Http.Description;
using System.Linq;
using OpenIZ.Core.Http;

namespace OpenIZ.Mobile.Core.Configuration
{

	/// <summary>
	/// Service client configuration
	/// </summary>
	[XmlType (nameof (ServiceClientConfigurationSection), Namespace = "http://openiz.org/mobile/configuration"), JsonObject(nameof(ServiceClientConfigurationSection))]
	public class ServiceClientConfigurationSection : IConfigurationSection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Configuration.ServiceClientConfigurationSection"/> class.
		/// </summary>
		public ServiceClientConfigurationSection ()
		{
			this.Client = new List<ServiceClientDescription> ();
		}

		/// <summary>
		/// Gets or sets the proxy address.
		/// </summary>
		/// <value>The proxy address.</value>
		[XmlElement("proxyAddress"), JsonProperty("proxyAddress")]
		public String ProxyAddress {
			get;
			set;
		}

		/// <summary>
		/// Represents a service client
		/// </summary>
		/// <value>The client.</value>
		[XmlElement("client")]
		public List<ServiceClientDescription> Client {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type which is to be used for rest clients
		/// </summary>
		/// <value>The rest client type xml.</value>
		[XmlAttribute ("clientType")]
		public String RestClientTypeXml {
			get { return this.RestClientType?.AssemblyQualifiedName; }
			set {
				this.RestClientType = Type.GetType (value);
			}
		}

		/// <summary>
		/// Gets or sets the rest client implementation
		/// </summary>
		/// <value>The type of the rest client.</value>
		[XmlIgnore]
		public Type RestClientType {
			get;
			set;
		}
	}

	/// <summary>
	/// A service client reprsent a single client to a service 
	/// </summary>
	[XmlType (nameof (ServiceClientDescription), Namespace = "http://openiz.org/mobile/configuration")]
	public class ServiceClientDescription : IRestClientDescription
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Configuration.ServiceClient"/> class.
		/// </summary>
		public ServiceClientDescription ()
		{
			this.Endpoint = new List<ServiceClientEndpoint> ();
		}

		/// <summary>
		/// Gets or sets the name of the service client
		/// </summary>
		/// <value>The name.</value>
		[XmlAttribute ("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// The endpoints of the client
		/// </summary>
		/// <value>The endpoint.</value>
		[XmlElement ("endpoint")]
		public List<ServiceClientEndpoint> Endpoint {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the binding for the service client.
		/// </summary>
		/// <value>The binding.</value>
		[XmlElement ("binding")]
		public ServiceClientBinding Binding {
			get;
			set;
		}

        /// <summary>
        /// Gets the endpoints
        /// </summary>
        List<IRestClientEndpointDescription> IRestClientDescription.Endpoint
        {
            get
            {
                return this.Endpoint.OfType<IRestClientEndpointDescription>().ToList();
            }
        }

        /// <summary>
        /// Gets the binding
        /// </summary>
        IRestClientBindingDescription IRestClientDescription.Binding
        {
            get
            {
                return this.Binding;
            }
        }

        /// <summary>
        /// Gets or sets the trace
        /// </summary>
        [XmlElement("trace")]
        public bool Trace { get; set; }

        /// <summary>
        /// Clone the object
        /// </summary>
        public ServiceClientDescription Clone()
        {
            var retVal = this.MemberwiseClone() as ServiceClientDescription;
            retVal.Endpoint = new List<ServiceClientEndpoint>(this.Endpoint.Select(o => new ServiceClientEndpoint()
            {
                Address = o.Address,
                Timeout = o.Timeout
            }));
            return retVal;
        }
    }

	/// <summary>
	/// Service client binding
	/// </summary>
	[XmlType (nameof (ServiceClientBinding), Namespace = "http://openiz.org/mobile/configuration")]
	public class ServiceClientBinding : IRestClientBindingDescription
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientBinding"/> class.
        /// </summary>
        public ServiceClientBinding ()
		{
			this.ContentTypeMapper = new DefaultContentTypeMapper ();
		}

		/// <summary>
		/// Gets or sets the type which dictates how a body maps to a 
		/// </summary>
		/// <value>The serialization binder type xml.</value>
		[XmlAttribute ("contentTypeMapper")]
		public string ContentTypeMapperXml {
			get { return this.ContentTypeMapper?.GetType().AssemblyQualifiedName; }
			set { this.ContentTypeMapper = Activator.CreateInstance(Type.GetType (value)) as IContentTypeMapper; }
		}

		/// <summary>
		/// Content type mapper
		/// </summary>
		/// <value>The content type mapper.</value>
		[XmlIgnore]
		public IContentTypeMapper ContentTypeMapper {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the security configuration
		/// </summary>
		/// <value>The security.</value>
		[XmlElement ("security")]
		public ServiceClientSecurity Security {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="OpenIZ.Mobile.Core.Configuration.ServiceClientBinding"/>
		/// is optimized
		/// </summary>
		/// <value><c>true</c> if optimize; otherwise, <c>false</c>.</value>
		[XmlElement("optimize")]
		public bool Optimize
		{
			get;set;
		}

        /// <summary>
        /// Gets or sets the optimization method
        /// </summary>
        [XmlElement("method")]
        public OptimizationMethod OptimizationMethod { get; set; }

        /// <summary>
        /// Gets the security description
        /// </summary>
        IRestClientSecurityDescription IRestClientBindingDescription.Security
        {
            get
            {
                return this.Security;
            }
        }
    }

    /// <summary>
    /// Optimization method
    /// </summary>
    [XmlType(nameof(OptimizationMethod), Namespace = "http://openiz.org/mobile/configuration")]
    public enum OptimizationMethod
    {
        [XmlEnum("off")]
        None = 0,
        [XmlEnum("df")]
        Deflate = 1,
        [XmlEnum("gz")]
        Gzip = 2,
        [XmlEnum("bz2")]
        Bzip2 = 3,
        [XmlEnum("7z")]
        Lzma = 4
    }

    /// <summary>
    /// Service client security configuration
    /// </summary>
    [XmlType (nameof (ServiceClientSecurity), Namespace = "http://openiz.org/mobile/configuration")]
	public class ServiceClientSecurity : IRestClientSecurityDescription
	{

		/// <summary>
		/// Gets or sets the ICertificateValidator interface which should be called to validate 
		/// certificates 
		/// </summary>
		/// <value>The serialization binder type xml.</value>
		[XmlAttribute ("certificateValidator")]
		public string CertificateValidatorXml {
			get { return this.CertificateValidator?.GetType ().AssemblyQualifiedName; }
			set { this.CertificateValidator = Activator.CreateInstance (Type.GetType (value)) as ICertificateValidator; }
		}

		/// <summary>
		/// Gets or sets the certificate validator.
		/// </summary>
		/// <value>The certificate validator.</value>
		[XmlIgnore]
		public ICertificateValidator CertificateValidator {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ICredentialProvider
		/// </summary>
		/// <value>The credential provider xml.</value>
		[XmlAttribute ("credentialProvider")]
		public string CredentialProviderXml {
			get { return this.CredentialProvider?.GetType ().AssemblyQualifiedName; }
			set { this.CredentialProvider = Activator.CreateInstance (Type.GetType (value)) as ICredentialProvider; }
		}

		/// <summary>
		/// Security mode
		/// </summary>
		/// <value>The mode.</value>
		[XmlAttribute ("mode")]
		public SecurityScheme Mode {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the credential provider.
		/// </summary>
		/// <value>The credential provider.</value>
		[XmlIgnore]
		public ICredentialProvider CredentialProvider {
			get;
			set;
		}


		/// <summary>
		/// Gets the thumbprint the device should use for authentication
		/// </summary>
		[XmlElement("certificate")]
		public ServiceCertificateConfiguration ClientCertificate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the authentication realm this client should verify
		/// </summary>
		/// <value>The auth realm.</value>
		[XmlAttribute("realm")]
		public String AuthRealm {
			get;
			set;
		}

        /// <summary>
        /// Gets certificate find
        /// </summary>
        IRestClientCertificateDescription IRestClientSecurityDescription.ClientCertificate
        {
            get
            {
                return this.ClientCertificate;
            }
        }

        /// <summary>
        /// Preemptive authentication
        /// </summary>
        [XmlElement("preAuth")]
        public bool PreemptiveAuthentication { get; set; }
    }

	/// <summary>
	/// Service certificate configuration
	/// </summary>
	[XmlType(nameof(ServiceCertificateConfiguration), Namespace = "http://openiz.org/mobile/configuration")]
	public class ServiceCertificateConfiguration : IRestClientCertificateDescription
	{
		/// <summary>
		/// Gets or sets the type of the find.
		/// </summary>
		/// <value>The type of the find.</value>
		[XmlAttribute("x509FindType")]
		public String FindType {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the store.
		/// </summary>
		/// <value>The name of the store.</value>
		[XmlAttribute("storeName")]
		public String StoreName {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the store location.
		/// </summary>
		/// <value>The store location.</value>
		[XmlAttribute("storeLocation")]
		public String StoreLocation {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the find value.
		/// </summary>
		/// <value>The find value.</value>
		[XmlAttribute("findValue")]
		public String FindValue {
			get;
			set;
		}
	}


	/// <summary>
	/// Represnts a single endpoint for use in the service client
	/// </summary>
	[XmlType (nameof (ServiceClientEndpoint), Namespace = "http://openiz.org/mobile/configuration")]
	public class ServiceClientEndpoint : IRestClientEndpointDescription
	{
        /// <summary>
        /// Timeout of 4 sec
        /// </summary>
        public ServiceClientEndpoint()
        {
            this.Timeout = 30000;
        }

		/// <summary>
		/// Gets or sets the service client endpoint's address
		/// </summary>
		/// <value>The address.</value>
		[XmlAttribute ("address")]
		public String Address {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the timeout
		/// </summary>
		[XmlAttribute("timeout")]
		public int Timeout { get; set; }
	}


}

