using System;

namespace OpenIZ.Mobile.Core.Exceptions
{
	/// <summary>
	/// Configuration exception
	/// </summary>
	public class ConfigurationException : Exception
	{
		/// <summary>
		/// Configuration exception
		/// </summary>
		public ConfigurationException (String message) : base(message)
		{
		}
	}
}

