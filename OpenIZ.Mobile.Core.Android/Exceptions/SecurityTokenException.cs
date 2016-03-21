using System;
using System.Security;

namespace OpenIZ.Mobile.Core.Android.Exceptions
{

	/// <summary>
	/// Token security exception type.
	/// </summary>
	public enum SecurityTokenExceptionType
	{
		TokenExpired,
		InvalidSignature,
		InvalidTokenType,
		KeyNotFound,
		NotYetValid
	}

	/// <summary>
	/// Represents an error with a token
	/// </summary>
	public class SecurityTokenException : SecurityException
	{

		/// <summary>
		/// Gets or sets the type of exception.
		/// </summary>
		/// <value>The type.</value>
		public SecurityTokenExceptionType Type {
			get;
			set;
		}

		/// <summary>
		/// Details of the exception
		/// </summary>
		/// <value>The detail.</value>
		public String Detail {
			get;
			set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Exceptions.TokenSecurityException"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="detail">Detail.</param>
		public SecurityTokenException (SecurityTokenExceptionType type, String detail) : base(type.ToString())
		{
			this.Type = type;
			this.Detail = detail;
		}
	}
}

