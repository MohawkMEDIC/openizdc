using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Extensibility
{
	/// <summary>
	/// Represents a simpe tag (version independent)
	/// </summary>
	public abstract class DbTag : DbIdentified
	{
		/// <summary>
		/// Gets or sets the source.
		/// </summary>
		/// <value>The source.</value>
		[Column("source"), NotNull]
		public int Source {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		[Column("key"), Indexed, NotNull]
		public String Key {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[Column("value"), NotNull]
		public String Value {
			get;
			set;
		}
	}

	/// <summary>
	/// Represents a tag associated with an enttiy
	/// </summary>
	[Table("entity_tag")]
	public class DbEntityTag : DbTag
	{
	}

	/// <summary>
	/// Represents a tag associated with an act
	/// </summary>
	[Table("act_tag")]
	public class DbActTag : DbTag
	{
	}

}

