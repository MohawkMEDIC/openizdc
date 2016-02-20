using System;
using System.Net;

namespace OpenIZ.Mobile.Core.Http
{
	/// <summary>
	/// Rest client exception.
	/// </summary>
	public class RestClientException<TResult> : System.Net.WebException
	{
		/// <summary>
		/// The result
		/// </summary>
		/// <value>The result.</value>
		public TResult Result {
			get;
			set;
		}

		/// <summary>
		/// Create the client exception
		/// </summary>
		public RestClientException (TResult result, Exception inner, WebExceptionStatus status, WebResponse response) : base("Request failed", inner, status, response)
		{
			this.Result = result;
		}
	}
}

