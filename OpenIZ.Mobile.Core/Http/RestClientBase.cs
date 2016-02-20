using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace OpenIZ.Mobile.Core.Http
{
	/// <summary>
	/// Represents a simple rest client 
	/// </summary>
	public abstract class RestClientBase : IRestClient
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Http.RestClient"/> class.
		/// </summary>
		public RestClientBase ()
		{
			this.SerializationBinder = new DefaultBodySerializerBinder ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Http.RestClient"/> class.
		/// </summary>
		/// <param name="binder">The serialization binder to use.</param>
		public RestClientBase (IBodySerializerBinder binder)
		{
			this.SerializationBinder = binder;
		}

		/// <summary>
		/// Create the HTTP request
		/// </summary>
		/// <param name="url">URL.</param>
		protected virtual WebRequest CreateHttpRequest(String url, params KeyValuePair<String, Object>[] query)
		{

			// Add query string
			if (query != null) {
				String queryString = String.Empty;
				foreach (var kv in query) {
					queryString += String.Format ("{0}={1}", kv.Key, Uri.EscapeDataString (kv.Value.ToString ()));
					if (!kv.Equals(query.Last ()))
						queryString += "&";
				}
				if (url.Contains ("?"))
					url += "&" + queryString;
				else
					url += "?" + queryString;
			}

			Uri uri = new Uri (url);

			// Add headers
			WebRequest retVal = HttpWebRequest.Create(uri.ToString());
			if (this.Credentials != null) {
				foreach (var kv in this.Credentials.GetHttpHeaders ()) {
					retVal.Headers[kv.Key] = kv.Value;
				}
			}


			// Compress?
			if (!String.IsNullOrEmpty(this.Accept)) {
				retVal.Headers ["Accept"] = this.Accept;
			}
					
			return retVal;
		}

		#region IRestClient implementation
		/// <summary>
		/// Occurs when invalid certificate.
		/// </summary>
		public event EventHandler<InvalidCertificateEventArgs> InvalidCertificate;
		/// <summary>
		/// Occurs when unauthorized.
		/// </summary>
		public event EventHandler<AuthorizationEventArgs> Unauthorized;

		/// <summary>
		/// Fires the invalid certificate event to let the user know that there was something wrong with the
		/// trust relationship
		/// </summary>
		protected bool FireInvalidCertificate(String remoteCertificateDN)
		{
			InvalidCertificateEventArgs e = new InvalidCertificateEventArgs () {
				CertificateDN = remoteCertificateDN
			};

			this.InvalidCertificate?.Invoke (this, e);

			return e.Cancel;
		}

		/// <summary>
		/// Fires the unauthorized event
		/// </summary>
		protected void FireUnauthorized()
		{
			AuthorizationEventArgs e = new AuthorizationEventArgs () {
				Credentials = this.Credentials
			};
			this.Unauthorized?.Invoke(this, e);
			this.Credentials = e.Credentials;
		}

		/// <summary>
		/// Gets the specified item
		/// </summary>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="queryString">Query string.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <param name="url">URL.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		public TResult Get<TResult> (string url)
		{
			return this.Get<TResult> (url, null);
		}

		/// <summary>
		/// Gets a inumerable result set of type T
		/// </summary>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="query">Query.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <param name="url">URL.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		public TResult Get<TResult> (string url, params KeyValuePair<string, object>[] query)
		{
			return this.Invoke<Object, TResult> ("GET", url, null, null, query);
		}
		/// <summary>
		/// Invokes the specified method against the URL provided
		/// </summary>
		/// <param name="method">Method.</param>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="contentType">Content type.</param>
		/// <param name="body">Body.</param>
		/// <typeparam name="TBody">The 1st type parameter.</typeparam>
		/// <typeparam name="TResult">The 2nd type parameter.</typeparam>
		/// <param name="url">URL.</param>
		public TResult Invoke<TBody, TResult> (string method, string url, string contentType, TBody body)
		{
			return this.Invoke<TBody, TResult> (method, url, contentType, body, null);
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
		public abstract TResult Invoke<TBody, TResult> (string method, string url, string contentType, TBody body, params KeyValuePair<string, object>[] query);

		/// <summary>
		/// Executes a post against the url
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="contentType">Content type.</param>
		/// <param name="body">Body.</param>
		/// <typeparam name="TBody">The 1st type parameter.</typeparam>
		/// <typeparam name="TResult">The 2nd type parameter.</typeparam>
		public TResult Post<TBody, TResult> (string url, string contentType, TBody body)
		{
			return this.Invoke<TBody, TResult> ("POST", url, contentType, body);
		}
		/// <summary>
		/// Deletes the specified object
		/// </summary>
		/// <param name="url">URL.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		public TResult Delete<TResult> (string url)
		{
			return this.Invoke<Object, TResult>("DELETE", url, null, null);
		}
		/// <summary>
		/// Executes a PUT for the specified object
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="contentType">Content type.</param>
		/// <param name="body">Body.</param>
		/// <typeparam name="TBody">The 1st type parameter.</typeparam>
		/// <typeparam name="TResult">The 2nd type parameter.</typeparam>
		public TResult Put<TBody, TResult> (string url, string contentType, TBody body)
		{
			return this.Invoke<TBody, TResult> ("PUT", url, contentType, body);
		}
		/// <summary>
		/// Executes an Options against the URL
		/// </summary>
		/// <param name="url">URL.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		public TResult Options<TResult> (string url)
		{
			return this.Invoke<Object, TResult>("OPTIONS", url, null, null);
		}

		/// <summary>
		/// Gets or sets the credentials to be used for this client
		/// </summary>
		/// <value>The credentials.</value>
		public OpenIZ.Mobile.Core.Authentication.Credentials Credentials {
			get;
			set;
		}
			
		/// <summary>
		/// Gets or sets a list of acceptable response formats
		/// </summary>
		/// <value>The accept.</value>
		public string Accept {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the serializer
		/// </summary>
		public IBodySerializerBinder SerializationBinder { get; set; }

		#endregion
	}
}

