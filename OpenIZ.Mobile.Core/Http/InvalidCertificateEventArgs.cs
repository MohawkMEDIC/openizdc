using System;
using OpenIZ.Mobile.Core.Authentication;

namespace OpenIZ.Mobile.Core.Http
{
	/// <summary>
	/// Fired when there are invalid certificate is encountered
	/// </summary>
	public class InvalidCertificateEventArgs : EventArgs
	{

		/// <summary>
		/// When set to true, indicates that the user does not trust the certificate
		/// </summary>
		/// <value><c>true</c> if this instance cancel; otherwise, <c>false</c>.</value>
		public bool Cancel {
			get;
			set;
		}

		/// <summary>
		/// Gets the certificate distinguished name which is invalid
		/// </summary>
		/// <value>The certificate.</value>
		public String CertificateDN {
			get;
			set;
		}

	}

}

