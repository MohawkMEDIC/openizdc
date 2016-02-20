using System;
using OpenIZ.Mobile.Core.Authentication;

namespace OpenIZ.Mobile.Core.Http
{
	/// <summary>
	/// Authorization event args
	/// </summary>
	public class AuthorizationEventArgs : EventArgs
	{

		/// <summary>
		/// Gets or sets the credentials which are used to authenticate
		/// </summary>
		public Credentials Credentials {
			get;
			set;
		}

	}

}

