using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents a class which is linked to an entity
	/// </summary>
	public abstract class DbEntityLink : DbIdentified
	{

		/// <summary>
		/// Gets or sets the entity identifier.
		/// </summary>
		/// <value>The entity identifier.</value>
		[Column("entity_uuid"), Indexed, MaxLength(16), NotNull]
		public byte[] EntityUuid {
			get;
			set;
		}



    }
}

