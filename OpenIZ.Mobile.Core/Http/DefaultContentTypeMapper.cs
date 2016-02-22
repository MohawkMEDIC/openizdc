using System;

namespace OpenIZ.Mobile.Core.Http
{
	/// <summary>
	/// Default body binder.
	/// </summary>
	internal class DefaultContentTypeMapper : IContentTypeMapper
	{
		#region IBodySerializerBinder implementation

		/// <summary>
		/// Gets the body serializer based on the content type
		/// </summary>
		/// <returns>The serializer.</returns>
		/// <param name="contentType">Content type.</param>
		public IBodySerializer GetSerializer (string contentType, Type typeHint)
		{
			switch (contentType) {
				case "text/xml":
				case "application/xml":
					return new XmlBodySerializer (typeHint);
				case "application/json":
					return new JsonBodySerializer (typeHint);
				default:
					throw new InvalidOperationException ();
			}
		}
		#endregion
	}
}

