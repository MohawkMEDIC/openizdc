using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Extensibility
{
	/// <summary>
	/// Extension.
	/// </summary>
	public abstract class DbExtension : DbIdentified
	{

		/// <summary>
		/// Gets or sets the source identifier.
		/// </summary>
		/// <value>The source identifier.</value>
		[Column ("source"), Indexed]
		public int SourceId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the extension identifier.
		/// </summary>
		/// <value>The extension identifier.</value>
		[Column ("extension_type")]
		public int ExtensionId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[Column ("value")]
		public byte[] Value {
			get;
			set;
		}

	}

	/// <summary>
	/// Entity extension.
	/// </summary>
	[Table ("entity_extension")]
	public class DbEntityExtension : DbExtension
	{
					
	}

	/// <summary>
	/// Act extensions
	/// </summary>
	[Table ("act_extension")]
	public class DbActExtension : DbExtension
	{
	}
}

