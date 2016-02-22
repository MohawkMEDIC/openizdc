using System;
using OpenIZ.Mobile.Core.Http;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using OpenIZ.Mobile.Core.Authentication;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;

namespace OpenIZ.Mobile.Core.Android.Http
{
	/// <summary>
	/// Represents an android enabled rest client
	/// </summary>
	public class RestClient : RestClientBase
	{

		// Tracer
		private Tracer m_tracer;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Http.RestClient"/> class.
		/// </summary>
		public RestClient () : base()
		{
			this.m_tracer = TracerHelper.GetTracer (this.GetType ());
			this.ClientCertificates = new X509Certificate2Collection ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Http.RestClient"/> class.
		/// </summary>
		public RestClient (ServiceClient config) : base(config)
		{
			this.m_tracer = TracerHelper.GetTracer (this.GetType ());
			this.ClientCertificates = new X509Certificate2Collection ();
			// Find the specified certificate


		}

		/// <summary>
		/// Create HTTP Request object
		/// </summary>
		protected override WebRequest CreateHttpRequest (string url, params KeyValuePair<string, object>[] query)
		{
			var retVal = (HttpWebRequest)base.CreateHttpRequest (url, query);
			retVal.ClientCertificates = this.ClientCertificates;
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
		public override TResult Invoke<TBody, TResult> (string method, string url, string contentType, TBody body, params KeyValuePair<string, object>[] query)
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
					triedAuth = this.Credentials;
					IBodySerializer serializer = null;
					if (body != null) {
						if (contentType == null)
							throw new ArgumentNullException (nameof (contentType));
						
						serializer = this.Description.Binding.ContentTypeMapper.GetSerializer (contentType, typeof(TBody));
						// Serialize
						serializer.Serialize (requestObj.GetRequestStream (), body);
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

					var retVal = (TResult)serializer.DeSerialize (response.GetResponseStream ());
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

