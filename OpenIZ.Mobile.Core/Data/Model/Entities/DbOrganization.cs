using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents an organization in the data store
	/// </summary>
	[Table("organization")]
	public class DbOrganization : DbEntityLink
	{
		/// <summary>
		/// Gets or sets the industry concept.
		/// </summary>
		/// <value>The industry concept.</value>
		[Column("industry_concept_uuid"), MaxLength(16)]
		public byte[] IndustryConceptUuid {
			get;
			set;
		}

	}
}

