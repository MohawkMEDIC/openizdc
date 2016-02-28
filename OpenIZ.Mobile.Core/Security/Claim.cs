using System;
using System.Security.Principal;
using System.Linq;
using System.Collections.Generic;

namespace OpenIZ.Mobile.Core.Security
{

	/// <summary>
	/// Represents a single claim made about a user
	/// </summary>
	public class Claim
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Security.Claim"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		public Claim (String type, String value)
		{
			this.Type = type;
			this.Value = value;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public String Type {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public String Value {
			get;
			set;
		}
	}


}

