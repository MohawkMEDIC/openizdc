using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
    /// <summary>
    /// Represents a person's languages of communication
    /// </summary>
    [Table("person_language")]
    public class DbPersonLanguageCommunication : DbEntityLink
    {
        /// <summary>
        /// Gets or sets the language code of the communication language
        /// </summary>
        [Column("languageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets wheter this person prefers to be contacted in this language
        /// </summary>
        [Column("isPreferred")]
        public bool IsPreferred { get; set; }


    }
}
