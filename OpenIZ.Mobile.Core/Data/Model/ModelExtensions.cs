using System;

namespace OpenIZ.Mobile.Core.Data.Model
{
	/// <summary>
	/// Model extensions.
	/// </summary>
	public static class ModelExtensions
	{
		/// <summary>
		/// Represents a byte array as a guid.
		/// </summary>
		/// <returns>The GUID.</returns>
		/// <param name="me">Me.</param>
		public static Guid? ToGuid(this byte[] me)
		{
			return me == null ? (Guid?)null : new Guid (me);
		}
	}
}

