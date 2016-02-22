using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using OpenIZ.Mobile.Core.Configuration;

namespace OpenIZ.Mobile.Core.Http
{
	/// <summary>
	/// Represents a simple rest client 
	/// </summary>
	public abstract class RestClientBase : IRestClient
	{

		// Configuration
		private ServiceClient m_configuration;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Http.RestClient"/> class.
		/// </summary>
		public RestClientBase ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Http.RestClient"/> class.
		/// </summary>
		/// <param name="binder">The serialization binder to use.</param>
		public RestClientBase (ServiceClient config)
		{
			this.m_configuration = config;
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
		/// Get the description of this service 
		/// </summary>
		/// <value>The description.</value>
		public ServiceClient Description { get { return this.m_configuration; } }

		#endregion

		/// <summary>
		/// Validate the response
		/// </summary>
		/// <returns><c>true</c>, if response was validated, <c>false</c> otherwise.</returns>
		/// <param name="response">Response.</param>
		protected virtual ServiceClientErrorType ValidateResponse(WebResponse response)
		{
			if (response is HttpWebResponse) {
				var httpResponse = response as HttpWebResponse;
				switch (httpResponse.StatusCode) {
					case HttpStatusCode.Unauthorized:
						{
							if (response.Headers ["WWW-Authenticate"]?.StartsWith (this.Description.Binding.Security.Mode.ToString (), StringComparison.CurrentCultureIgnoreCase) == false)
								return ServiceClientErrorType.AuthenticationSchemeMismatch;
							else {
								// Validate the realm
								string wwwAuth = response.Headers ["WWW-Authenticate"];
								int realmStart = wwwAuth.IndexOf ("realm=\"") + 7;
								if (realmStart < 0)
									return ServiceClientErrorType.SecurityError; // No realm
								string realm = wwwAuth.Substring (realmStart, wwwAuth.IndexOf ('"', realmStart) - realmStart);

								if (!String.IsNullOrEmpty (this.Description.Binding.Security.AuthRealm) &&
								    !this.Description.Binding.Security.AuthRealm.Equals (realm))
									return ServiceClientErrorType.RealmMismatch;
								
								// Credential provider
								if (this.Description.Binding.Security.CredentialProvider != null) {
									this.Credentials = this.Description.Binding.Security.CredentialProvider.GetCredentials (this);
									return ServiceClientErrorType.Valid;
								} else
									return ServiceClientErrorType.SecurityError;
								}
						}
					default:
						return ServiceClientErrorType.Valid;
				}
			} else
				return ServiceClientErrorType.GenericError;
		}

	}

	/// <summary>
	/// Service client error type
	/// </summary>
	public enum ServiceClientErrorType
	{
		Valid,
		GenericError,
		AuthenticationSchemeMismatch,
		SecurityError,
		RealmMismatch
	}

}

