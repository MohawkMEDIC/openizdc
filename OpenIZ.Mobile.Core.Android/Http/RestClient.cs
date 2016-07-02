using System;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Android.Security;
using System.Security;
using System.IO.Compression;
using OpenIZ.Core.Http;

namespace OpenIZ.Mobile.Core.Android.Http
{
	/// <summary>
	/// Represents an android enabled rest client
	/// </summary>
	public class RestClient : RestClientBase
	{

		// Config section
		private ServiceClientConfigurationSection m_configurationSection;
		// Tracer
		private Tracer m_tracer;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Http.RestClient"/> class.
		/// </summary>
		public RestClient () : base()
		{
			this.m_tracer = Tracer.GetTracer (this.GetType ());
			this.m_configurationSection = ApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection> ();

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Http.RestClient"/> class.
		/// </summary>
		public RestClient (ServiceClientDescription config) : base(config)
		{
			this.m_configurationSection = ApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection> ();
			this.m_tracer = Tracer.GetTracer (this.GetType ());
			// Find the specified certificate
			if (config.Binding.Security.ClientCertificate != null) {
				this.ClientCertificates = new X509Certificate2Collection ();
				var cert = X509CertificateUtils.FindCertificate (config.Binding.Security.ClientCertificate.FindType, 
					          config.Binding.Security.ClientCertificate.StoreLocation,
					          config.Binding.Security.ClientCertificate.StoreName,
					          config.Binding.Security.ClientCertificate.FindValue);
				if (cert == null)
					throw new SecurityException (String.Format("Certificate described by {0} could not be found in {1}/{2}", 
						config.Binding.Security.ClientCertificate.FindValue,
						config.Binding.Security.ClientCertificate.StoreLocation,
						config.Binding.Security.ClientCertificate.StoreName));
				this.ClientCertificates.Add (cert);
			}
		}

		/// <summary>
		/// Create HTTP Request object
		/// </summary>
		protected override WebRequest CreateHttpRequest (string url, params KeyValuePair<string, object>[] query)
		{
			var retVal = (HttpWebRequest)base.CreateHttpRequest (url, query);

			// Certs?
			if(this.ClientCertificates != null)
				retVal.ClientCertificates.AddRange(this.ClientCertificates);

			// Compress?
			if(this.Description.Binding.Optimize)
				retVal.Headers.Add (HttpRequestHeader.AcceptEncoding, "deflate gzip");

			// Proxy?
			if (!String.IsNullOrEmpty (this.m_configurationSection.ProxyAddress))
				retVal.Proxy = new WebProxy (this.m_configurationSection.ProxyAddress);
			return retVal;
		}

		/// <summary>
		/// Invokes the specified method against the url provided
		/// </summary>
		/// <param name="method">Method.</param>
		/// <param name="url">URL.</param>
		/// <param name="contentType">Content type.</param>
		/// <param name="body">Body.</param>
		/// <param name="query">Query.</param>
		/// <typeparam name="TBody">The 1st type parameter.</typeparam>
		/// <typeparam name="TResult">The 2nd type parameter.</typeparam>
		protected override TResult InvokeInternal<TBody, TResult> (string method, string url, string contentType, TBody body, params KeyValuePair<string, object>[] query)
		{

			if (String.IsNullOrEmpty (method))
				throw new ArgumentNullException (nameof (method));
			if (String.IsNullOrEmpty (url))
				throw new ArgumentNullException (nameof (url));

			var requestObj = this.CreateHttpRequest (url, query);
			if (!String.IsNullOrEmpty (contentType))
				requestObj.ContentType = contentType;
			requestObj.Method = method;

			// Body was provided?
			Credentials triedAuth = null;
			while(triedAuth != this.Credentials)
				try {

					// Try assigned credentials
					triedAuth = this.Credentials;
					IBodySerializer serializer = null;
					if (body != null) {
						if (contentType == null)
							throw new ArgumentNullException (nameof (contentType));
						
						serializer = this.Description.Binding.ContentTypeMapper.GetSerializer (contentType, typeof(TBody));
						// Serialize and compress with deflate
						if(this.Description.Binding.Optimize)
						{
							requestObj.Headers.Add(HttpResponseHeader.ContentEncoding, "deflate");
							using(var df = new DeflateStream(requestObj.GetRequestStream(), CompressionMode.Compress))
								serializer.Serialize (df, body);
						}
						else
							serializer.Serialize(requestObj.GetRequestStream(), body);
					}

					// Response
					var response = requestObj.GetResponse ();
					var validationResult = this.ValidateResponse(response);
					if(validationResult != ServiceClientErrorType.Valid)
					{
						this.m_tracer.TraceError("Response failed validation : {0}", validationResult);
						throw new WebException("Response failed validation", null, WebExceptionStatus.Success, response);
					}	
					// De-serialize
					serializer = this.Description.Binding.ContentTypeMapper.GetSerializer (response.ContentType, typeof(TResult));

					// Compression?
					switch(response.Headers[HttpResponseHeader.ContentEncoding])
					{
						case "deflate":
							using(DeflateStream df = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
								return (TResult)serializer.DeSerialize(df);
							break;
						case "gzip":
							using(GZipStream df = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
								return (TResult)serializer.DeSerialize(df);
						break;
							default:
								return (TResult)serializer.DeSerialize (response.GetResponseStream ());
								break;
					}
				} catch (WebException e) {

					this.m_tracer.TraceError (e.ToString ());

					// status
					switch (e.Status) {
						case WebExceptionStatus.ProtocolError:

								// Deserialize
							var errorResponse = (e.Response as HttpWebResponse);
							var serializer = this.Description.Binding.ContentTypeMapper.GetSerializer (e.Response.ContentType, typeof(TResult));
							TResult result = (TResult)serializer.DeSerialize (e.Response.GetResponseStream ());

							switch (errorResponse.StatusCode) {
								case HttpStatusCode.Unauthorized: // Validate the response
									if (this.ValidateResponse (errorResponse) != ServiceClientErrorType.Valid)
										throw new RestClientException<TResult> (
											result, 
											e, 
											e.Status, 
											e.Response); 

									break;						
								default:
									throw new RestClientException<TResult> (
										result, 
										e, 
										e.Status, 
										e.Response); 
							}
							break;
						default:
							throw;
					}
				}
			return default(TResult);
		}

		/// <summary>
		/// Gets or sets the client certificate
		/// </summary>
		/// <value>The client certificate.</value>
		public X509Certificate2Collection ClientCertificates {
			get;
			set;
		}
	}
}

