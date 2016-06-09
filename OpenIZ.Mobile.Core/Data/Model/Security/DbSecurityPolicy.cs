using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Model.Security
{
	/// <summary>
	/// Represents a single security policy
	/// </summary>
	[Table("security_policy")]
	public class DbSecurityPolicy : DbIdentified
	{

		/// <summary>
		/// Gets or sets the handler.
		/// </summary>
		/// <value>The handler.</value>
		[Column("handler")]
		public String Handler {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Column("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is public.
		/// </summary>
		/// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
		[Column("is_public")]
		public bool IsPublic {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance can override.
		/// </summary>
		/// <value><c>true</c> if this instance can override; otherwise, <c>false</c>.</value>
		[Column("can_override")]
		public bool CanOverride {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the policy oid
        /// </summary>
        [Column("oid"), Unique]
        public String Oid
        {
            get;
            set;
        }


    }
}

